using System.Diagnostics;
using LHA.Ddd.Application;
using LHA.Ddd.Domain;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace LHA.AspNetCore;

/// <summary>
/// Global exception handler that converts unhandled exceptions into a
/// standardised <see cref="ApiResponse{T}"/> JSON envelope.
/// <para>
/// Exception → HTTP status mapping:
/// <list type="bullet">
///   <item><see cref="BusinessException"/> → <see cref="BusinessException.StatusCode"/> (default 400)</item>
///   <item><see cref="EntityNotFoundException"/> → 404</item>
///   <item><see cref="UnauthorizedAccessException"/> → 401</item>
///   <item><see cref="ArgumentException"/> / <see cref="FormatException"/> → 400</item>
///   <item><see cref="OperationCanceledException"/> → 499 (Client Closed Request)</item>
///   <item>Everything else → 500</item>
/// </list>
/// </para>
/// </summary>
internal sealed class GlobalExceptionHandler(
    IHostEnvironment environment,
    ILogger<GlobalExceptionHandler> logger,
    IBusinessExceptionLocalizer? exceptionLocalizer = null) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var (statusCode, code, message, errors) = MapException(exception, exceptionLocalizer);

        // Log
        if (statusCode >= 500)
            logger.LogError(exception, "Unhandled server error [{Code}]: {Message}", code, exception.Message);
        else
            logger.LogWarning(exception, "Handled client error [{Code}]: {Message}", code, exception.Message);

        // Build response
        var errorDetails = errors.Count > 0
            ? errors
            : [new ErrorDetailDto { Code = code, Message = message }];

        // In Development, append stack trace as an extra error
        if (environment.IsDevelopment() && statusCode >= 500)
        {
            errorDetails =
            [
                .. errorDetails,
                new ErrorDetailDto
                {
                    Code = "STACK_TRACE",
                    Message = exception.ToString()
                }
            ];
        }

        var response = new ApiResponse<object>
        {
            StatusCode = statusCode,
            Result = new ResponseResult<object>
            {
                Success = false,
                Errors = [.. errorDetails]
            }
        };

        httpContext.Response.StatusCode = statusCode;
        httpContext.Response.ContentType = "application/json";

        // Propagate trace id for correlation
        var traceId = Activity.Current?.Id ?? httpContext.TraceIdentifier;
        httpContext.Response.Headers["X-Trace-Id"] = traceId;

        await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);
        return true;
    }

    private static (int StatusCode, string Code, string Message, List<ErrorDetailDto> Errors)
        MapException(Exception exception, IBusinessExceptionLocalizer? localizer) => exception switch
    {
        // ── Domain / Business exceptions ─────────────────────────────
        Ddd.Domain.ValidationException vex => (
            vex.StatusCode,
            vex.Code,
            vex.Message,
            vex.Errors.Select(e => new ErrorDetailDto
            {
                Code = "VALIDATION_ERROR",
                Message = e.Message,
                Target = e.Field
            }).ToList()),

        BusinessException bex => (
            bex.StatusCode,
            bex.Code,
            localizer?.Localize(bex) ?? bex.Message,
            []),

        EntityNotFoundException enf => (
            404,
            "ENTITY_NOT_FOUND",
            enf.Message,
            []),

        // ── Auth ─────────────────────────────────────────────────────
        UnauthorizedAccessException uae => (
            401,
            "UNAUTHORIZED",
            uae.Message,
            []),

        // ── Concurrency (EF Core DbUpdateConcurrencyException, etc.) ─
        Exception ex when ex.GetType().Name == "DbUpdateConcurrencyException" => (
            409,
            "CONCURRENCY_CONFLICT",
            "The record was modified or deleted by another request. Please reload and try again.",
            []),

        // ── Bad input ────────────────────────────────────────────────
        ArgumentException ae => (
            400,
            "BAD_REQUEST",
            ae.Message,
            ae.ParamName is not null
                ? [new ErrorDetailDto { Code = "INVALID_ARGUMENT", Message = ae.Message, Target = ae.ParamName }]
                : []),

        FormatException fe => (
            400,
            "BAD_FORMAT",
            fe.Message,
            []),

        InvalidOperationException ioe => (
            400,
            "INVALID_OPERATION",
            ioe.Message,
            []),

        // ── Cancellation (client disconnected) ──────────────────────
        OperationCanceledException => (
            499,
            "CLIENT_CLOSED",
            "The request was cancelled.",
            []),

        // ── Catch-all ────────────────────────────────────────────────
        _ => (
            500,
            "INTERNAL_SERVER_ERROR",
            "An unexpected error occurred. Please try again later.",
            [])
    };
}
