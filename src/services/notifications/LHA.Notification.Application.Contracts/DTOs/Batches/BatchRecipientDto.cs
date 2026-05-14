using LHA.Notification.Domain.Shared;

namespace LHA.Notification.Application.Contracts;

public record BatchRecipientDto(
    Guid RecipientId,
    CRecipientType RecipientType,
    Dictionary<string, object>? Variables);