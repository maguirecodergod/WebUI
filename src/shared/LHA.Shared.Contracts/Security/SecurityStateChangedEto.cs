namespace LHA.Shared.Contracts.Security;

public sealed record SecurityStateChangedEto(
    SecurityStateTargetType TargetType,
    string TargetId,
    long VersionUnixSeconds,
    string Reason);

public enum SecurityStateTargetType
{
    User = 1,
    Role = 2
}

