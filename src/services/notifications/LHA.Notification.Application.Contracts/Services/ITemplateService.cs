using LHA.Notification.Domain.Shared;

namespace LHA.Notification.Application.Contracts;

public interface ITemplateService
{
    Task<TemplateDto> CreateAsync(CreateTemplateDto request, Guid tenantId, CancellationToken cancellationToken = default);
    Task<TemplateDto> UpdateAsync(Guid id, UpdateTemplateDto request, Guid tenantId, CancellationToken cancellationToken = default);
    Task<TemplateDto?> DeleteAsync(Guid id, Guid tenantId, CancellationToken cancellationToken = default);
    Task<TemplateDto?> GetByIdAsync(Guid id, Guid tenantId, CancellationToken cancellationToken = default);
    Task<TemplateDto?> GetByCodeAsync(string code, Guid tenantId, CancellationToken cancellationToken = default);
    Task<List<TemplateDto>> GetByTypeAsync(CNotificationType type, Guid tenantId, CancellationToken cancellationToken = default);
    Task<TemplateRenderResultDto> PreviewAsync(TemplatePreviewDto request, Guid tenantId, CancellationToken cancellationToken = default);
    Task<TemplateRenderResultDto> RenderAsync(TemplatePreviewDto request, Guid tenantId, CancellationToken cancellationToken = default);
    Task<TemplateVersionDto> AddVersionAsync(Guid id, string version, CNotificationChannel channel, string locale, string bodyTemplate, string? subjectTemplate, string? htmlTemplate, string? pushTitle, string? pushBody, CancellationToken cancellationToken = default);
    Task<TemplateDto> ActivateAsync(Guid id, Guid tenantId, CancellationToken cancellationToken = default);
    Task<TemplateDto> DeactivateAsync(Guid id, Guid tenantId, CancellationToken cancellationToken = default);
    Task<List<TemplateDto>> SearchAsync(string query, Guid tenantId, CancellationToken cancellationToken = default);
}