using LHA.Ddd.Domain;
using LHA.Shared.Domain;

namespace LHA.Tax.Domain.Customers
{
    /// <summary>
    /// Tax registration numbers the customer holds (they may be VAT-registered
    /// in multiple countries as a multinational).
    /// </summary>
    public class CustomerTaxIdentifierEntity : FullAuditedEntity<Guid>
    {
        public Guid CustomerTaxProfileId { get; set; }
        public virtual CustomerTaxProfileEntity CustomerTaxProfile { get; set; } = default!;

        public Guid JurisdictionId { get; set; }
        public virtual Aggregates.TaxJurisdictionEntity Jurisdiction { get; set; } = default!;

        public string TaxRegistrationNumber { get; set; } = default!;

        public CTaxIdVerificationStatus VerificationStatus { get; set; }
        public DateTime? LastVerifiedAt { get; set; }

        /// <summary>Verification response from VIES / authority API — stored for audit.</summary>
        public string? VerificationResponse { get; set; }

        public DateOnly ValidFrom { get; set; }
        public DateOnly ValidTo { get; set; } = new DateOnly(9999, 12, 31);
    }
}
