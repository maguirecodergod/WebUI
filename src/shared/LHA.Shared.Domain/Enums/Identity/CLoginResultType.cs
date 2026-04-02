namespace LHA.Shared.Domain.Identity
{
    /// <summary>
    /// Represents the result of a login attempt.
    /// </summary>
    public enum CLoginResultType
    {
        /// <summary>Login succeeded.</summary>
        Success = 1,

        /// <summary>Invalid username or email.</summary>
        InvalidUserNameOrEmail = 2,

        /// <summary>Invalid password.</summary>
        InvalidPassword = 3,

        /// <summary>User account is not active / disabled.</summary>
        NotAllowed = 4,

        /// <summary>User account is locked out due to excessive failed attempts.</summary>
        LockedOut = 5,

        /// <summary>
        /// Two-factor authentication is required but not yet completed.
        /// </summary>
        RequiresTwoFactor = 6,

        /// <summary>User's tenant is inactive.</summary>
        TenantIsNotActive = 7
    }
}