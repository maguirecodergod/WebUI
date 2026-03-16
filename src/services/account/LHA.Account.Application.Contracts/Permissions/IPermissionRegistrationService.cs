namespace LHA.Account.Application.Contracts.Permissions;

public interface IPermissionRegistrationService
{
    Task RegisterAsync(RegisterServicePermissionsInput input, CancellationToken ct = default);
}
