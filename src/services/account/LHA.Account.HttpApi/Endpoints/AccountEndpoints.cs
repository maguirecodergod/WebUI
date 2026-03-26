using LHA.Account.HttpApi.Grpc;
using LHA.AuditLog.HttpApi;
using LHA.Identity.HttpApi;
using LHA.PermissionManagement.HttpApi;
using LHA.TenantManagement.HttpApi;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace LHA.Account.HttpApi;

/// <summary>
/// Aggregates all module endpoint mappings for the Account Service composite host.
/// </summary>
public static class AccountEndpoints
{
    /// <summary>
    /// Maps all endpoints from Identity, TenantManagement, AuditLog,
    /// and PermissionManagement modules.
    /// </summary>
    public static IEndpointRouteBuilder MapAccountEndpoints(this IEndpointRouteBuilder endpoints)
    {
        // Identity module: auth, users, roles, permissions, claim-types, security-logs
        endpoints.MapAuthEndpoints();
        endpoints.MapUserEndpoints();
        endpoints.MapRoleEndpoints();
        endpoints.MapPermissionEndpoints();
        endpoints.MapClaimTypeEndpoints();
        endpoints.MapSecurityLogEndpoints();

        // Tenant Management module
        endpoints.MapTenantEndpoints();

        // Audit Log module
        endpoints.MapAuditLogEndpoints();
        endpoints.MapAccountAuditLogEndpoints();

        // Permission Management module
        endpoints.MapPermissionManagementEndpoints();

        // Internal endpoints (permission registration from other services)
        endpoints.MapInternalEndpoints();

        // gRPC services (inter-service communication)
        endpoints.MapGrpcService<PermissionRegistrationGrpcServiceV1>();

        return endpoints;
    }
}
