using LHA.Ddd.Application;
using LHA.Notification.Application.Contracts;
using LHA.Notification.Domain;
using LHA.Notification.Domain.Repositories;
using LHA.Notification.Domain.Shared;
using LHA.Notification.Domain.ValueObjects;
using LHA.UnitOfWork;

namespace LHA.Notification.Application.Templates;

public sealed class TemplateAppService : ApplicationService, ITemplateService
{
    private readonly ITemplateRepository _templateRepository;
    private readonly IUnitOfWorkManager _uowManager;

    public TemplateAppService(
        ITemplateRepository templateRepository,
        IUnitOfWorkManager uowManager)
    {
        _templateRepository = templateRepository;
        _uowManager = uowManager;
    }

    public async Task<TemplateDto> CreateAsync(CreateTemplateDto request, Guid tenantId, CancellationToken cancellationToken = default)
    {
        await using var uow = _uowManager.Begin(new UnitOfWorkOptions { IsTransactional = true });

        var entity = new TemplateEntity(
            code: request.Code,
            name: request.Name,
            description: request.Description,
            type: request.Type,
            defaultLocale: request.DefaultLocale);

        entity.UpdateSupportedChannels(request.SupportedChannels);

        if (request.Tags != null)
            entity.UpdateTags(request.Tags);

        await _templateRepository.InsertAsync(entity, cancellationToken);
        await uow.CompleteAsync();

        return MapToDto(entity);
    }

    public async Task<TemplateDto> UpdateAsync(Guid id, UpdateTemplateDto request, Guid tenantId, CancellationToken cancellationToken = default)
    {
        await using var uow = _uowManager.Begin(new UnitOfWorkOptions { IsTransactional = true });

        var entity = await _templateRepository.GetAsync(id, cancellationToken);

        if (request.Name is not null)
            entity.UpdateName(request.Name);

        if (request.Description is not null)
            entity.UpdateDescription(request.Description);

        if (request.SupportedChannels is not null)
            entity.UpdateSupportedChannels(request.SupportedChannels);

        if (request.DefaultLocale is not null)
            entity.UpdateDefaultLocale(request.DefaultLocale);

        if (request.Tags is not null)
            entity.UpdateTags(request.Tags);

        if (request.IsActive.HasValue)
        {
            if (request.IsActive.Value) entity.Activate();
            else entity.Deactivate();
        }

        await _templateRepository.UpdateAsync(entity, cancellationToken);
        await uow.CompleteAsync();

        return MapToDto(entity);
    }

    public async Task<TemplateDto?> DeleteAsync(Guid id, Guid tenantId, CancellationToken cancellationToken = default)
    {
        await using var uow = _uowManager.Begin(new UnitOfWorkOptions { IsTransactional = true });

        var entity = await _templateRepository.FindAsync(id, cancellationToken);
        if (entity is null) return null;

        await _templateRepository.DeleteAsync(id, cancellationToken);
        await uow.CompleteAsync();

        return MapToDto(entity);
    }

    public async Task<TemplateDto?> GetByIdAsync(Guid id, Guid tenantId, CancellationToken cancellationToken = default)
    {
        var entity = await _templateRepository.FindAsync(id, cancellationToken);
        return entity is null ? null : MapToDto(entity);
    }

    public async Task<TemplateDto?> GetByCodeAsync(string code, Guid tenantId, CancellationToken cancellationToken = default)
    {
        var entity = await _templateRepository.GetByCodeAsync(code, cancellationToken);
        return entity is null ? null : MapToDto(entity);
    }

    public async Task<List<TemplateDto>> GetByTypeAsync(CNotificationType type, Guid tenantId, CancellationToken cancellationToken = default)
    {
        var entities = await _templateRepository.GetByTypeAsync(type, cancellationToken);
        return entities.Select(MapToDto).ToList();
    }

    public async Task<TemplateRenderResultDto> PreviewAsync(TemplatePreviewDto request, Guid tenantId, CancellationToken cancellationToken = default)
    {
        var entity = await _templateRepository.GetByCodeAsync(request.Code, cancellationToken);
        if (entity is null)
            throw new InvalidOperationException($"Template '{request.Code}' not found.");

        var version = entity.Versions.FirstOrDefault(v =>
            v.Locale == (request.Locale ?? entity.DefaultLocale) &&
            v.Channel == request.Channel);

        if (version is null)
            throw new InvalidOperationException($"No version for locale '{request.Locale}' and channel '{request.Channel}'.");

        var body = RenderTemplate(version.BodyTemplate, request.Variables);
        var subject = version.SubjectTemplate is not null ? RenderTemplate(version.SubjectTemplate, request.Variables) : null;
        var html = version.HtmlTemplate is not null ? RenderTemplate(version.HtmlTemplate, request.Variables) : null;

        return new TemplateRenderResultDto(
            TemplateId: entity.Id,
            Code: entity.Code,
            Type: entity.Type,
            Subject: subject,
            Body: body,
            HtmlBody: html,
            PushTitle: version.PushTitle,
            PushBody: version.PushBody,
            EngineType: CTemplateEngineType.Handlebars // Assuming default engine
        );
    }

    public Task<TemplateRenderResultDto> RenderAsync(TemplatePreviewDto request, Guid tenantId, CancellationToken cancellationToken = default)
        => PreviewAsync(request, tenantId, cancellationToken);

    public async Task<TemplateVersionDto> AddVersionAsync(
        Guid id, string version, CNotificationChannel channel, string locale,
        string bodyTemplate, string? subjectTemplate, string? htmlTemplate,
        string? pushTitle, string? pushBody, CancellationToken cancellationToken = default)
    {
        await using var uow = _uowManager.Begin(new UnitOfWorkOptions { IsTransactional = true });

        var entity = await _templateRepository.GetAsync(id, cancellationToken);

        if (!int.TryParse(version, out var versionNumber))
            versionNumber = entity.Versions.Count + 1;

        var versionEntity = new TemplateVersionEntity(versionNumber, locale, channel, bodyTemplate);
        versionEntity.SetSubjectTemplate(subjectTemplate);
        versionEntity.SetHtmlTemplate(htmlTemplate);
        versionEntity.SetPushTitle(pushTitle);
        versionEntity.SetPushBody(pushBody);

        entity.AddVersion(versionEntity);
        await _templateRepository.UpdateAsync(entity, cancellationToken);
        await uow.CompleteAsync();

        return MapVersionToDto(versionEntity);
    }

    public async Task<TemplateDto> ActivateAsync(Guid id, Guid tenantId, CancellationToken cancellationToken = default)
    {
        await using var uow = _uowManager.Begin(new UnitOfWorkOptions { IsTransactional = true });

        var entity = await _templateRepository.GetAsync(id, cancellationToken);
        entity.Activate();
        await _templateRepository.UpdateAsync(entity, cancellationToken);
        await uow.CompleteAsync();

        return MapToDto(entity);
    }

    public async Task<TemplateDto> DeactivateAsync(Guid id, Guid tenantId, CancellationToken cancellationToken = default)
    {
        await using var uow = _uowManager.Begin(new UnitOfWorkOptions { IsTransactional = true });

        var entity = await _templateRepository.GetAsync(id, cancellationToken);
        entity.Deactivate();
        await _templateRepository.UpdateAsync(entity, cancellationToken);
        await uow.CompleteAsync();

        return MapToDto(entity);
    }

    public async Task<List<TemplateDto>> SearchAsync(string query, Guid tenantId, CancellationToken cancellationToken = default)
    {
        var entities = await _templateRepository.SearchAsync(query, cancellationToken);
        return entities.Select(MapToDto).ToList();
    }

    // ─── Helpers ─────────────────────────────────────────────────────

    private static string RenderTemplate(string template, Dictionary<string, object>? variables)
    {
        if (variables is null) return template;
        foreach (var kv in variables)
            template = template.Replace($"{{{{{kv.Key}}}}}", kv.Value?.ToString() ?? string.Empty);
        return template;
    }

    // ─── Mapping ─────────────────────────────────────────────────────

    private static TemplateDto MapToDto(TemplateEntity e) => new(
        Id: e.Id,
        TenantId: e.TenantId ?? Guid.Empty,
        Code: e.Code,
        Name: e.Name,
        Description: e.Description,
        Type: e.Type,
        SupportedChannels: e.SupportedChannels,
        IsActive: e.IsActive,
        IsSystem: e.IsSystem,
        DefaultLocale: e.DefaultLocale,
        Tags: e.Tags,
        CreatedAt: e.CreationTime,
        UpdatedAt: e.LastModificationTime ?? e.CreationTime,
        Versions: e.Versions.Select(MapVersionToDto).ToList());

    private static TemplateVersionDto MapVersionToDto(TemplateVersionEntity v) => new(
        Version: v.Version,
        Locale: v.Locale,
        Channel: v.Channel,
        SubjectTemplate: v.SubjectTemplate,
        BodyTemplate: v.BodyTemplate,
        HtmlTemplate: v.HtmlTemplate,
        PushTitle: v.PushTitle,
        PushBody: v.PushBody,
        Variables: v.Variables.Select(x => new TemplateVariableDefinitionDto(
            x.Name,
            x.Type,
            x.Required,
            x.DefaultValue,
            x.Description)).ToList(),
        IsDefault: v.IsDefault,
        CreatedAt: v.CreatedAt);
}
