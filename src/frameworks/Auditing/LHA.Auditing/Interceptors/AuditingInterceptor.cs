using Castle.DynamicProxy;
using System.Diagnostics;
using System.Reflection;
using System.Text.Json;

namespace LHA.Auditing.Interceptors;

/// <summary>
/// Intercepts calls to application services to record as audit log actions.
/// Implements IInterceptor to be compatible with Castle ProxyGenerator.
/// </summary>
public sealed class AuditingInterceptor(IAuditingManager auditingManager) : IInterceptor
{
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
        // Use the declaring type's name (the interface or the class)
        var serviceName = invocation.Method.DeclaringType?.Name ?? invocation.TargetType?.Name ?? "Unknown";

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

            var dict = parameters
                .Select((p, i) => new { p.Name, Value = parameterValues[i] })
                .ToDictionary(x => x.Name ?? "arg", x => x.Value);

            return JsonSerializer.Serialize(dict, JsonOptions);
        }
        catch
        {
            return "{\"error\":\"Serialization failed\"}";
        }
    }
}
