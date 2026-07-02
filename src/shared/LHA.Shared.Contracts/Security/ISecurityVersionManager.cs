namespace LHA.Shared.Contracts.Security;

public interface ISecurityVersionManager
{
    Task<long> GetMaxVersionAsync(string userId, IReadOnlyCollection<string> roles, CancellationToken cancellationToken = default);

    Task<bool> IsIssuedAtValidAsync(long issuedAtUnixSeconds, string userId, IReadOnlyCollection<string> roles, CancellationToken cancellationToken = default);

    Task<long> BumpUserAsync(string userId, CancellationToken cancellationToken = default);

    Task<long> BumpRoleAsync(string roleName, CancellationToken cancellationToken = default);
}

