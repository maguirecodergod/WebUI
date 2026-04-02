namespace LHA.Shared.Domain.Identity
{
    /// <summary>
    /// The provider that grants a permission.
    /// <c>"R"</c> = role, <c>"U"</c> = user.
    /// </summary>
    public static class PermissionGrantProviderName
    {
        /// <summary>Permission granted to a role.</summary>
        public const string Role = "R";

        /// <summary>Permission granted to a specific user.</summary>
        public const string User = "U";
    }
}