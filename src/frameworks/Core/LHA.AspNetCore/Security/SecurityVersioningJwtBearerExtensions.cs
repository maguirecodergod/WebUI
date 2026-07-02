using System.Security.Claims;
using LHA.Core.Security;
using LHA.Shared.Contracts.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.JsonWebTokens;

namespace LHA.AspNetCore.Security;

public static class SecurityVersioningJwtBearerExtensions
{
    public static IServiceCollection AddLHASecurityVersioning(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddMemoryCache();
        services.Configure<SecurityVersioningOptions>(configuration.GetSection(SecurityVersioningOptions.SectionName));
        services.PostConfigure<SecurityVersioningOptions>(options =>
        {
            var redisConnection = configuration.GetConnectionString("Redis")
                ?? configuration["Redis:Configuration"]
                ?? configuration["Redis:ConnectionString"];

            if (!string.IsNullOrWhiteSpace(redisConnection))
            {
                options.RedisConfiguration = redisConnection;
            }
        });
        services.AddSingleton<ISecurityVersionManager, RedisSecurityVersionManager>();
        return services;
    }

    public static JwtBearerOptions EnableSecurityVersionValidation(this JwtBearerOptions options)
    {
        options.Events ??= new JwtBearerEvents();
        var previousMessageReceived = options.Events.OnMessageReceived;
        var previousValidated = options.Events.OnTokenValidated;
        var previousChallenge = options.Events.OnChallenge;

        options.Events.OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];
            var path = context.HttpContext.Request.Path;
            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
            {
                context.Token = accessToken;
            }

            return previousMessageReceived is not null
                ? previousMessageReceived(context)
                : Task.CompletedTask;
        };

        options.Events.OnTokenValidated = async context =>
        {
            if (previousValidated is not null)
            {
                await previousValidated(context);
            }

            if (context.Result?.Failure is not null)
            {
                return;
            }

            var principal = context.Principal;
            if (principal is null)
            {
                return;
            }

            var userId = principal.FindFirstValue(LhaClaimTypes.Subject)
                ?? principal.FindFirstValue(JwtRegisteredClaimNames.Sub)
                ?? principal.FindFirstValue(ClaimTypes.NameIdentifier);

            var issuedAt = GetIssuedAtUnixSeconds(principal);
            if (string.IsNullOrWhiteSpace(userId) || issuedAt is null)
            {
                return;
            }

            var roles = principal.FindAll(LhaClaimTypes.Role)
                .Concat(principal.FindAll(ClaimTypes.Role))
                .Select(c => c.Value)
                .Where(v => !string.IsNullOrWhiteSpace(v))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();

            var manager = context.HttpContext.RequestServices.GetService<ISecurityVersionManager>();
            if (manager is null)
            {
                return;
            }

            var valid = await manager.IsIssuedAtValidAsync(issuedAt.Value, userId, roles, context.HttpContext.RequestAborted);
            if (!valid)
            {
                context.HttpContext.Items[SecurityRevocationConstants.TokenRevokedItemKey] = true;
                context.Fail(SecurityRevocationConstants.SecurityVersionExpired);
            }
        };

        options.Events.OnChallenge = async context =>
        {
            if (context.HttpContext.Items.TryGetValue(SecurityRevocationConstants.TokenRevokedItemKey, out var value)
                && value is true
                && !context.Response.HasStarted)
            {
                context.Response.Headers[SecurityRevocationConstants.TokenRevokedHeaderName] = "true";
            }

            if (previousChallenge is not null)
            {
                await previousChallenge(context);
            }
        };

        return options;
    }

    private static long? GetIssuedAtUnixSeconds(ClaimsPrincipal principal)
    {
        var iat = principal.FindFirstValue(JwtRegisteredClaimNames.Iat) ?? principal.FindFirstValue("iat");
        if (long.TryParse(iat, out var unixSeconds))
        {
            return unixSeconds;
        }

        return null;
    }
}
