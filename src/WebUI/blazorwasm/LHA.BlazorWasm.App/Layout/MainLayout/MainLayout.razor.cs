using LHA.BlazorWasm.App.Components.Base;
using LHA.BlazorWasm.Components.Sidebar.Models;
using LHA.BlazorWasm.Components.Sidebar;
using LHA.BlazorWasm.Components.Topbar;
using Microsoft.AspNetCore.Components;
using LHA.Shared.Contracts.AuditLog;
using LHA.Shared.Contracts.TenantManagement;
using LHA.Shared.Contracts.Identity;

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
            Id = "host-management",
            GroupName = "Super Admin",
            TitleKey = "Sidebar.HostManagement",
            Icon = """<i class="bi bi-gear-wide-connected"></i>""",
            Children = new()
            {
                new() { Id = "host-tenants", TitleKey = "Sidebar.Tenants", Href = "/host/tenants", Icon = """<i class="bi bi-building"></i>""", RequiredPermission = TenantManagementPermissions.Tenants.Read },
                new() { Id = "host-roles", TitleKey = "Sidebar.Roles", Href = "/host/roles", Icon = """<i class="bi bi-person-vcard"></i>""", RequiredPermission = IdentityPermissions.Roles.Read }
            }
        },
        new()
        {
            Id = "tenant-management",
            GroupName = "Tenant Admin",
            TitleKey = "Sidebar.Management",
            Icon = """<i class="bi bi-shield-lock"></i>""",
            Children = new()
            {
                new() { Id = "tenant-users", TitleKey = "Sidebar.Users", Href = "/tenant/users", Icon = """<i class="bi bi-people"></i>""", RequiredPermission = IdentityPermissions.Users.Read },
                new() { Id = "tenant-logs", TitleKey = "Sidebar.AuditLogs", Href = "/audit-logs", Icon = """<i class="bi bi-journal-text"></i>""", RequiredPermission = AuditLogPermissions.AuditLogs.Read }
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
