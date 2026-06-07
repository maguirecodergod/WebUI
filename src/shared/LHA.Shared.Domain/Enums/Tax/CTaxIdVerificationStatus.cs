namespace LHA.Shared.Domain
{
    public enum CTaxIdVerificationStatus
    {
        /// <summary>
        /// 1 - Pending
        /// </summary>
        Pending = 1,
        /// <summary>
        /// 2 - Valid
        /// </summary>
        Valid = 2,
        /// <summary>
        /// 3 - Invalid
        /// </summary>
        Invalid = 3,
        /// <summary>
        /// 4 - UnverifiableApiDown
        /// </summary>
        UnverifiableApiDown = 4,
        /// <summary>
        /// 5 - Expired
        /// </summary>
        Expired = 5
    }
}