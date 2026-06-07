namespace LHA.Tax.EntityFrameworkCore.Configurations;

/// <summary>
/// Column length constants for Tax entity properties.
/// Used by <see ref="IEntityTypeConfiguration{TEntity}"/> classes.
/// </summary>
internal static class TaxDbConsts
{
    // ─── General ────────────────────────────────────────────
    public const int MaxCodeLength = 128;
    public const int MaxNameLength = 256;
    public const int MaxLocalNameLength = 256;
    public const int MaxDescriptionLength = 1024;
    public const int MaxCurrencyCodeLength = 3;
    public const int MaxTimezoneLength = 64;
    public const int MaxUrlLength = 2048;
    public const int MaxRegimeTypeLength = 32;
    public const int MaxFilingFrequencyLength = 32;
    public const int MaxLegalReferenceLength = 512;
    public const int MaxNotesLength = 2048;
    public const int MaxReasonCodeLength = 256;
    public const int MaxReasonNarrativeLength = 1024;
    public const int MaxEngineVersionLength = 64;
    public const int MaxMeasurementPeriodLength = 32;

    // ─── Jurisdiction ───────────────────────────────────────
    public const int MaxJurisdictionCodeLength = 10;
    public const int MaxLocalCurrencyCodeLength = 3;

    // ─── Registration ───────────────────────────────────────
    public const int MaxRegistrationNumberLength = 64;
    public const int MaxRegistrationNumberFormatLength = 256;
    public const int MaxLegalEntityNameLength = 256;
    public const int MaxLegalEntityAddressLength = 1024;

    // ─── Product Category ───────────────────────────────────
    public const int MaxHsCodeLength = 32;
    public const int MaxEuCnCodeLength = 32;

    // ─── Invoice ────────────────────────────────────────────
    public const int MaxDisplayLabelLength = 128;
    public const int MaxCustomerVatNumberLength = 64;
    public const int MaxLegalNoteLength = 512;

    // ─── Exemption ──────────────────────────────────────────
    public const int MaxExemptionTypeLength = 64;
    public const int MaxCertificateNumberLength = 128;
    public const int MaxDocumentStorageKeyLength = 512;

    // ─── Tax Period Summary ─────────────────────────────────
    public const int MaxAuthorityReferenceLength = 128;

    // ─── Verification ───────────────────────────────────────
    public const int MaxVerificationResponseLength = 4096;
}
