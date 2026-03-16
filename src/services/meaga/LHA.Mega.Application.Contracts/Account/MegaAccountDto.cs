using LHA.Ddd.Application;

namespace LHA.Mega.Application.Contracts.Account;

public sealed class MegaAccountDto : FullAuditedEntityDto<Guid>
{
    public Guid? TenantId { get; init; }
    public required string Code { get; init; }
    public required string Name { get; init; }
    public string? PhoneNumber { get; init; }
    public string? Email { get; init; }
    public string? Address { get; init; }
    public bool IsActive { get; init; }
}
