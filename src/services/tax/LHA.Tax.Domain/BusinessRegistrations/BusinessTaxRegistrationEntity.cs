using LHA.Core;
using LHA.Ddd.Domain;

namespace LHA.Tax.Domain.BusinessRegistrations
{
    /// <summary>
    /// Records that OUR BUSINESS is registered to collect tax
    /// in a specific jurisdiction under a specific regime.
    /// This drives the "do we charge tax?" decision before rate lookup.
    ///
    /// If no registration exists for a jurisdiction → we are NOT registered
    /// → we cannot legally charge/collect that tax.
    /// </summary>
    public class BusinessTaxRegistrationEntity : FullAuditedEntity<Guid>
    {
        public Guid TaxRegimeId { get; set; }
        public virtual Configuration.TaxRegimeEntity TaxRegime { get; set; } = default!;

        /// <summary>
        /// Our VAT/GST registration number issued by the authority.
        /// E.g., "DE123456789", "AU51824753556", "GB123456789".
        /// </summary>
        public string RegistrationNumber { get; set; } = default!;

        public DateOnly RegistrationDate { get; set; }
        public DateOnly? DeregistrationDate { get; set; }

        /// <summary>
        /// For EU OSS registrations: the member state of identification.
        /// One OSS registration covers all 27 EU B2C digital supplies.
        /// </summary>
        public bool IsOssRegistration { get; set; }

        /// <summary>
        /// Entity (legal entity) that holds this registration.
        /// Multi-entity businesses have different registrations per entity.
        /// </summary>
        public string LegalEntityName { get; set; } = default!;

        public string? LegalEntityAddress { get; set; }

        public CMasterStatus Status { get; set; } = CMasterStatus.Active;
    }
}
