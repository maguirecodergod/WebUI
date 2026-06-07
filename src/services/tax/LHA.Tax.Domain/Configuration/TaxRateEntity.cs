using LHA.Core;
using LHA.Ddd.Domain;
using LHA.Shared.Domain;

namespace LHA.Tax.Domain.Configuration
{
    public class TaxRateEntity : FullAuditedEntity<Guid>
    {
        public Guid TaxRegimeId { get; set; }
        public virtual TaxRegimeEntity TaxRegime { get; set; } = default!;

        /// <summary>
        /// Null = applies to all product categories (default/catch-all rate).
        /// Non-null = applies only to this product category.
        /// Resolution: most-specific match wins.
        /// </summary>
        public Guid? TaxProductCategoryId { get; set; }
        public virtual TaxProductCategoryEntity? TaxProductCategory { get; set; }

        public CTaxRateCategory RateCategory { get; set; }

        /// <summary>
        /// Customer type scope. Null = applies to all customers.
        /// Use to define different rates for B2B vs B2C where relevant.
        /// </summary>
        public CCustomerTaxStatus? ApplicableToCustomerStatus { get; set; }

        /// <summary>Rate as decimal: 0.20 = 20%, 0.05 = 5%, 0 = zero-rated.</summary>
        public decimal Rate { get; set; }

        /// <summary>
        /// Date from which this rate is legally effective.
        /// The query layer always filters: EffectiveFrom ≤ InvoiceDate ≤ EffectiveTo.
        /// </summary>
        public DateOnly EffectiveFrom { get; set; }

        /// <summary>9999-12-31 = open-ended (currently in force).</summary>
        public DateOnly EffectiveTo { get; set; } = new DateOnly(9999, 12, 31);

        /// <summary>Official legislative reference, e.g. "UK VATA 1994 s.29A Group 3"</summary>
        public string? LegalReference { get; set; }

        /// <summary>Internal note for compliance team.</summary>
        public string? Notes { get; set; }

        public CMasterStatus Status { get; set; } = CMasterStatus.Active;
    }
}
