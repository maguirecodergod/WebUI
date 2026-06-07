namespace LHA.Shared.Domain.Identity
{
    /// <summary>
    /// Value types for <c>IdentityClaimType</c>.
    /// </summary>
    public enum CIdentityClaimValueType
    {
        /// <summary>
        /// 0 - String: Free-form string.
        /// </summary>
        String = 0,

        /// <summary>
        /// 1 - Int: Integer value.
        /// </summary>
        Int = 1,

        /// <summary>
        /// 2 - Boolean: Boolean (true / false).
        /// </summary>
        Boolean = 2,

        /// <summary>
        /// 3 - DateTime: ISO-8601 date/time.
        /// </summary>
        DateTime = 3
    }
}