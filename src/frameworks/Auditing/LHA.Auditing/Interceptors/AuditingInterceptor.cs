using Castle.DynamicProxy;
using System.Diagnostics;
using System.Reflection;
using System.Text.Json;

namespace LHA.Auditing.Interceptors;

/// <summary>
/// Intercepts calls to application services to record as audit log actions.
/// Implements IInterceptor to be compatible with Castle ProxyGenerator.
/// </summary>
public sealed class AuditingInterceptor(
    IAuditingManager auditingManager,
    Microsoft.Extensions.Options.IOptions<AuditingOptions> options) : IInterceptor
{
    private readonly AuditingOptions _options = options.Value;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    public void Intercept(IInvocation invocation)
    {
        if (!ShouldAudit(invocation))
        {
            invocation.Proceed();
            return;
        }

        var (action, sw) = PreProcess(invocation);
        try
        {
            invocation.Proceed();

            var returnType = invocation.Method.ReturnType;
            if (returnType == typeof(Task) && invocation.ReturnValue != null)
            {
                invocation.ReturnValue = HandleAsync((Task)invocation.ReturnValue, action, sw);
            }
            else if (returnType.IsGenericType && returnType.GetGenericTypeDefinition() == typeof(Task<>) && invocation.ReturnValue != null)
            {
                var resultType = returnType.GetGenericArguments()[0];
                var method = typeof(AuditingInterceptor)
                    .GetMethod(nameof(HandleAsyncGeneric), BindingFlags.NonPublic | BindingFlags.Instance)
                    ?.MakeGenericMethod(resultType);

                invocation.ReturnValue = method?.Invoke(this, [invocation.ReturnValue, action, sw]);
            }
            else
            {
                // Synchronous or unknown (ValueTask etc)
                PostProcess(action, sw);
            }
        }
        catch (Exception)
        {
            action.ExecutionDuration = (int)sw.ElapsedMilliseconds;
            throw;
        }
    }

    private async Task HandleAsync(Task task, AuditLogAction action, Stopwatch sw)
    {
        try
        {
            await task;
        }
        finally
        {
            PostProcess(action, sw);
        }
    }

    private async Task<T> HandleAsyncGeneric<T>(Task<T> task, AuditLogAction action, Stopwatch sw)
    {
        try
        {
            return await task;
        }
        finally
        {
            PostProcess(action, sw);
        }
    }

    private bool ShouldAudit(IInvocation invocation)
    {
        return auditingManager.Current != null;
    }

    private (AuditLogAction action, Stopwatch sw) PreProcess(IInvocation invocation)
    {
        var methodName = invocation.Method.Name;
        // TargetType = the concrete class (e.g. AuthAppService), NOT the proxy or interface
        var serviceName = invocation.TargetType?.Name
                       ?? invocation.Method.DeclaringType?.Name
                       ?? "Unknown";

        var action = new AuditLogAction
        {
            ServiceName = serviceName,
            MethodName = methodName,
            ExecutionTime = DateTimeOffset.UtcNow,
            Parameters = SerializeParameters(invocation)
        };

        auditingManager.Current?.Log.Actions.Add(action);

        return (action, Stopwatch.StartNew());
    }

    private void PostProcess(AuditLogAction action, Stopwatch sw)
    {
        if (sw.IsRunning) sw.Stop();
        action.ExecutionDuration = (int)sw.ElapsedMilliseconds;
    }

    private string? SerializeParameters(IInvocation invocation)
    {
        try
        {
            var parameters = invocation.Method.GetParameters();
            var parameterValues = invocation.Arguments;
            if (parameters.Length == 0) return null;

            var dict = new Dictionary<string, object?>();
            for (var i = 0; i < parameters.Length; i++)
            {
                var p = parameters[i];
                var value = parameterValues[i];

                // Skip unserializable / sensitive types
                if (value is null
                    || value is CancellationToken
                    || value is System.IO.Stream
                    || value is System.Linq.Expressions.Expression)
                {
                    continue;
                }

                dict[p.Name ?? $"arg{i}"] = value;
            }

            var json = dict.Count == 0 ? null : JsonSerializer.Serialize(dict, JsonOptions);

            // Mask sensitive fields before storing
            if (json is not null && _options.SensitivePropertyNames.Count > 0)
                json = SensitiveDataMasker.MaskJson(json, _options.SensitivePropertyNames);

            return json;
        }
        catch (Exception ex)
        {
            return $"{{\"error\":\"Serialization failed: {ex.Message.Replace("\"", "'")}\"}}"; 
        }
    }
}
