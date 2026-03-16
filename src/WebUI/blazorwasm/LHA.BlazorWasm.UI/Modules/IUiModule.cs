namespace LHA.BlazorWasm.UI.Modules;

/// <summary>
/// Interface for UI modules.
/// Similar to ABP's module system — each module can register menus, routes, permissions, and widgets.
/// </summary>
public interface IUiModule
{
    /// <summary> Module unique identifier. </summary>
    string ModuleId { get; }

    /// <summary> Human-readable module name. </summary>
    string ModuleName { get; }

    /// <summary> Module initialization order (lower = earlier). </summary>
    int Order => 0;

    /// <summary>
    /// Configure the module: register menus, routes, permissions, widgets.
    /// </summary>
    void Configure(UiModuleBuilder builder);
}
