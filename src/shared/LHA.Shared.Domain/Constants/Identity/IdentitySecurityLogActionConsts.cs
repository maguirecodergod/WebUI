namespace LHA.Shared.Domain.Identity
{
    /// <summary>
    /// Security log action constants.
    /// </summary>
    public static class IdentitySecurityLogActionConsts
    {
        public const string LoginSucceeded = "LoginSucceeded";
        public const string LoginFailed = "LoginFailed";
        public const string LoginNotAllowed = "LoginNotAllowed";
        public const string LoginLockedout = "LoginLockedout";
        public const string LoginRequiresTwoFactor = "LoginRequiresTwoFactor";
        public const string Logout = "Logout";
        public const string ChangePassword = "ChangePassword";
        public const string TokenRefreshed = "TokenRefreshed";
    }
}