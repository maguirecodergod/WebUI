using Asp.Versioning;
using Asp.Versioning.Builder;
using Microsoft.AspNetCore.Builder;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class LhaApiVersioningServiceCollectionExtensions
    {
        public static IServiceCollection AddLhaApiVersioning(this IServiceCollection services)
        {
            services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true;

                options.ApiVersionReader = ApiVersionReader.Combine(
                    new UrlSegmentApiVersionReader(),
                    new HeaderApiVersionReader("x-api-version"));
            })
            .AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });

            return services;
        }
    }
}

namespace Microsoft.AspNetCore.Routing
{
    public static class LhaApiVersioningEndpointExtensions
    {
        public static ApiVersionSet CreateLhaApiVersionSet(this IEndpointRouteBuilder endpoints, string moduleName = "")
        {
            return endpoints.NewApiVersionSet(moduleName)
                .HasApiVersion(new ApiVersion(1, 0))
                .HasApiVersion(new ApiVersion(2, 0))
                .ReportApiVersions()
                .Build();
        }

        public static RouteGroupBuilder MapVersionedGroup(this IEndpointRouteBuilder endpoints, string moduleName, string prefix)
        {
            var versionSet = endpoints.CreateLhaApiVersionSet(moduleName);
            return endpoints.MapGroup(prefix)
                .WithApiVersionSet(versionSet)
                .MapToApiVersion(1.0);
        }
    }
}
