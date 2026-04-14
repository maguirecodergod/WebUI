using LHA.BlazorWasm.App.Components.Base;
using LHA.BlazorWasm.Components.Sidebar;
using LHA.BlazorWasm.Components.Sidebar.Models;
using LHA.BlazorWasm.Components.Topbar;
using Microsoft.AspNetCore.Components;
using LHA.BlazorWasm.Services.Theme;

namespace LHA.BlazorWasm.App.Layout.SuperAdminLayout;

public partial class SuperAdminLayout : LHALayoutComponentBase
{
    [Inject] protected ITopbarService TopbarService { get; set; } = default!;

    protected Sidebar? _sidebar;

    protected List<SidebarItemModel> _sidebarItems = new()
    {
        new()
        {
            Id = "sa-dashboard",
            TitleKey = "Sidebar.Dashboard",
            Href = "/sa",
            MatchMode = CNavLinkMatchMode.Exact,
            Icon = """<i class="bi bi-house"></i>"""
        },
        new() { IsDivider = true, TitleKey = "Platform" },
        new()
        {
            Id = "sa-tenants",
            TitleKey = "Sidebar.Tenants",
            Href = "/sa/tenants",
            Icon = """<i class="bi bi-building"></i>""",
            Badge = "300+"
        },
        new()
        {
            Id = "sa-database",
            TitleKey = "Sidebar.Databases",
            Href = "/sa/databases",
            Icon = """<i class="bi bi-database"></i>"""
        },
        new() { IsDivider = true, TitleKey = "Identity & Security" },
        new()
        {
            Id = "sa-users",
            TitleKey = "Sidebar.GlobalUsers",
            Href = "/sa/users",
            Icon = """<i class="bi bi-people"></i>"""
        },
        new()
        {
            Id = "sa-audit",
            TitleKey = "Sidebar.AuditLogs",
            Href = "/sa/audit-logs",
            Icon = """<i class="bi bi-journal-text"></i>"""
        },
        new() { IsDivider = true, TitleKey = "System" },
        new()
        {
            Id = "sa-settings",
            TitleKey = "Sidebar.Settings",
            Href = "/sa/settings",
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
            DisplayName = "System Admin", 
            Role = "Host Access",
            Email = "admin@lha.core"
        });

        TopbarService.AddNotification(new NotificationModel
        {
            Title = "System Health",
            Message = "All 324 tenant databases are operating normally.",
            Severity = NotificationSeverity.Success
        });

        _ = Task.Delay(1000).ContinueWith(_ => TopbarService.SetLoading(false));
    }
}
