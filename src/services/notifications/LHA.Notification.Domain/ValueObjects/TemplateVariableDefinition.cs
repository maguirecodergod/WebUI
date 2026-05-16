using LHA.Notification.Domain.Shared;

namespace LHA.Notification.Domain.ValueObjects;

public sealed class TemplateVariableDefinition
{
    public string Name { get; private set; } = default!;
    public CVariableType Type { get; private set; }
    public bool Required { get; private set; }
    public string? DefaultValue { get; private set; }
    public string Description { get; private set; } = default!;

    public TemplateVariableDefinition(
        string name,
        CVariableType type,
        bool required,
        string? defaultValue = null,
        string description = "")
    {
        Name = name;
        Type = type;
        Required = required;
        DefaultValue = defaultValue;
        Description = description;
    }

    public void SetDefaultValue(string? defaultValue)
    {
        DefaultValue = defaultValue;
    }

    public void SetDescription(string description)
    {
        Description = description;
    }
}
