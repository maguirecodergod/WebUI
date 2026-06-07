namespace LHA.Shared.Domain
{
    /// <summary>
    /// Whether the customer is a taxable business or an end consumer.
    /// Determines reverse-charge, place-of-supply, and rate selection.
    /// </summary>
    public enum CCustomerTaxStatus
    {
        /// <summary>
        /// Private individual — charge tax at supplier's rate
        /// </summary>
        B2C = 1,
        /// <summary>
        /// VAT-registered business, verified — reverse charge eligible
        /// </summary>
        B2B_Verified = 2,
        /// <summary>
        /// Claimed B2B but VAT ID not yet validated
        /// </summary>
        B2B_Unverified = 3,
        /// <summary>
        /// Public bodies — often exempt by statute
        /// </summary>
        Government = 4,
        /// <summary>
        /// Non-profit organizations — exempt from tax
        /// </summary>
        NonProfit = 5,
        /// <summary>
        /// Resellers — exempt from tax
        /// </summary>
        Reseller = 6
    }
}