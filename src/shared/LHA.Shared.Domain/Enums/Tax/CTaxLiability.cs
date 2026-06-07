namespace LHA.Shared.Domain
{
    /// <summary>
    /// Determines which party is liable for remitting tax.
    /// </summary>
    public enum CTaxLiability
    {
        /// <summary>
        /// 1 - SupplierCharged: Normal: supplier collects and remits
        /// </summary>
        SupplierCharged = 1,
        /// <summary>
        /// 2 - ReverseCharge: B2B cross-border: customer self-accounts
        /// </summary>
        ReverseCharge = 2,
        /// <summary>
        /// 3 - SplitPayment: IT/TR: customer self-accounts
        /// </summary>
        SplitPayment = 3,
        /// <summary>
        /// 4 - NotApplicable: Zero/Exempt — no tax recovery on costs
        /// </summary>
        NotApplicable = 4
    }
}