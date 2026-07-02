namespace LHA.AspNetCore.Security;

public sealed class SecurityVersioningOptions
{
    public const string SectionName = "SecurityVersioning";

    public string RedisConfiguration { get; set; } = "localhost:6379";

    public TimeSpan L1CacheTtl { get; set; } = TimeSpan.FromSeconds(10);

    public int CircuitBreakerFailureThreshold { get; set; } = 3;

    public TimeSpan CircuitBreakerBreakDuration { get; set; } = TimeSpan.FromSeconds(30);
}

