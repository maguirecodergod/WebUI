namespace LHA.BlazorWasm.Modules.Host.Roles.Models;

public class PermissionGroupViewModel
{
    public string Name { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string ServiceName { get; set; } = string.Empty;
    public List<PermissionDefinitionViewModel> Permissions { get; set; } = [];
}

public class PermissionDefinitionViewModel
{
    public string Name { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public bool IsGranted { get; set; }
    public bool IsInherited { get; set; }
    public bool IsDirectOverride { get; set; }
    public bool IsFromTemplate { get; set; }
    public List<string> InheritedFrom { get; set; } = [];
}

public class RoleEditViewModel
{
    public string Name { get; set; } = string.Empty;
    public bool IsDefault { get; set; }
    public bool IsPublic { get; set; }
    public bool IsStatic { get; set; }
    public string? ConcurrencyStamp { get; set; }
}
