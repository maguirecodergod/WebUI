using LHA.Ddd.Domain;

namespace LHA.Tax.Domain.Determination
{
    /// <summary>
    /// Everything the tax engine needs to determine the applicable tax
    /// for a single invoice line.
    /// </summary>
    public class TaxDeterminationRequestEntity : FullAuditedEntity<Guid>
    {
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

        public decimal LineAmountExcludingTax { get; set; }

        public string BillingCurrencyCode { get; set; } = default!;

        /// <summary>
        /// Exchange rate from billing currency to local tax currency.
        /// Tax authorities require reporting in local currency.
        /// </summary>
        public decimal ExchangeRateToLocalCurrency { get; set; }
    }
}
