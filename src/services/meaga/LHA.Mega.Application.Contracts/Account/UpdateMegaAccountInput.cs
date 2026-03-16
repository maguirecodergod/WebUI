namespace LHA.Mega.Application.Contracts.Account;

public sealed class UpdateMegaAccountInput
{
    public required string Name { get; init; }
    public string? PhoneNumber { get; init; }
    public string? Email { get; init; }
    public string? Address { get; init; }
    public bool IsActive { get; init; }
    public required string ConcurrencyStamp { get; init; }
}
