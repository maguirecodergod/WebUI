// ============================================================
// GLOBAL VAT / GST / INDIRECT TAX ENTITY DESIGN
// Designed for: Multi-jurisdiction SaaS / Digital Services
// Tax Expert Perspective: 20+ years VAT/GST global compliance
//
// Key considerations:
//   - VAT (EU), GST (AU/NZ/CA/IN), Sales Tax (US), consumption tax (JP)
//   - Multi-tier jurisdiction: federal → state/province → county/city
//   - B2B vs B2C treatment (reverse charge, input credit)
//   - Digital services place-of-supply rules (EU VAT 2015, OECD BEPS)
//   - Effective-date historization (rates change; old invoices must recalculate)
//   - Registration thresholds (EU OSS, AU GST A$75k, etc.)
//   - Tax exemptions: exempt customers (hospitals, NGO), exempt supplies
//   - Compound tax (some CA provinces: GST + PST stacked)
//   - Currency: tax calculated in local currency, stored in billing currency
// ============================================================

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GlobalTax.Domain
{
    // ──────────────────────────────────────────────────────────
    // ENUMERATIONS
    // ──────────────────────────────────────────────────────────

    /// <summary>
    /// Type of indirect tax regime the jurisdiction operates.
    /// </summary>
    public enum TaxRegimeType
    {
        VAT,            // Value-Added Tax (EU, UK, most of world)
        GST,            // Goods and Services Tax (AU, NZ, IN, CA federal)
        SalesTax,       // US-style: single-stage, no input credit
        ConsumptionTax, // Japan JCT
        ExciseDuty,     // Alcohol, tobacco, fuel – layered on top
        WithholdingTax, // Some APAC service payments
        None            // No indirect tax (Bahrain free zones, etc.)
    }

    /// <summary>
    /// Granularity level of the jurisdiction in the hierarchy.
    /// </summary>
    public enum JurisdictionLevel
    {
        Global    = 0,  // Placeholder root
        Regional  = 1,  // EU, GCC bloc
        Country   = 2,  // DE, AU, US
        State     = 3,  // Bavaria, Queensland, California
        County    = 4,  // US county
        City      = 5   // City / Municipality
    }

    /// <summary>
    /// How this VAT rate category is treated for an invoice line.
    /// Drives the logic of whether tax is charged or just annotated.
    /// </summary>
    public enum TaxRateCategory
    {
        Standard   = 1,  // Full rate (e.g., 20% UK, 19% DE)
        Reduced    = 2,  // Reduced rate (e.g., 5% UK books, 7% DE food)
        SuperReduced = 3,// Some EU states have a 3rd bracket (IE 4.8%, LU 3%)
        ZeroRated  = 4,  // Taxable supply, 0% — customer can still claim input
        Exempt     = 5,  // Outside scope — no input tax recovery on costs
        ReverseCharge = 6, // B2B cross-border: buyer accounts for tax
        OutOfScope = 7   // Export, EEA distance selling under threshold
    }

    /// <summary>
    /// Whether the customer is a taxable business or an end consumer.
    /// Determines reverse-charge, place-of-supply, and rate selection.
    /// </summary>
    public enum CustomerTaxStatus
    {
        B2C          = 1, // Private individual — charge tax at supplier's rate
        B2B_Verified = 2, // VAT-registered business, verified — reverse charge eligible
        B2B_Unverified = 3, // Claimed B2B but VAT ID not yet validated
        Government   = 4, // Public bodies — often exempt by statute
        NonProfit    = 5, // Registered charities — jurisdiction-specific exemption
        Reseller     = 6  // Holds resale certificate (US) or intra-community buyer
    }

    /// <summary>
    /// Determines which party is liable for remitting tax.
    /// </summary>
    public enum TaxLiability
    {
        SupplierCharged,   // Normal: supplier collects and remits
        ReverseCharge,     // B2B cross-border: customer self-accounts
        SplitPayment,      // IT/TR: tax paid direct to authority by bank
        NotApplicable      // Zero/Exempt
    }

    /// <summary>
    /// Place-of-supply rule that determined the taxing jurisdiction.
    /// Audit trail for which rule was applied at the time of supply.
    /// </summary>
    public enum PlaceOfSupplyRule
    {
        SupplierLocation,      // Default for physical goods
        CustomerLocation,      // Digital services (EU VAT 2015, OECD)
        ServicePerformed,      // Restaurant, live events
        PropertyLocation,      // Real estate
        DeparturPoint,         // Passenger transport
        OSSRegistration,       // EU One-Stop-Shop for B2C digital below threshold
        FixedEstablishment     // Where the customer has a fixed establishment
    }

    // ──────────────────────────────────────────────────────────
    // 1. TAX JURISDICTION
    // The territory tree: Global → Region → Country → State → City
    // ──────────────────────────────────────────────────────────

    /// <summary>
    /// Represents a taxing authority territory at any level of the hierarchy.
    /// Self-referencing tree allows: US → California → San Francisco.
    /// A single transaction may traverse multiple jurisdiction levels
    /// (GST federal + QST provincial in Canada).
    /// </summary>
    public class TaxJurisdiction
    {
        [Key]
        public Guid Id { get; set; }

        /// <summary>ISO 3166-1 alpha-2 for Country, ISO 3166-2 for subdivisions.</summary>
        [Required, MaxLength(10)]
        public string Code { get; set; } = default!;

        [Required, MaxLength(100)]
        public string Name { get; set; } = default!;

        public JurisdictionLevel Level { get; set; }

        /// <summary>
        /// Which tax regime governs this jurisdiction.
        /// A country may have multiple: e.g., Canada has GST (federal) + PST/QST (provincial).
        /// Each TaxRegime record points back here.
        /// </summary>
        public TaxRegimeType PrimaryRegime { get; set; }

        /// <summary>Parent in the hierarchy tree.</summary>
        public Guid? ParentJurisdictionId { get; set; }

        [ForeignKey(nameof(ParentJurisdictionId))]
        public TaxJurisdiction? ParentJurisdiction { get; set; }

        /// <summary>
        /// ISO 4217 currency code for local tax reporting.
        /// Tax amounts must also be stored in this currency for returns.
        /// </summary>
        [Required, MaxLength(3)]
        public string LocalCurrencyCode { get; set; } = default!;

        /// <summary>
        /// IANA timezone — needed to determine tax period (Dec 31 23:59 UTC may be Jan 1 in AU).
        /// </summary>
        [Required, MaxLength(50)]
        public string Timezone { get; set; } = default!;

        /// <summary>
        /// Some jurisdictions are compound: apply both this level AND parent level.
        /// E.g., Quebec: federal GST 5% AND QST 9.975%.
        /// </summary>
        public bool IsCompoundWithParent { get; set; }

        public bool IsActive { get; set; } = true;

        public ICollection<TaxJurisdiction> Children { get; set; } = new List<TaxJurisdiction>();
        public ICollection<TaxRegime> TaxRegimes { get; set; } = new List<TaxRegime>();
        public ICollection<TaxRegistrationThreshold> Thresholds { get; set; } = new List<TaxRegistrationThreshold>();
    }

    // ──────────────────────────────────────────────────────────
    // 2. TAX REGIME
    // Governs one specific indirect tax within a jurisdiction
    // (a country can have multiple regimes at different levels)
    // ──────────────────────────────────────────────────────────

    /// <summary>
    /// A specific tax regime within a jurisdiction.
    /// E.g., "Germany VAT (MwSt)", "Australia GST", "California Sales Tax",
    /// "Quebec QST", "India IGST", "India CGST".
    /// </summary>
    public class TaxRegime
    {
        [Key]
        public Guid Id { get; set; }

        public Guid JurisdictionId { get; set; }
        public TaxJurisdiction Jurisdiction { get; set; } = default!;

        [Required, MaxLength(100)]
        public string Name { get; set; } = default!;

        /// <summary>Local name, e.g., "Mehrwertsteuer", "TVA", "消費税"</summary>
        [MaxLength(100)]
        public string? LocalName { get; set; }

        public TaxRegimeType RegimeType { get; set; }

        /// <summary>
        /// Two-character tax type code used on invoices: "VAT", "GST", "QST", "PST".
        /// Required for legal invoice compliance.
        /// </summary>
        [Required, MaxLength(10)]
        public string InvoiceCode { get; set; } = default!;

        /// <summary>
        /// Authority's tax registration number format regex for validation.
        /// E.g., EU VAT: "^[A-Z]{2}[0-9A-Z]{2,12}$"
        /// </summary>
        [MaxLength(200)]
        public string? RegistrationNumberFormat { get; set; }

        /// <summary>
        /// URL for real-time VAT number verification API (VIES, ABN Lookup, etc.)
        /// </summary>
        [MaxLength(500)]
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
        [MaxLength(20)]
        public string? FilingFrequency { get; set; }

        public bool IsActive { get; set; } = true;

        public ICollection<TaxRate> Rates { get; set; } = new List<TaxRate>();
        public ICollection<BusinessTaxRegistration> BusinessRegistrations { get; set; } = new List<BusinessTaxRegistration>();
    }

    // ──────────────────────────────────────────────────────────
    // 3. PRODUCT / SERVICE TAX CATEGORY
    // Classification of what is being sold — drives which rate applies
    // ──────────────────────────────────────────────────────────

    /// <summary>
    /// Classifies products or services for tax purposes.
    /// Different from your internal product category — this is the tax classification.
    /// A single product may map to different tax categories in different countries.
    /// E.g., "Baby food" is zero-rated in UK, standard-rated in Germany.
    /// </summary>
    public class TaxProductCategory
    {
        [Key]
        public Guid Id { get; set; }

        [Required, MaxLength(100)]
        public string Code { get; set; } = default!;

        [Required, MaxLength(200)]
        public string Name { get; set; } = default!;

        [MaxLength(1000)]
        public string? Description { get; set; }

        /// <summary>
        /// UN CPC / WTO HS code for cross-border classification.
        /// Used to auto-map to jurisdiction-specific rules.
        /// </summary>
        [MaxLength(20)]
        public string? HsCode { get; set; }

        /// <summary>
        /// EU CN code (Combined Nomenclature) for EU VAT purposes.
        /// </summary>
        [MaxLength(20)]
        public string? EuCnCode { get; set; }

        /// <summary>
        /// Is this a digital/electronically supplied service?
        /// Critical: triggers customer-location place-of-supply rules under EU VAT 2015.
        /// </summary>
        public bool IsDigitalService { get; set; }

        /// <summary>Whether this category is a service (vs goods).</summary>
        public bool IsService { get; set; }

        public ICollection<TaxRate> Rates { get; set; } = new List<TaxRate>();
    }

    // ──────────────────────────────────────────────────────────
    // 4. TAX RATE
    // The actual percentage — fully historized with effective dates
    // ──────────────────────────────────────────────────────────

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
    public class TaxRate
    {
        [Key]
        public Guid Id { get; set; }

        public Guid TaxRegimeId { get; set; }
        public TaxRegime TaxRegime { get; set; } = default!;

        /// <summary>
        /// Null = applies to all product categories (default/catch-all rate).
        /// Non-null = applies only to this product category.
        /// Resolution: most-specific match wins.
        /// </summary>
        public Guid? TaxProductCategoryId { get; set; }
        public TaxProductCategory? TaxProductCategory { get; set; }

        public TaxRateCategory RateCategory { get; set; }

        /// <summary>
        /// Customer type scope. Null = applies to all customers.
        /// Use to define different rates for B2B vs B2C where relevant.
        /// </summary>
        public CustomerTaxStatus? ApplicableToCustomerStatus { get; set; }

        /// <summary>Rate as decimal: 0.20 = 20%, 0.05 = 5%, 0 = zero-rated.</summary>
        [Column(TypeName = "decimal(7,6)")]
        public decimal Rate { get; set; }

        /// <summary>
        /// Date from which this rate is legally effective.
        /// The query layer always filters: EffectiveFrom ≤ InvoiceDate ≤ EffectiveTo.
        /// </summary>
        public DateOnly EffectiveFrom { get; set; }

        /// <summary>9999-12-31 = open-ended (currently in force).</summary>
        public DateOnly EffectiveTo { get; set; } = new DateOnly(9999, 12, 31);

        /// <summary>Official legislative reference, e.g. "UK VATA 1994 s.29A Group 3"</summary>
        [MaxLength(500)]
        public string? LegalReference { get; set; }

        /// <summary>Internal note for compliance team.</summary>
        [MaxLength(1000)]
        public string? Notes { get; set; }

        public Guid CreatedByUserId { get; set; }
        public DateTime CreatedAt { get; set; }

        public bool IsActive { get; set; } = true;
    }

    // ──────────────────────────────────────────────────────────
    // 5. REGISTRATION THRESHOLD
    // When the business MUST register in a jurisdiction
    // ──────────────────────────────────────────────────────────

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
    public class TaxRegistrationThreshold
    {
        [Key]
        public Guid Id { get; set; }

        public Guid JurisdictionId { get; set; }
        public TaxJurisdiction Jurisdiction { get; set; } = default!;

        /// <summary>Revenue threshold in the jurisdiction's local currency.</summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal ThresholdAmount { get; set; }

        [Required, MaxLength(3)]
        public string CurrencyCode { get; set; } = default!;

        /// <summary>Rolling 12-month, calendar year, fiscal year?</summary>
        [MaxLength(50)]
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

    // ──────────────────────────────────────────────────────────
    // 6. BUSINESS TAX REGISTRATION
    // YOUR company's registration in each jurisdiction
    // ──────────────────────────────────────────────────────────

    /// <summary>
    /// Records that OUR BUSINESS is registered to collect tax
    /// in a specific jurisdiction under a specific regime.
    /// This drives the "do we charge tax?" decision before rate lookup.
    ///
    /// If no registration exists for a jurisdiction → we are NOT registered
    /// → we cannot legally charge/collect that tax.
    /// </summary>
    public class BusinessTaxRegistration
    {
        [Key]
        public Guid Id { get; set; }

        public Guid TaxRegimeId { get; set; }
        public TaxRegime TaxRegime { get; set; } = default!;

        /// <summary>
        /// Our VAT/GST registration number issued by the authority.
        /// E.g., "DE123456789", "AU51824753556", "GB123456789".
        /// </summary>
        [Required, MaxLength(50)]
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
        [Required, MaxLength(200)]
        public string LegalEntityName { get; set; } = default!;

        [MaxLength(500)]
        public string? LegalEntityAddress { get; set; }

        public bool IsActive { get; set; } = true;
    }

    // ──────────────────────────────────────────────────────────
    // 7. CUSTOMER TAX PROFILE
    // Tax-relevant attributes of each customer
    // ──────────────────────────────────────────────────────────

    /// <summary>
    /// Tax profile for a customer — separate from the CRM/billing customer record.
    /// A customer may have one tax profile globally, or different profiles
    /// per-jurisdiction (e.g., a US company that's VAT-registered in Germany).
    /// </summary>
    public class CustomerTaxProfile
    {
        [Key]
        public Guid Id { get; set; }

        /// <summary>FK to your CRM/Account entity.</summary>
        public Guid CustomerId { get; set; }

        public CustomerTaxStatus TaxStatus { get; set; }

        /// <summary>
        /// The jurisdiction where the customer is established / tax-resident.
        /// Drives place-of-supply for B2B cross-border.
        /// </summary>
        public Guid EstablishedJurisdictionId { get; set; }
        public TaxJurisdiction EstablishedJurisdiction { get; set; } = default!;

        /// <summary>
        /// Is this a consumer in an EU member state for VAT purposes?
        /// B2C in EU triggers OSS / destination-country tax rate.
        /// </summary>
        public bool IsEuConsumer { get; set; }

        public ICollection<CustomerTaxIdentifier> TaxIdentifiers { get; set; } = new List<CustomerTaxIdentifier>();
        public ICollection<CustomerTaxExemption> Exemptions { get; set; } = new List<CustomerTaxExemption>();
    }

    /// <summary>
    /// Tax registration numbers the customer holds (they may be VAT-registered
    /// in multiple countries as a multinational).
    /// </summary>
    public class CustomerTaxIdentifier
    {
        [Key]
        public Guid Id { get; set; }

        public Guid CustomerTaxProfileId { get; set; }
        public CustomerTaxProfile CustomerTaxProfile { get; set; } = default!;

        public Guid JurisdictionId { get; set; }
        public TaxJurisdiction Jurisdiction { get; set; } = default!;

        [Required, MaxLength(50)]
        public string TaxRegistrationNumber { get; set; } = default!;

        public TaxIdVerificationStatus VerificationStatus { get; set; }
        public DateTime? LastVerifiedAt { get; set; }

        /// <summary>Verification response from VIES / authority API — stored for audit.</summary>
        [MaxLength(2000)]
        public string? VerificationResponse { get; set; }

        public DateOnly ValidFrom { get; set; }
        public DateOnly ValidTo { get; set; } = new DateOnly(9999, 12, 31);
    }

    public enum TaxIdVerificationStatus
    {
        Pending,
        Valid,
        Invalid,
        UnverifiableApiDown,
        Expired
    }

    // ──────────────────────────────────────────────────────────
    // 8. TAX EXEMPTION CERTIFICATE
    // ──────────────────────────────────────────────────────────

    /// <summary>
    /// A customer's exemption from tax in a specific jurisdiction.
    /// Examples:
    ///   - US resale certificates (customer resells to end consumers)
    ///   - EU article 151 exemptions (diplomatic missions, NATO)
    ///   - Healthcare/charity exemptions
    ///   - US government/federal exemptions
    /// </summary>
    public class CustomerTaxExemption
    {
        [Key]
        public Guid Id { get; set; }

        public Guid CustomerTaxProfileId { get; set; }
        public CustomerTaxProfile CustomerTaxProfile { get; set; } = default!;

        public Guid JurisdictionId { get; set; }
        public TaxJurisdiction Jurisdiction { get; set; } = default!;

        /// <summary>Which product categories this exemption covers. Empty = all.</summary>
        public ICollection<TaxProductCategory> ApplicableCategories { get; set; } = new List<TaxProductCategory>();

        [Required, MaxLength(100)]
        public string ExemptionType { get; set; } = default!; // "Resale", "Government", "Charity", "Diplomatic"

        [Required, MaxLength(200)]
        public string CertificateNumber { get; set; } = default!;

        /// <summary>Scanned certificate document reference.</summary>
        [MaxLength(500)]
        public string? DocumentStorageKey { get; set; }

        public DateOnly IssuedDate { get; set; }
        public DateOnly ExpiryDate { get; set; }

        public bool IsActive { get; set; } = true;
    }

    // ──────────────────────────────────────────────────────────
    // 9. TAX DETERMINATION REQUEST / RESPONSE
    // Input and output of the tax engine at calculation time
    // ──────────────────────────────────────────────────────────

    /// <summary>
    /// Everything the tax engine needs to determine the applicable tax
    /// for a single invoice line.
    /// </summary>
    public class TaxDeterminationRequest
    {
        public Guid RequestId { get; set; } = Guid.NewGuid();

        /// <summary>Date of supply (tax point) — NOT the invoice date in many jurisdictions.</summary>
        public DateOnly TaxPointDate { get; set; }

        /// <summary>Where the supplier is established.</summary>
        public Guid SupplierJurisdictionId { get; set; }

        /// <summary>Where the customer is established / tax-resident.</summary>
        public Guid CustomerJurisdictionId { get; set; }

        /// <summary>The customer's billing / ship-to jurisdiction (may differ from establishment).</summary>
        public Guid? CustomerDeliveryJurisdictionId { get; set; }

        public Guid TaxProductCategoryId { get; set; }
        public Guid CustomerTaxProfileId { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal LineAmountExcludingTax { get; set; }

        [Required, MaxLength(3)]
        public string BillingCurrencyCode { get; set; } = default!;

        /// <summary>
        /// Exchange rate from billing currency to local tax currency.
        /// Tax authorities require reporting in local currency.
        /// </summary>
        [Column(TypeName = "decimal(18,8)")]
        public decimal ExchangeRateToLocalCurrency { get; set; }
    }

    /// <summary>
    /// The computed tax decision returned by the tax engine.
    /// Immutable once stored — represents what was decided at transaction time.
    /// </summary>
    public class TaxDeterminationResult
    {
        [Key]
        public Guid Id { get; set; }

        public Guid RequestId { get; set; }

        public Guid? AppliedTaxRateId { get; set; }
        public TaxRate? AppliedTaxRate { get; set; }

        public TaxRateCategory RateCategory { get; set; }
        public TaxLiability LiabilityType { get; set; }
        public PlaceOfSupplyRule PlaceOfSupplyRuleApplied { get; set; }

        /// <summary>Which jurisdiction is entitled to the tax revenue.</summary>
        public Guid TaxingJurisdictionId { get; set; }
        public TaxJurisdiction TaxingJurisdiction { get; set; } = default!;

        [Column(TypeName = "decimal(7,6)")]
        public decimal TaxRate { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal TaxableAmount { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal TaxAmountInBillingCurrency { get; set; }

        /// <summary>For the tax return: amount in jurisdiction's local currency.</summary>
        [Column(TypeName = "decimal(18,4)")]
        public decimal TaxAmountInLocalCurrency { get; set; }

        /// <summary>
        /// Was a customer exemption applied? Points to the certificate used.
        /// </summary>
        public Guid? AppliedExemptionId { get; set; }
        public CustomerTaxExemption? AppliedExemption { get; set; }

        /// <summary>
        /// Machine-readable reason code for why this tax treatment was chosen.
        /// Critical for audit: "B2B_REVERSE_CHARGE_EU_ARTICLE_44" etc.
        /// </summary>
        [Required, MaxLength(200)]
        public string ReasonCode { get; set; } = default!;

        [MaxLength(2000)]
        public string? ReasonNarrative { get; set; }

        public DateTime DeterminedAt { get; set; } = DateTime.UtcNow;

        /// <summary>Version of the tax engine logic that produced this result. For audit.</summary>
        [Required, MaxLength(20)]
        public string TaxEngineVersion { get; set; } = default!;
    }

    // ──────────────────────────────────────────────────────────
    // 10. INVOICE TAX LINE
    // Snapshot stored on the invoice — never recalculated after finalization
    // ──────────────────────────────────────────────────────────

    /// <summary>
    /// Persisted tax line on a finalized invoice.
    /// Once the invoice is issued, this record is IMMUTABLE.
    /// If a rate changes later, this record still holds the rate that was
    /// in force at the time of supply (TaxPointDate).
    ///
    /// An invoice line may have MULTIPLE tax records:
    ///   - Federal GST 5% + Provincial QST 9.975% for Quebec
    ///   - GST 10% + no additional for AU
    ///   - US: County 1% + City 0.5% + State 7.25%
    /// </summary>
    public class InvoiceTaxLine
    {
        [Key]
        public Guid Id { get; set; }

        public Guid InvoiceId { get; set; }
        public Guid InvoiceLineId { get; set; }

        /// <summary>Links to the immutable determination record.</summary>
        public Guid TaxDeterminationResultId { get; set; }
        public TaxDeterminationResult TaxDeterminationResult { get; set; } = default!;

        public Guid TaxRegimeId { get; set; }
        public TaxRegime TaxRegime { get; set; } = default!;

        /// <summary>Label printed on invoice: "VAT @ 20%", "GST @ 10%", "QST @ 9.975%"</summary>
        [Required, MaxLength(100)]
        public string DisplayLabel { get; set; } = default!;

        [Column(TypeName = "decimal(7,6)")]
        public decimal AppliedRate { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal TaxableAmount { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal TaxAmount { get; set; }

        [Required, MaxLength(3)]
        public string CurrencyCode { get; set; } = default!;

        /// <summary>
        /// For reverse charge: the customer's VAT number printed on invoice
        /// per legal requirement ("Reverse charge: customer VAT DE123456789").
        /// </summary>
        [MaxLength(50)]
        public string? CustomerVatNumberOnInvoice { get; set; }

        /// <summary>Legal note required on invoice, e.g. "VAT reverse charged to recipient per Art. 44 EU Dir 2006/112"</summary>
        [MaxLength(500)]
        public string? LegalNoteForInvoice { get; set; }

        /// <summary>Sequence: 1=federal, 2=state, 3=county, 4=city for US stack.</summary>
        public int SequenceOrder { get; set; } = 1;
    }

    // ──────────────────────────────────────────────────────────
    // 11. TAX PERIOD SUMMARY
    // Aggregate for filing returns with each authority
    // ──────────────────────────────────────────────────────────

    /// <summary>
    /// Aggregated tax data for a reporting period, by jurisdiction and regime.
    /// Used to generate VAT returns / GST returns / Sales Tax returns.
    /// One record per jurisdiction per filing period.
    /// </summary>
    public class TaxPeriodSummary
    {
        [Key]
        public Guid Id { get; set; }

        public Guid BusinessTaxRegistrationId { get; set; }
        public BusinessTaxRegistration BusinessTaxRegistration { get; set; } = default!;

        public DateOnly PeriodStartDate { get; set; }
        public DateOnly PeriodEndDate { get; set; }

        /// <summary>All amounts in the jurisdiction's local reporting currency.</summary>
        [Required, MaxLength(3)]
        public string ReportingCurrencyCode { get; set; } = default!;

        // ── Output tax (tax you collected from customers) ──
        [Column(TypeName = "decimal(18,2)")]
        public decimal StandardRatedSales { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal StandardRatedTaxCollected { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal ReducedRatedSales { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal ReducedRatedTaxCollected { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal ZeroRatedSales { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal ExemptSales { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal ReverseChargeSales { get; set; }   // B2B cross-border (output=0 for us)

        // ── Input tax (tax you paid to suppliers — only for VAT/GST regimes) ──
        [Column(TypeName = "decimal(18,2)")]
        public decimal InputTaxRecoverable { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal InputTaxNonRecoverable { get; set; }

        // ── Net payable / refundable ──
        [Column(TypeName = "decimal(18,2)")]
        public decimal NetTaxPayable { get; set; }       // Positive = you owe; Negative = refund

        public TaxReturnStatus ReturnStatus { get; set; }
        public DateTime? SubmittedAt { get; set; }
        public DateTime? PaidAt { get; set; }

        [MaxLength(200)]
        public string? AuthorityReferenceNumber { get; set; }
    }

    public enum TaxReturnStatus
    {
        Draft,
        UnderReview,
        Submitted,
        Accepted,
        Rejected,
        Amended,
        Paid
    }

    // ──────────────────────────────────────────────────────────
    // 12. TAX RATE CHANGE AUDIT LOG
    // Immutable append-only log for regulatory compliance
    // ──────────────────────────────────────────────────────────

    /// <summary>
    /// Every change to tax rates, registrations, or customer exemptions
    /// is recorded here. Tax authorities may audit 7 years back.
    /// Never delete or update records in this table.
    /// </summary>
    public class TaxAuditLog
    {
        [Key]
        public Guid Id { get; set; }

        [Required, MaxLength(100)]
        public string EntityType { get; set; } = default!; // "TaxRate", "CustomerTaxExemption", etc.

        public Guid EntityId { get; set; }

        [Required, MaxLength(50)]
        public string Action { get; set; } = default!; // "Created", "Superseded", "VerificationSucceeded"

        [MaxLength(4000)]
        public string? BeforeStateJson { get; set; }

        [MaxLength(4000)]
        public string? AfterStateJson { get; set; }

        [MaxLength(500)]
        public string? ChangeReason { get; set; }

        public Guid ChangedByUserId { get; set; }
        public DateTime ChangedAt { get; set; } = DateTime.UtcNow;

        /// <summary>IP address of the request origin — required by some tax authorities.</summary>
        [MaxLength(45)]
        public string? RequestIpAddress { get; set; }
    }
}

// ============================================================
// USAGE EXAMPLE — Tax Engine Resolution Logic (pseudocode)
// ============================================================
/*

STEP 1 — Determine place of supply (which jurisdiction gets the tax)
  if (product.IsDigitalService && customer.IsB2C)
      → customer's jurisdiction (EU VAT 2015, OECD guidelines)
  else if (customer.IsB2BVerified && crossBorder)
      → customer's jurisdiction → reverse charge applies
  else
      → supplier's jurisdiction

STEP 2 — Is our business registered in the taxing jurisdiction?
  if (!businessRegistrations.Any(r => r.JurisdictionId == taxingJurisdiction))
      → check if we exceed the threshold for that jurisdiction
      → if below threshold: OutOfScope, no tax
      → if above threshold: ALERT — registration required

STEP 3 — Apply customer exemptions
  if (customer.Exemptions.Any(e => e.JurisdictionId == taxingJurisdiction
                                && e.ExpiryDate >= taxPointDate
                                && e.CoversCategory(productCategory)))
      → TaxRateCategory = Exempt, Rate = 0
      → store certificate reference for audit

STEP 4 — Resolve applicable tax rate
  rates = TaxRates
      .Where(r => r.TaxRegimeId == regime.Id
               && (r.TaxProductCategoryId == productCategory.Id
                   || r.TaxProductCategoryId == null)   // null = default/catch-all
               && r.EffectiveFrom <= taxPointDate
               && r.EffectiveTo >= taxPointDate
               && r.IsActive)
      .OrderByDescending(r => r.TaxProductCategoryId.HasValue)  // specific > catch-all
      .ThenByDescending(r => r.EffectiveFrom)                   // newest first
      .First()

STEP 5 — Handle compound/stacked jurisdictions (CA, US)
  for each level in jurisdictionHierarchy.Where(j => j.IsActive):
      resolve a TaxRate for that level
      sum all rates → composite effective rate

STEP 6 — Compute and persist
  taxAmount = lineAmount * rate.Rate
  TaxDeterminationResult { rate, jurisdiction, liability, placeOfSupplyRule, reasonCode }
  InvoiceTaxLine { displayLabel, amount, legalNote }

KEY AUDIT RULE:
  - TaxDeterminationResult is WRITE-ONCE. Never update after invoice is issued.
  - If a credit note / reversal: create a new negative TaxDeterminationResult
    referencing the original, at the ORIGINAL rate (not the current rate).

*/
