namespace LHA.Notification.Domain.Shared;

public static class ChannelConstants
{
    public const int MaxConcurrentPerChannel = 50;
    public const int CircuitBreakerThreshold = 5;
    public const int CircuitBreakerTimeoutSeconds = 30;
    public const int CircuitBreakerRecoverySeconds = 60;
    public const int SmtpPoolSize = 10;
    public const int SmtpPoolIdleTimeoutMinutes = 5;
}