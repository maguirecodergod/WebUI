namespace LHA.Shared.Domain.Identity
{
    /// <summary>
    /// Represents the result of a login attempt.
    /// </summary>
    public enum CLoginResultType
    {
        /// <summary>
        /// 1 - Success: Login succeeded.
        /// </summary>
        Success = 1,

        /// <summary>
        /// 2 - InvalidUserNameOrEmail: Invalid username or email.
        /// </summary>
        InvalidUserNameOrEmail = 2,

        /// <summary>
        /// 3 - InvalidPassword: Invalid password.
        /// </summary>
        InvalidPassword = 3,

        /// <summary>
        /// 4 - NotAllowed: User account is not active / disabled.
        /// </summary>
        NotAllowed = 4,

        /// <summary>
        /// 5 - LockedOut: User account is locked out due to excessive failed attempts.
        /// </summary>
        LockedOut = 5,

        /// <summary>
        /// 6 - RequiresTwoFactor: Two-factor authentication is required but not yet completed.
        /// </summary>
        RequiresTwoFactor = 6,

        /// <summary>
        /// 7 - TenantIsNotActive: User's tenant is inactive.
        /// </summary>
        TenantIsNotActive = 7
    }
}