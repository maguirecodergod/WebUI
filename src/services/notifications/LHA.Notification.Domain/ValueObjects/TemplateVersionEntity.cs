using LHA.Notification.Domain.Shared;

namespace LHA.Notification.Domain.ValueObjects;

public sealed class TemplateVersionEntity
{
    public int Version { get; private set; }
    public string Locale { get; private set; } = default!;
    public CNotificationChannel Channel { get; private set; }
    public string? SubjectTemplate { get; private set; }
    public string BodyTemplate { get; private set; } = default!;
    public string? HtmlTemplate { get; private set; }
    public string? PushTitle { get; private set; }
    public string? PushBody { get; private set; }
    public List<TemplateVariableDefinition> Variables { get; private set; } = new();
    public bool IsDefault { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }

    public TemplateVersionEntity(
        int version,
        string locale,
        CNotificationChannel channel,
        string bodyTemplate)
    {
        Version = version;
        Locale = locale;
        Channel = channel;
        BodyTemplate = bodyTemplate;
        CreatedAt = DateTimeOffset.UtcNow;
    }

    public void SetSubjectTemplate(string? subjectTemplate)
    {
        SubjectTemplate = subjectTemplate;
    }

    public void SetHtmlTemplate(string? htmlTemplate)
    {
        HtmlTemplate = htmlTemplate;
    }

    public void SetPushTitle(string? pushTitle)
    {
        PushTitle = pushTitle;
    }

    public void SetPushBody(string? pushBody)
    {
        PushBody = pushBody;
    }

    public void SetDefault(bool isDefault)
    {
        IsDefault = isDefault;
    }

    public void AddVariable(TemplateVariableDefinition variable)
    {
        if (!Variables.Any(v => v.Name == variable.Name))
        {
            Variables.Add(variable);
        }
    }
}
