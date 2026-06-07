using LHA.Core;
using LHA.Ddd.Domain;

namespace LHA.Tax.Domain.Customers
{
    /// <summary>
    /// A customer's exemption from tax in a specific jurisdiction.
    /// Examples:
    ///   - US resale certificates (customer resells to end consumers)
    ///   - EU article 151 exemptions (diplomatic missions, NATO)
    ///   - Healthcare/charity exemptions
    ///   - US government/federal exemptions
    /// </summary>
    public class CustomerTaxExemptionEntity : FullAuditedEntity<Guid>
    {
        public Guid CustomerTaxProfileId { get; set; }
        public virtual CustomerTaxProfileEntity CustomerTaxProfile { get; set; } = default!;

        public Guid JurisdictionId { get; set; }
        public virtual Aggregates.TaxJurisdictionEntity Jurisdiction { get; set; } = default!;

        /// <summary>Which product categories this exemption covers. Empty = all.</summary>
        public ICollection<Configuration.TaxProductCategoryEntity> ApplicableCategories { get; set; } = new List<Configuration.TaxProductCategoryEntity>();

        public string ExemptionType { get; set; } = default!; // "Resale", "Government", "Charity", "Diplomatic"

        public string CertificateNumber { get; set; } = default!;

        /// <summary>Scanned certificate document reference.</summary>
        public string? DocumentStorageKey { get; set; }

        public DateOnly IssuedDate { get; set; }
        public DateOnly ExpiryDate { get; set; }

        public CMasterStatus Status { get; set; } = CMasterStatus.Active;
    }
}
