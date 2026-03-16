namespace LHA.Mega.Application.Contracts.Account;

public sealed class CreateMegaAccountInput
{
    public required string Code { get; init; }
    public required string Name { get; init; }
    public string? PhoneNumber { get; init; }
    public string? Email { get; init; }
    public string? Address { get; init; }
}
