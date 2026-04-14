using LHA.BlazorWasm.App.Components.Base;
using LHA.BlazorWasm.Components.Sidebar;
using LHA.BlazorWasm.Components.Sidebar.Models;
using LHA.BlazorWasm.Components.Topbar;
using Microsoft.AspNetCore.Components;
using LHA.BlazorWasm.Services.Theme;

namespace LHA.BlazorWasm.App.Layout.TenantAdminLayout;

public partial class TenantAdminLayout : LHALayoutComponentBase
{
    [Inject] protected ITopbarService TopbarService { get; set; } = default!;

    protected Sidebar? _sidebar;

    protected List<SidebarItemModel> _sidebarItems = new()
    {
        new()
        {
            Id = "ta-dashboard",
            TitleKey = "Sidebar.Dashboard",
            Href = "/dashboard",
            MatchMode = CNavLinkMatchMode.Exact,
            Icon = """<i class="bi bi-house"></i>"""
        },
        new() { IsDivider = true, TitleKey = "Content" },
        new()
        {
            Id = "ta-movies",
            TitleKey = "Sidebar.Movies",
            Href = "/movies",
            Icon = """<i class="bi bi-film"></i>"""
        },
        new()
        {
            Id = "ta-actors",
            TitleKey = "Sidebar.Actors",
            Href = "/actors",
            Icon = """<i class="bi bi-person-badge"></i>"""
        },
        new() { IsDivider = true, TitleKey = "Organization" },
        new()
        {
            Id = "ta-users",
            TitleKey = "Sidebar.Users",
            Href = "/org/users",
            Icon = """<i class="bi bi-people"></i>""",
            Badge = "12"
        },
        new()
        {
            Id = "ta-roles",
            TitleKey = "Sidebar.Roles",
            Href = "/org/roles",
            Icon = """<i class="bi bi-shield-lock"></i>"""
        },
        new() { IsDivider = true, TitleKey = "Administration" },
        new()
        {
            Id = "ta-settings",
            TitleKey = "Sidebar.Settings",
            Href = "/org/settings",
            Icon = """<i class="bi bi-gear"></i>"""
        }
    };

    protected void OnSidebarItemClick(SidebarItemModel item)
    {
    }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        
        TopbarService.SetUser(new UserInfoModel 
        { 
            DisplayName = "Admin", 
            Role = "Acme Corp.",
            Email = "admin@acme.corp"
        });

        // Simulating async load
        _ = Task.Delay(1500).ContinueWith(_ => TopbarService.SetLoading(false));
    }
}
