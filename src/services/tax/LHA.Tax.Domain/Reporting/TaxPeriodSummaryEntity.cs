using LHA.Ddd.Domain;
using LHA.Shared.Domain;

namespace LHA.Tax.Domain.Reporting
{
    /// <summary>
    /// Aggregated tax data for a reporting period, by jurisdiction and regime.
    /// Used to generate VAT returns / GST returns / Sales Tax returns.
    /// One record per jurisdiction per filing period.
    /// </summary>
    public class TaxPeriodSummaryEntity : FullAuditedEntity<Guid>
    {
        public Guid BusinessTaxRegistrationId { get; set; }
        public virtual BusinessRegistrations.BusinessTaxRegistrationEntity BusinessTaxRegistration { get; set; } = default!;

        public DateOnly PeriodStartDate { get; set; }
        public DateOnly PeriodEndDate { get; set; }

        /// <summary>All amounts in the jurisdiction's local reporting currency.</summary>
        public string ReportingCurrencyCode { get; set; } = default!;

        // ── Output tax (tax you collected from customers) ──
        public decimal StandardRatedSales { get; set; }
        public decimal StandardRatedTaxCollected { get; set; }

        public decimal ReducedRatedSales { get; set; }
        public decimal ReducedRatedTaxCollected { get; set; }

        public decimal ZeroRatedSales { get; set; }

        public decimal ExemptSales { get; set; }

        public decimal ReverseChargeSales { get; set; }   // B2B cross-border (output=0 for us)

        // ── Input tax (tax you paid to suppliers — only for VAT/GST regimes) ──
        public decimal InputTaxRecoverable { get; set; }
        public decimal InputTaxNonRecoverable { get; set; }

        // ── Net payable / refundable ──
        public decimal NetTaxPayable { get; set; }       // Positive = you owe; Negative = refund

        public CTaxReturnStatus ReturnStatus { get; set; }
        public DateTimeOffset? SubmittedAt { get; set; }
        public DateTimeOffset? PaidAt { get; set; }

        public string? AuthorityReferenceNumber { get; set; }
    }
}
