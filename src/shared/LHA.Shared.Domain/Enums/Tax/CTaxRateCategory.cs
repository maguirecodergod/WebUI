namespace LHA.Shared.Domain
{
    public enum CTaxRateCategory
    {
        /// <summary>
        /// Full rate (e.g., 20% UK, 19% DE)
        /// </summary>
        Standard   = 1,
        /// <summary>
        /// Reduced rate (e.g., 5% UK books, 7% DE food)
        /// </summary>
        Reduced    = 2,
        /// <summary>
        /// Some EU states have a 3rd bracket (IE 4.8%, LU 3%)
        /// </summary>
        SuperReduced = 3,
        /// <summary>
        /// Taxable supply, 0% — customer can still claim input
        /// </summary>
        ZeroRated  = 4,
        /// <summary>
        /// Outside scope — no input tax recovery on costs
        /// </summary>
        Exempt     = 5,
        /// <summary>
        /// B2B cross-border: buyer accounts for tax
        /// </summary>
        ReverseCharge = 6,
        /// <summary>
        /// Export, EEA distance selling under threshold
        /// </summary>
        OutOfScope = 7
    }
}