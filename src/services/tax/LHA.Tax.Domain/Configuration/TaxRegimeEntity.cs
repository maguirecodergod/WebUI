using LHA.Ddd.Domain;
using LHA.Shared.Domain;

namespace LHA.Tax.Domain.Configuration
{
    /// <summary>
    /// A specific tax regime within a jurisdiction.
    /// E.g., "Germany VAT (MwSt)", "Australia GST", "California Sales Tax",
    /// "Quebec QST", "India IGST", "India CGST".
    /// </summary>
    public class TaxRegimeEntity : FullAuditedEntity<Guid>
    {
        public Guid JurisdictionId { get; set; }
        public virtual Aggregates.TaxJurisdictionEntity Jurisdiction { get; set; } = default!;

        public string Name { get; set; } = default!;

        /// <summary>Local name, e.g., "Mehrwertsteuer", "TVA", "消費税"</summary>
        public string? LocalName { get; set; }

        public CTaxRegimeType RegimeType { get; set; }

        /// <summary>
        /// Two-character tax type code used on invoices: "VAT", "GST", "QST", "PST".
        /// Required for legal invoice compliance.
        /// </summary>
        public string InvoiceCode { get; set; } = default!;

        /// <summary>
        /// Authority's tax registration number format regex for validation.
        /// E.g., EU VAT: "^[A-Z]{2}[0-9A-Z]{2,12}$"
        /// </summary>
        public string? RegistrationNumberFormat { get; set; }

        /// <summary>
        /// URL for real-time VAT number verification API (VIES, ABN Lookup, etc.)
        /// </summary>
        public string? VatVerificationApiUrl { get; set; }

        /// <summary>
        /// Is the tax inclusive or exclusive of the listed price?
        /// GST-inclusive pricing is common in AU, NZ retail.
        /// </summary>
        public bool IsTaxInclusive { get; set; }

        /// <summary>
        /// Cascade: is this tax applied on top of another tax (tax-on-tax)?
        /// Quebec PST is applied on the price AFTER federal GST is added.
        /// </summary>
        public bool IsCascading { get; set; }

        /// <summary>Filing frequency: Monthly, Quarterly, Annually.</summary>
        public string? FilingFrequency { get; set; }

        public bool IsActive { get; set; } = true;

        public virtual ICollection<TaxRateEntity> Rates { get; set; } = new List<TaxRateEntity>();
        public virtual ICollection<BusinessRegistrations.BusinessTaxRegistrationEntity> BusinessRegistrations { get; set; } = new List<BusinessRegistrations.BusinessTaxRegistrationEntity>();
    }
}
