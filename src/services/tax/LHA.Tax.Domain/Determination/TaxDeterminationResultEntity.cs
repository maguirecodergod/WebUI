using LHA.Ddd.Domain;
using LHA.Shared.Domain;

namespace LHA.Tax.Domain.Determination
{
    /// <summary>
    /// The computed tax decision returned by the tax engine.
    /// Immutable once stored — represents what was decided at transaction time.
    /// </summary>
    public class TaxDeterminationResultEntity : FullAuditedEntity<Guid>
    {
        public Guid RequestId { get; set; }

        public Guid? AppliedTaxRateId { get; set; }
        public virtual Configuration.TaxRateEntity? AppliedTaxRate { get; set; }

        public CTaxRateCategory RateCategory { get; set; }
        public CTaxLiability LiabilityType { get; set; }
        public CPlaceOfSupplyRule PlaceOfSupplyRuleApplied { get; set; }

        /// <summary>Which jurisdiction is entitled to the tax revenue.</summary>
        public Guid TaxingJurisdictionId { get; set; }
        public virtual Aggregates.TaxJurisdictionEntity TaxingJurisdiction { get; set; } = default!;

        public decimal TaxRate { get; set; }

        public decimal TaxableAmount { get; set; }

        public decimal TaxAmountInBillingCurrency { get; set; }

        /// <summary>For the tax return: amount in jurisdiction's local currency.</summary>
        public decimal TaxAmountInLocalCurrency { get; set; }

        /// <summary>
        /// Was a customer exemption applied? Points to the certificate used.
        /// </summary>
        public Guid? AppliedExemptionId { get; set; }
        public virtual Customers.CustomerTaxExemptionEntity? AppliedExemption { get; set; }

        /// <summary>
        /// Machine-readable reason code for why this tax treatment was chosen.
        /// Critical for audit: "B2B_REVERSE_CHARGE_EU_ARTICLE_44" etc.
        /// </summary>
        public string ReasonCode { get; set; } = default!;

        public string? ReasonNarrative { get; set; }

        public DateTime DeterminedAt { get; set; } = DateTime.UtcNow;

        /// <summary>Version of the tax engine logic that produced this result. For audit.</summary>
        public string TaxEngineVersion { get; set; } = default!;
    }
}
