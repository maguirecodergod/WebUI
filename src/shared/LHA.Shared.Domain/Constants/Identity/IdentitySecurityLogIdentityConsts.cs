namespace LHA.Shared.Domain.Identity
{
    /// <summary>
    /// Security log identity constants (the "source" of the action).
    /// </summary>
    public static class IdentitySecurityLogIdentityConsts
    {
        public const string Identity = "Identity";
        public const string IdentityExternal = "IdentityExternal";
        public const string IdentityTwoFactor = "IdentityTwoFactor";
    }
}