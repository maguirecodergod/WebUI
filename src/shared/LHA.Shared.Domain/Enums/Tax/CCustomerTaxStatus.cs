namespace LHA.Shared.Domain
{
    /// <summary>
    /// Whether the customer is a taxable business or an end consumer.
    /// Determines reverse-charge, place-of-supply, and rate selection.
    /// </summary>
    public enum CCustomerTaxStatus
    {
        /// <summary>
        /// 1 - B2C: Private individual — charge tax at supplier's rate
        /// </summary>
        B2C = 1,
        /// <summary>
        /// 2 - B2B_Verified: VAT-registered business, verified — reverse charge eligible
        /// </summary>
        B2B_Verified = 2,
        /// <summary>
        /// 3 - B2B_Unverified: Claimed B2B but VAT ID not yet validated
        /// </summary>
        B2B_Unverified = 3,
        /// <summary>
        /// 4 - Government: Public bodies — often exempt by statute
        /// </summary>
        Government = 4,
        /// <summary>
        /// 5 - NonProfit: Non-profit organizations — exempt from tax
        /// </summary>
        NonProfit = 5,
        /// <summary>
        /// 6 - Reseller: Resellers — exempt from tax
        /// </summary>
        Reseller = 6
    }
}