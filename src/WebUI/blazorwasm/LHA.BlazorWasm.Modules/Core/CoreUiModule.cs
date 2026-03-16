using LHA.BlazorWasm.UI.Modules;
using LHA.BlazorWasm.DesignSystem.Icons;

namespace LHA.BlazorWasm.Modules.Core;

/// <summary>
/// Core UI Module — registers the default navigation items that every SaaS app needs.
/// </summary>
public sealed class CoreUiModule : IUiModule
{
    public string ModuleId => "core";
    public string ModuleName => "Core";
    public int Order => 0;

    public void Configure(UiModuleBuilder builder)
    {
        // Home / Dashboard
        builder.AddMenu("home", menu =>
        {
            menu.Title = "Dashboard";
            menu.Route = "/";
            menu.Icon = IconNames.Home;
            menu.Order = 0;
        });

        builder.AddMenu("analytics", menu =>
        {
            menu.Title = "Analytics";
            menu.Route = "/analytics";
            menu.Icon = IconNames.BarChart;
            menu.Order = 20;
        });

        builder.AddMenu("content", menu =>
        {
            menu.Title = "Content";
            menu.Icon = IconNames.Document;
            menu.Order = 30;
            menu.Children =
            [
                new()
                {
                    Id = "content.editor",
                    Title = "Rich Editor",
                    Route = "/editor-example",
                    Icon = IconNames.Edit,
                    Order = 1
                },
                new()
                {
                    Id = "content.media",
                    Title = "Media",
                    Icon = IconNames.Image,
                    Order = 2,
                    Children =
                    [
                        new() { Id = "content.media.images", Title = "Images", Route = "/content/images", Order = 1 },
                        new() { Id = "content.media.videos", Title = "Videos", Route = "/content/videos", Order = 2 },
                    ]
                }
            ];
        });

        // Settings
        builder.AddMenu("settings", menu =>
        {
            menu.Title = "Settings";
            menu.Icon = IconNames.Settings;
            menu.Order = 900;
            menu.Children =
            [
                new()
                {
                    Id = "settings.general",
                    Title = "General",
                    Route = "/settings/general",
                    Order = 1
                },
                new()
                {
                    Id = "settings.security",
                    Title = "Security",
                    Route = "/settings/security",
                    Badge = "New",
                    Order = 2
                }
            ];
        });

        // Admin menu items
        builder.AddAdminMenu("admin.dashboard", menu =>
        {
            menu.Title = "Dashboard";
            menu.Route = "/admin";
            menu.Icon = IconNames.Dashboard;
            menu.Order = 0;
        });

        builder.AddAdminMenu("admin.settings", menu =>
        {
            menu.Title = "System Settings";
            menu.Route = "/admin/settings";
            menu.Icon = IconNames.Settings;
            menu.Order = 100;
        });
    }
}
