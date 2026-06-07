namespace LHA.Shared.Domain
{
    public enum CTaxIdVerificationStatus
    {
        Pending = 1,
        Valid = 2,
        Invalid = 3,
        UnverifiableApiDown = 4,
        Expired = 5
    }
}