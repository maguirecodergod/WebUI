using LHA.Ddd.Application;
using LHA.Notification.Application.Contracts;
using LHA.Notification.Domain;
using LHA.Notification.Domain.Repositories;
using LHA.Notification.Domain.Shared;
using LHA.UnitOfWork;

namespace LHA.Notification.Application.Notifications;

public sealed class NotificationAppService : ApplicationService, INotificationService
{
    private readonly INotificationRepository _notificationRepository;
    private readonly IUnitOfWorkManager _uowManager;
    private readonly IUnreadCountCache _unreadCountCache;

    public NotificationAppService(
        INotificationRepository notificationRepository,
        IUnitOfWorkManager uowManager,
        IUnreadCountCache unreadCountCache)
    {
        _notificationRepository = notificationRepository;
        _uowManager = uowManager;
        _unreadCountCache = unreadCountCache;
    }

    public async Task<NotificationDto> SendAsync(SendNotificationDto request, CancellationToken cancellationToken = default)
    {
        await using var uow = _uowManager.Begin(new UnitOfWorkOptions { IsTransactional = true });

        var entity = new NotificationEntity(
            correlationId: Guid.CreateVersion7().ToString(),
            recipientId: request.RecipientId,
            recipientType: request.RecipientType,
            type: request.Type,
            priority: request.Priority,
            subject: request.Subject,
            body: request.Body,
            actionUrl: request.ActionUrl,
            templateId: request.TemplateId,
            templateVariables: request.TemplateVariables,
            data: request.Data);

        if (request.ExpiresAt.HasValue)
            entity.SetExpiresAt(request.ExpiresAt.Value);

        if (request.Tags != null)
            foreach (var tag in request.Tags)
                entity.AddTag(tag);

        await _notificationRepository.InsertAsync(entity, cancellationToken);
        await _unreadCountCache.IncrementUnreadCountAsync(entity.TenantId ?? Guid.Empty, entity.RecipientId, cancellationToken);
        await uow.CompleteAsync();

        return MapToDto(entity);
    }

    public async Task<NotificationDto> ScheduleAsync(ScheduleNotificationDto request, CancellationToken cancellationToken = default)
    {
        await using var uow = _uowManager.Begin(new UnitOfWorkOptions { IsTransactional = true });

        var entity = new NotificationEntity(
            correlationId: Guid.CreateVersion7().ToString(),
            recipientId: request.RecipientId,
            recipientType: request.RecipientType,
            type: request.Type,
            priority: request.Priority,
            subject: request.Subject,
            body: request.Body,
            actionUrl: request.ActionUrl,
            templateId: request.TemplateId,
            templateVariables: request.TemplateVariables,
            data: request.Data);

        entity.ScheduleFor(request.ScheduledAt);

        if (request.ExpiresAt.HasValue)
            entity.SetExpiresAt(request.ExpiresAt.Value);

        if (request.Tags != null)
            foreach (var tag in request.Tags)
                entity.AddTag(tag);

        await _notificationRepository.InsertAsync(entity, cancellationToken);
        await uow.CompleteAsync();

        return MapToDto(entity);
    }

    public async Task<NotificationPagedResultDto<NotificationDto>> GetByRecipientAsync(Guid recipientId, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var items = new List<NotificationDto>();
        var total = 0;
        var skip = (page - 1) * pageSize;
        var index = 0;

        await foreach (var entity in _notificationRepository.GetByRecipientCursorAsync(recipientId, pageSize, cancellationToken))
        {
            if (index >= skip && items.Count < pageSize)
                items.Add(MapToDto(entity));
            index++;
            total++;
        }

        return new NotificationPagedResultDto<NotificationDto>(
            items, total, page, pageSize, (page * pageSize) < total);
    }

    public async Task<NotificationDto?> GetByIdAsync(Guid id, Guid tenantId, CancellationToken cancellationToken = default)
    {
        var entity = await _notificationRepository.FindAsync(id, cancellationToken);
        return entity is null ? null : MapToDto(entity);
    }

    public async Task<NotificationDto?> MarkAsReadAsync(Guid id, Guid tenantId, CancellationToken cancellationToken = default)
    {
        await using var uow = _uowManager.Begin(new UnitOfWorkOptions { IsTransactional = true });

        var entity = await _notificationRepository.FindAsync(id, cancellationToken);
        if (entity is null) return null;

        entity.MarkAsRead(DateTimeOffset.UtcNow);
        await _notificationRepository.UpdateAsync(entity, cancellationToken);
        await _unreadCountCache.DecrementUnreadCountAsync(entity.TenantId ?? Guid.Empty, entity.RecipientId, cancellationToken);
        await uow.CompleteAsync();

        return MapToDto(entity);
    }

    public async Task<NotificationDto?> DeleteAsync(Guid id, Guid tenantId, CancellationToken cancellationToken = default)
    {
        await using var uow = _uowManager.Begin(new UnitOfWorkOptions { IsTransactional = true });

        var entity = await _notificationRepository.FindAsync(id, cancellationToken);
        if (entity is null) return null;

        await _notificationRepository.DeleteAsync(id, cancellationToken);
        await uow.CompleteAsync();

        return MapToDto(entity);
    }

    public async Task<int> GetUnreadCountAsync(Guid recipientId, CancellationToken cancellationToken = default)
    {
        return await _notificationRepository.GetUnreadCountByRecipientAsync(recipientId, cancellationToken);
    }

    public async Task<bool> CancelAsync(Guid id, Guid tenantId, CancellationToken cancellationToken = default)
    {
        await using var uow = _uowManager.Begin(new UnitOfWorkOptions { IsTransactional = true });

        var entity = await _notificationRepository.FindAsync(id, cancellationToken);
        if (entity is null) return false;

        entity.Cancel();
        await _notificationRepository.UpdateAsync(entity, cancellationToken);
        await uow.CompleteAsync();

        return true;
    }

    public async Task<NotificationPagedResultDto<NotificationSummaryDto>> SearchAsync(
        Guid recipientId, string? query, CNotificationType? type, CDeliveryStatus? status,
        DateTimeOffset? from, DateTimeOffset? to, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var items = new List<NotificationSummaryDto>();
        var total = 0;
        var skip = (page - 1) * pageSize;
        var index = 0;

        await foreach (var entity in _notificationRepository.GetByRecipientCursorAsync(recipientId, pageSize * 10, cancellationToken))
        {
            if (type.HasValue && entity.Type != type.Value) continue;
            if (status.HasValue && entity.Status != status.Value) continue;
            if (from.HasValue && entity.CreationTime < from.Value) continue;
            if (to.HasValue && entity.CreationTime > to.Value) continue;
            if (!string.IsNullOrWhiteSpace(query) &&
                !entity.Body.Contains(query, StringComparison.OrdinalIgnoreCase) &&
                !(entity.Subject?.Contains(query, StringComparison.OrdinalIgnoreCase) ?? false))
                continue;

            if (index >= skip && items.Count < pageSize)
                items.Add(MapToSummaryDto(entity));

            index++;
            total++;
        }

        return new NotificationPagedResultDto<NotificationSummaryDto>(
            items, total, page, pageSize, (page * pageSize) < total);
    }

    public async Task<int> GetUnreadCountByTenantAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        return await _notificationRepository.GetUnreadCountByTenantAsync(cancellationToken);
    }

    public async Task<int> GetUnreadCountByUserAsync(Guid userId, Guid tenantId, CancellationToken cancellationToken = default)
    {
        return await _unreadCountCache.GetUnreadCountAsync(tenantId, userId, cancellationToken);
    }

    private static NotificationDto MapToDto(NotificationEntity e) => new(
        Id: e.Id,
        TenantId: e.TenantId ?? Guid.Empty,
        CorrelationId: Guid.TryParse(e.CorrelationId, out var cid) ? cid : Guid.Empty,
        BatchId: e.BatchId,
        RecipientId: e.RecipientId,
        RecipientType: e.RecipientType,
        Type: e.Type,
        Priority: e.Priority,
        Status: e.Status,
        Subject: e.Subject,
        Body: e.Body,
        Data: e.Data,
        ImageUrl: e.ImageUrl,
        ActionUrl: e.ActionUrl,
        TemplateId: e.TemplateId?.ToString(),
        TemplateVariables: e.TemplateVariables,
        ScheduledAt: e.ScheduledAt,
        ExpiresAt: e.ExpiresAt,
        SentAt: e.SentAt,
        DeliveredAt: e.DeliveredAt,
        ReadAt: e.ReadAt,
        FailedAt: e.FailedAt,
        RetryCount: e.RetryCount,
        MaxRetries: e.MaxRetries,
        Channels: e.Channels.Select(c => new NotificationChannelDto(
            c.Channel,
            c.Status,
            c.ProviderType,
            c.ExternalId,
            c.SentAt,
            c.DeliveredAt,
            c.FailedAt,
            c.FailureReason,
            c.RetryCount,
            c.Metadata)).ToList(),
        Tags: e.Tags,
        CreatedAt: e.CreationTime,
        UpdatedAt: e.LastModificationTime ?? e.CreationTime);

    private static NotificationSummaryDto MapToSummaryDto(NotificationEntity e) => new(
        Id: e.Id,
        RecipientId: e.RecipientId,
        RecipientType: e.RecipientType,
        Type: e.Type,
        Priority: e.Priority,
        Status: e.Status,
        Subject: e.Subject,
        CreatedAt: e.CreationTime);
}
