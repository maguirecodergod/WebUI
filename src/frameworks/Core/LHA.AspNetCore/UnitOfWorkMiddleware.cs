using LHA.UnitOfWork;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace LHA.AspNetCore;

/// <summary>
/// Middleware that wraps every HTTP request in an ambient <see cref="IUnitOfWork"/>.
/// <para>
/// This ensures that all repository / DbContext access within the request pipeline
/// has an active UoW without requiring each application service to manually call
/// <c>IUnitOfWorkManager.Begin()</c>.
/// </para>
/// <para>
/// On success the UoW is completed (saves changes + commits transactions).
/// On failure (exception), the UoW is disposed without completing, which triggers rollback.
/// </para>
/// </summary>
internal sealed class UnitOfWorkMiddleware
{
    private readonly RequestDelegate _next;

    public UnitOfWorkMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IUnitOfWorkManager unitOfWorkManager, ILogger<UnitOfWorkMiddleware> logger)
    {
        await using var uow = unitOfWorkManager.Begin();

        logger.LogDebug("UoW [{UowId}] started for {Method} {Path}", uow.Id, context.Request.Method, context.Request.Path);

        await _next(context);

        await uow.CompleteAsync(context.RequestAborted);

        logger.LogDebug("UoW [{UowId}] completed", uow.Id);
    }
}
