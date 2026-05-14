namespace LHA.Notification.Application.Contracts;

public record NotificationStatsDto(
    Guid TenantId,
    DateTimeOffset PeriodStart,
    DateTimeOffset PeriodEnd,
    long TotalSent,
    long TotalDelivered,
    long TotalRead,
    long TotalFailed,
    long TotalCancelled,
    double DeliveryRate,
    double ReadRate,
    double FailureRate);