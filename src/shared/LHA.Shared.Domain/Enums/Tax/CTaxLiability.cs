namespace LHA.Shared.Domain
{
    /// <summary>
    /// Determines which party is liable for remitting tax.
    /// </summary>
    public enum CTaxLiability
    {
        /// <summary>
        /// Normal: supplier collects and remits
        /// </summary>
        /// <remarks>
        /// This is the default case for most businesses.
        /// </remarks>
        SupplierCharged = 1,
        /// <summary>
        /// B2B cross-border: customer self-accounts
        /// </summary>
        ReverseCharge = 2,
        /// <summary>
        /// IT/TR: customer self-accounts
        /// </summary>
        /// <remarks>
        /// This is the default case for most businesses.
        /// </remarks>
        SplitPayment = 3,
        /// <summary>
        /// Zero/Exempt — no tax recovery on costs
        /// </summary>
        NotApplicable = 4
    }
}