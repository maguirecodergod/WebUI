using LHA.BlazorWasm.App.Components.Base;
using LHA.BlazorWasm.Components.Sidebar.Models;
using LHA.BlazorWasm.Components.Sidebar;
using LHA.BlazorWasm.Components.Topbar;
using Microsoft.AspNetCore.Components;
using LHA.BlazorWasm.Services.Theme;

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
        new()
        {
            Id = "counter",
            TitleKey = "Sidebar.Counter",
            Href = "/counter",
            Icon = """<i class="bi bi-plus-square"></i>"""
        },
        new()
        {
            Id = "weather",
            TitleKey = "Sidebar.Weather",
            Href = "/weather",
            Icon = """<i class="bi bi-cloud-sun"></i>"""
        },
        new() { IsDivider = true },
        new()
        {
            Id = "content",
            TitleKey = "Sidebar.Content",
            Icon = """<i class="bi bi-file-earmark-text"></i>""",
            Children = new()
            {
                new()
                {
                    Id = "editor",
                    TitleKey = "Sidebar.RichEditor",
                    Href = "/editor-example",
                    Icon = """<i class="bi bi-pencil-square"></i>"""
                },
                new()
                {
                    Id = "media",
                    TitleKey = "Sidebar.Media",
                    Icon = """<i class="bi bi-images"></i>""",
                    Children = new()
                    {
                        new() { Id = "images", TitleKey = "Sidebar.Images", Href = "/content/images" },
                        new() { Id = "videos", TitleKey = "Sidebar.Videos", Href = "/content/videos" },
                        new() { Id = "documents", TitleKey = "Sidebar.Documents", Href = "/content/documents" }
                    }
                }
            }
        },
        new()
        {
            Id = "settings",
            TitleKey = "Sidebar.Settings",
            Icon = """<i class="bi bi-gear"></i>""",
            Children = new()
            {
                new()
                {
                    Id = "general",
                    TitleKey = "Sidebar.General",
                    Href = "/settings/general"
                },
                new()
                {
                    Id = "security",
                    TitleKey = "Sidebar.Security",
                    Href = "/settings/security",
                    Badge = "New"
                },
                new()
                {
                    Id = "advanced",
                    TitleKey = "Sidebar.Advanced",
                    Children = new()
                    {
                        new() { Id = "logs", TitleKey = "Sidebar.AuditLogs", Href = "/audit-logs", Icon = """<i class="bi bi-journal-text"></i>""" },
                        new() { Id = "config", TitleKey = "Sidebar.Config", Href = "/settings/advanced/config" }
                    }
                }
            }
        },
        new() { IsDivider = true },
        new()
        {
            Id = "api-test",
            TitleKey = "Sidebar.ApiTest",
            Href = "/test",
            Icon = """<i class="bi bi-activity"></i>""",
            Badge = "β"
        }
    };

    protected void OnSidebarItemClick(SidebarItemModel item)
    {
        // Optional: handle additional sidebar item click logic
    }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        
        // Initialize Topbar sample data
        TopbarService.SetUser(new UserInfoModel 
        { 
            DisplayName = "John Doe", 
            Role = "Administrator",
            Email = "john.doe@example.com"
        });

        TopbarService.AddNotification(new NotificationModel
        {
            Title = "Welcome!",
            Message = "Welcome to the enterprise LHA WebUI dashboard.",
            Severity = NotificationSeverity.Info
        });

        // Simulate async loading finish
        _ = Task.Delay(2000).ContinueWith(_ => TopbarService.SetLoading(false));
    }
}
