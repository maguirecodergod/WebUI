using LHA.BlazorWasm.UI.Modules;
using LHA.BlazorWasm.UI.Permissions;
using LHA.BlazorWasm.DesignSystem.Icons;

namespace LHA.BlazorWasm.Modules.UserManagement;

/// <summary>
/// User Management UI Module.
/// Registers menus, routes, and permissions for user management.
/// </summary>
public sealed class UserManagementUiModule : IUiModule
{
    public string ModuleId => "user-management";
    public string ModuleName => "User Management";
    public int Order => 10;

    public void Configure(UiModuleBuilder builder)
    {
        // Register permissions
        builder.AddPermissions(
            PermissionNames.UsersView,
            PermissionNames.UsersCreate,
            PermissionNames.UsersEdit,
            PermissionNames.UsersDelete,
            PermissionNames.UsersManage
        );

        // Register main menu
        builder.AddMenu("users", menu =>
        {
            menu.Title = "Users";
            menu.Route = "/admin/users";
            menu.Icon = IconNames.Users;
            menu.Permission = PermissionNames.UsersView;
            menu.Order = 100;
            menu.Children =
            [
                new()
                {
                    Id = "users.list",
                    Title = "All Users",
                    Route = "/admin/users",
                    Icon = IconNames.List,
                    Permission = PermissionNames.UsersView,
                    Order = 1
                },
                new()
                {
                    Id = "users.create",
                    Title = "Create User",
                    Route = "/admin/users/create",
                    Icon = IconNames.UserPlus,
                    Permission = PermissionNames.UsersCreate,
                    Order = 2
                },
                new()
                {
                    Id = "users.roles",
                    Title = "Roles",
                    Route = "/admin/roles",
                    Icon = IconNames.Shield,
                    Permission = PermissionNames.RolesView,
                    Order = 3
                }
            ];
        });

        // Register admin menu
        builder.AddAdminMenu("admin.users", menu =>
        {
            menu.Title = "Users";
            menu.Route = "/admin/users";
            menu.Icon = IconNames.Users;
            menu.Permission = PermissionNames.AdminAccess;
            menu.Order = 10;
        });
    }
}
