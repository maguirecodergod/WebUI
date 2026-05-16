using LHA.Notification.Domain.Shared;

namespace LHA.Notification.Application.Contracts;

public record ChannelStatsDto(
    CNotificationChannel Channel,
    long TotalSent,
    long TotalDelivered,
    long TotalRead,
    long TotalFailed,
    double DeliveryRate,
    double ReadRate);
