using LHA.Notification.Domain.Shared;

namespace LHA.Notification.Application.Contracts;

public record DeliveryRateDto(
    CDeliveryStatus Status,
    long Count,
    double Percentage);