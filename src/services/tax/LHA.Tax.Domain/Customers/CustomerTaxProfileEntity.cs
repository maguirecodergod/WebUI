using LHA.Ddd.Domain;
using LHA.Shared.Domain;

namespace LHA.Tax.Domain.Customers
{
    /// <summary>
    /// A specific tax rate for a jurisdiction + regime + product category combination.
    /// Fully historized: never update, only insert new records with new EffectiveFrom.
    ///
    /// Example row set for UK digital services:
    ///   Regime=UK VAT | Category=Digital Services | Standard | 20% | 2011-01-04 → 9999-12-31
    ///
    /// When UK temporarily reduced VAT to 5% during COVID:
    ///   Regime=UK VAT | Category=Hospitality | Reduced | 5% | 2020-07-15 → 2021-09-30
    ///   Regime=UK VAT | Category=Hospitality | Standard | 20% | 2021-10-01 → 9999-12-31
    ///
    /// Query: get rate for invoice dated 2020-08-01 → returns 5%.
    /// Query: get rate for invoice dated 2022-01-01 → returns 20%. ✓
    /// </summary>
    public class CustomerTaxProfileEntity : FullAuditedEntity<Guid>
    {
        /// <summary>FK to your CRM/Account entity.</summary>
        public Guid CustomerId { get; set; }

        public CCustomerTaxStatus TaxStatus { get; set; }

        /// <summary>
        /// The jurisdiction where the customer is established / tax-resident.
        /// Drives place-of-supply for B2B cross-border.
        /// </summary>
        public Guid EstablishedJurisdictionId { get; set; }
        public virtual Aggregates.TaxJurisdictionEntity EstablishedJurisdiction { get; set; } = default!;

        /// <summary>
        /// Is this a consumer in an EU member state for VAT purposes?
        /// B2C in EU triggers OSS / destination-country tax rate.
        /// </summary>
        public bool IsEuConsumer { get; set; }

        public ICollection<CustomerTaxIdentifierEntity> TaxIdentifiers { get; set; } = new List<CustomerTaxIdentifierEntity>();
        public ICollection<CustomerTaxExemptionEntity> Exemptions { get; set; } = new List<CustomerTaxExemptionEntity>();
    }
}
