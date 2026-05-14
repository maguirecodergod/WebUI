using LHA.Ddd.Application;
using LHA.Notification.Application.Contracts;
using LHA.Notification.Domain;
using LHA.Notification.Domain.Repositories;
using LHA.Notification.Domain.Shared;
using LHA.Notification.Domain.ValueObjects;
using LHA.UnitOfWork;

namespace LHA.Notification.Application.Preferences;

/// <summary>
/// Application service for user notification preferences management.
/// </summary>
public sealed class UserPreferenceAppService : ApplicationService, IUserPreferenceService
{
    private readonly IUserPreferenceRepository _preferenceRepository;
    private readonly IUnitOfWorkManager _uowManager;

    public UserPreferenceAppService(
        IUserPreferenceRepository preferenceRepository,
        IUnitOfWorkManager uowManager)
    {
        _preferenceRepository = preferenceRepository;
        _uowManager = uowManager;
    }

    /// <inheritdoc />
    public async Task<UserPreferenceDto> GetAsync(Guid userId, Guid tenantId, CancellationToken cancellationToken = default)
    {
        var entity = await _preferenceRepository.GetByUserIdAsync(userId, cancellationToken);

        if (entity is null)
        {
            await using var uow = _uowManager.Begin(new UnitOfWorkOptions { IsTransactional = true });
            entity = new UserPreferenceEntity(userId);
            await _preferenceRepository.InsertAsync(entity, cancellationToken);
            await uow.CompleteAsync();
        }

        return MapToDto(entity);
    }

    /// <inheritdoc />
    public async Task<UserPreferenceDto> UpdateAsync(Guid userId, UpdatePreferenceDto request, Guid tenantId, CancellationToken cancellationToken = default)
    {
        await using var uow = _uowManager.Begin(new UnitOfWorkOptions { IsTransactional = true });

        var entity = await _preferenceRepository.GetByUserIdAsync(userId, cancellationToken);
        
        if (entity is null)
        {
            entity = new UserPreferenceEntity(userId);
        }

        if (request.GlobalOptOut.HasValue)
            entity.OptOut(request.GlobalOptOut.Value);

        if (request.Channels is not null)
        {
            foreach (var ch in request.Channels)
            {
                entity.SetChannelPreference(ch.Channel, ch.Enabled, ch.Categories);
            }
        }

        if (request.Categories is not null)
        {
            foreach (var cat in request.Categories)
            {
                entity.SetCategoryPreference(cat.Category, cat.Enabled, cat.Channels);
            }
        }

        if (request.QuietHours is not null)
        {
            if (TimeOnly.TryParse(request.QuietHours.StartTime, out var startTime) && 
                TimeOnly.TryParse(request.QuietHours.EndTime, out var endTime))
            {
                entity.SetQuietHours(new QuietHoursSettings(
                    request.QuietHours.Enabled,
                    startTime,
                    endTime,
                    request.QuietHours.Timezone));
            }
        }

        if (!string.IsNullOrWhiteSpace(request.Timezone))
            entity.SetTimezone(request.Timezone);

        if (!string.IsNullOrWhiteSpace(request.Locale))
            entity.SetLocale(request.Locale);

        if (entity.Id == Guid.Empty)
            await _preferenceRepository.InsertAsync(entity, cancellationToken);
        else
            await _preferenceRepository.UpdateAsync(entity, cancellationToken);

        await uow.CompleteAsync();

        return MapToDto(entity);
    }

    /// <inheritdoc />
    public async Task<bool> OptOutAsync(Guid userId, Guid tenantId, CancellationToken cancellationToken = default)
    {
        await using var uow = _uowManager.Begin(new UnitOfWorkOptions { IsTransactional = true });

        var entity = await _preferenceRepository.GetByUserIdAsync(userId, cancellationToken)
                     ?? new UserPreferenceEntity(userId);

        entity.OptOut(true);

        if (entity.Id == Guid.Empty)
            await _preferenceRepository.InsertAsync(entity, cancellationToken);
        else
            await _preferenceRepository.UpdateAsync(entity, cancellationToken);

        await uow.CompleteAsync();
        return true;
    }

    /// <inheritdoc />
    public async Task<bool> OptInAsync(Guid userId, Guid tenantId, CancellationToken cancellationToken = default)
    {
        await using var uow = _uowManager.Begin(new UnitOfWorkOptions { IsTransactional = true });

        var entity = await _preferenceRepository.GetByUserIdAsync(userId, cancellationToken)
                     ?? new UserPreferenceEntity(userId);

        entity.OptOut(false);

        if (entity.Id == Guid.Empty)
            await _preferenceRepository.InsertAsync(entity, cancellationToken);
        else
            await _preferenceRepository.UpdateAsync(entity, cancellationToken);

        await uow.CompleteAsync();
        return true;
    }

    /// <inheritdoc />
    public async Task<bool> IsChannelEnabledAsync(Guid userId, Guid tenantId, string channel, CancellationToken cancellationToken = default)
    {
        if (!Enum.TryParse<CNotificationChannel>(channel, true, out var channelEnum))
            return false;

        return await _preferenceRepository.IsChannelEnabledAsync(userId, channelEnum, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> IsCategoryEnabledAsync(Guid userId, Guid tenantId, string category, CancellationToken cancellationToken = default)
    {
        var entity = await _preferenceRepository.GetByUserIdAsync(userId, cancellationToken);
        if (entity is null) return true; // Default enabled
        if (entity.GlobalOptOut) return false;

        var cat = entity.Categories.FirstOrDefault(c => c.Category == category);
        return cat?.Enabled ?? true;
    }

    /// <inheritdoc />
    public async Task<List<CNotificationChannel>> GetEnabledChannelsAsync(Guid userId, Guid tenantId, CancellationToken cancellationToken = default)
    {
        var channels = await _preferenceRepository.GetEnabledChannelsByUserAsync(userId, cancellationToken);
        return channels.ToList();
    }

    /// <inheritdoc />
    public async Task<bool> IsQuietHourAsync(Guid userId, Guid tenantId, CancellationToken cancellationToken = default)
    {
        var entity = await _preferenceRepository.GetByUserIdAsync(userId, cancellationToken);
        if (entity?.QuietHours is null || !entity.QuietHours.Enabled) return false;

        var now = TimeOnly.FromDateTime(DateTime.UtcNow);
        return entity.QuietHours.IsQuietHour(now, entity.Timezone);
    }

    // ─── Mapping ─────────────────────────────────────────────────────

    private static UserPreferenceDto MapToDto(UserPreferenceEntity e) => new(
        Id: e.Id,
        TenantId: e.TenantId ?? Guid.Empty,
        UserId: e.UserId,
        GlobalOptOut: e.GlobalOptOut,
        Channels: e.Channels.Select(c => new ChannelPreferenceDto(c.Channel, c.Enabled, c.Categories)).ToList(),
        Categories: e.Categories.Select(c => new CategoryPreferenceDto(c.Category, c.Enabled, c.Channels)).ToList(),
        QuietHours: e.QuietHours is null ? null : new QuietHoursDto(
            e.QuietHours.Enabled,
            e.QuietHours.StartTime.ToString("HH:mm"),
            e.QuietHours.EndTime.ToString("HH:mm"),
            e.QuietHours.Timezone),
        Timezone: e.Timezone,
        Locale: e.Locale,
        UpdatedAt: e.LastModificationTime ?? e.CreationTime);
}
