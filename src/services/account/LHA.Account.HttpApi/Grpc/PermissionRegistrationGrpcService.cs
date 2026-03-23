using Grpc.Core;
using LHA.Account.Application.Contracts.Permissions;
using LHA.Grpc.Contracts.Services.Account.V1;

namespace LHA.Account.HttpApi.Grpc;

/// <summary>
/// gRPC server endpoint for external microservices to register their permissions
/// with the Account (IAM) service. Wraps <see cref="IPermissionRegistrationService"/>.
/// </summary>
public sealed class PermissionRegistrationGrpcServiceV1(
    IPermissionRegistrationService registrationService)
    : PermissionRegistrationService.PermissionRegistrationServiceBase
{
    public override async Task<RegisterPermissionsResponse> RegisterPermissions(
        RegisterPermissionsRequest request, ServerCallContext context)
    {
        var input = new RegisterServicePermissionsInput
        {
            ServiceName = request.ServiceName,
            Permissions = request.Permissions.Select(p => new PermissionDefinitionInput
            {
                Name = p.Name,
                DisplayName = p.DisplayName,
                GroupName = p.GroupName,
            }).ToList(),
            Groups = request.Groups.Select(g => new PermissionGroupInput
            {
                Name = g.Name,
                DisplayName = g.DisplayName,
            }).ToList(),
            GrantAllToAdminRole = request.GrantAllToAdminRole,
        };

        await registrationService.RegisterAsync(input, context.CancellationToken);

        return new RegisterPermissionsResponse
        {
            Success = true,
            Message = "Permissions registered successfully.",
        };
    }
}
