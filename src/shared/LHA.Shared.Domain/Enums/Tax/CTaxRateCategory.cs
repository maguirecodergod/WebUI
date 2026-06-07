namespace LHA.Shared.Domain
{
    public enum CTaxRateCategory
    {
        /// <summary>
        /// 1 - Standard: Full rate (e.g., 20% UK, 19% DE)
        /// </summary>
        Standard   = 1,
        /// <summary>
        /// 2 - Reduced: Reduced rate (e.g., 5% UK books, 7% DE food)
        /// </summary>
        Reduced    = 2,
        /// <summary>
        /// 3 - SuperReduced: Some EU states have a 3rd bracket (IE 4.8%, LU 3%)
        /// </summary>
        SuperReduced = 3,
        /// <summary>
        /// 4 - ZeroRated: Taxable supply, 0% — customer can still claim input
        /// </summary>
        ZeroRated  = 4,
        /// <summary>
        /// 5 - Exempt: Outside scope — no input tax recovery on costs
        /// </summary>
        Exempt     = 5,
        /// <summary>
        /// 6 - ReverseCharge: B2B cross-border: buyer accounts for tax
        /// </summary>
        ReverseCharge = 6,
        /// <summary>
        /// 7 - OutOfScope: Export, EEA distance selling under threshold
        /// </summary>
        OutOfScope = 7
    }
}