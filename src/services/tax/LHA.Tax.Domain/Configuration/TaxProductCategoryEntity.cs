using LHA.Ddd.Domain;

namespace LHA.Tax.Domain.Configuration
{
    /// <summary>
    /// Classifies products or services for tax purposes.
    /// Different from your internal product category — this is the tax classification.
    /// A single product may map to different tax categories in different countries.
    /// E.g., "Baby food" is zero-rated in UK, standard-rated in Germany.
    /// </summary>
    public class TaxProductCategoryEntity : FullAuditedEntity<Guid>
    {
        public string Code { get; set; } = default!;

        public string Name { get; set; } = default!;

        public string? Description { get; set; }

        /// <summary>
        /// UN CPC / WTO HS code for cross-border classification.
        /// Used to auto-map to jurisdiction-specific rules.
        /// </summary>
        public string? HsCode { get; set; }

        /// <summary>
        /// EU CN code (Combined Nomenclature) for EU VAT purposes.
        /// </summary>
        public string? EuCnCode { get; set; }

        /// <summary>
        /// Is this a digital/electronically supplied service?
        /// Critical: triggers customer-location place-of-supply rules under EU VAT 2015.
        /// </summary>
        public bool IsDigitalService { get; set; }

        /// <summary>Whether this category is a service (vs goods).</summary>
        public bool IsService { get; set; }

        public ICollection<TaxRateEntity> Rates { get; set; } = new List<TaxRateEntity>();
    }
}
