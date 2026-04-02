namespace LHA.Shared.Domain.Identity
{
    /// <summary>
    /// Value types for <c>IdentityClaimType</c>.
    /// </summary>
    public enum CIdentityClaimValueType
    {
        /// <summary>Free-form string.</summary>
        String = 0,

        /// <summary>Integer value.</summary>
        Int = 1,

        /// <summary>Boolean (true / false).</summary>
        Boolean = 2,

        /// <summary>ISO-8601 date/time.</summary>
        DateTime = 3
    }
}