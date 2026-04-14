using LHA.BlazorWasm.App.Components.Base;
using LHA.BlazorWasm.Components.Sidebar.Models;
using LHA.BlazorWasm.Components.Sidebar;
using LHA.BlazorWasm.Components.Topbar;
using Microsoft.AspNetCore.Components;
using LHA.Shared.Contracts.AuditLog;

namespace LHA.BlazorWasm.App.Layout.MainLayout;

public partial class MainLayout : LHALayoutComponentBase
{
    [Inject] protected ITopbarService TopbarService { get; set; } = default!;

    protected Sidebar? _sidebar;

    protected List<SidebarItemModel> _sidebarItems = new()
    {
        new()
        {
            Id = "home",
            TitleKey = "Sidebar.Home",
            Href = "/",
            MatchMode = CNavLinkMatchMode.Exact,
            Icon = """<i class="bi bi-house"></i>"""
        },
        new() { IsDivider = true },
        new()
        {
            Id = "management",
            TitleKey = "Sidebar.Management",
            Icon = """<i class="bi bi-shield-lock"></i>""",
            Children = new()
            {
                new() 
                { 
                    Id = "logs", 
                    TitleKey = "Sidebar.AuditLogs", 
                    Href = "/audit-logs", 
                    Icon = """<i class="bi bi-journal-text"></i>""",
                    RequiredPermission = AuditLogPermissions.AuditLogs.Read
                }
            }
        }
    };

    protected void OnSidebarItemClick(SidebarItemModel item)
    {
        // Optional: handle additional sidebar item click logic
    }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        
        // Simulate async loading finish
        _ = Task.Delay(500).ContinueWith(_ => TopbarService.SetLoading(false));
    }
}
