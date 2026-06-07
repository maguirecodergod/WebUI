using LHA.Ddd.Domain;

namespace LHA.Tax.Domain.Configuration
{
    /// <summary>
    /// Defines the sales threshold beyond which the business is required
    /// to register and collect tax in that jurisdiction.
    ///
    /// Examples:
    ///   EU OSS: €10,000 total cross-border B2C turnover before OSS applies
    ///   Australia GST: AUD 75,000 / year
    ///   US states (post-Wayfair): USD 100,000 or 200 transactions per state
    ///   UK: GBP 85,000 / year
    /// </summary>
    public class TaxRegistrationThresholdEntity : FullAuditedEntity<Guid>
    {
        public Guid JurisdictionId { get; set; }
        public virtual Aggregates.TaxJurisdictionEntity Jurisdiction { get; set; } = default!;

        /// <summary>Revenue threshold in the jurisdiction's local currency.</summary>
        public decimal ThresholdAmount { get; set; }

        public string CurrencyCode { get; set; } = default!;

        /// <summary>Rolling 12-month, calendar year, fiscal year?</summary>
        public string MeasurementPeriod { get; set; } = "Rolling12Months";

        /// <summary>US states also use transaction count. Null = not applicable.</summary>
        public int? TransactionCountThreshold { get; set; }

        /// <summary>
        /// Only for digital services? Some jurisdictions have different thresholds
        /// for digital services vs physical goods.
        /// </summary>
        public bool DigitalServicesOnly { get; set; }

        public DateOnly EffectiveFrom { get; set; }
        public DateOnly EffectiveTo { get; set; } = new DateOnly(9999, 12, 31);
    }
}
