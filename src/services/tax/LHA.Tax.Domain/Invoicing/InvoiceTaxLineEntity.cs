using LHA.Ddd.Domain;

namespace LHA.Tax.Domain.Invoicing
{
    public class InvoiceTaxLineEntity : FullAuditedEntity<Guid>
    {
        public Guid InvoiceId { get; set; }
        public Guid InvoiceLineId { get; set; }

        /// <summary>Links to the immutable determination record.</summary>
        public Guid TaxDeterminationResultId { get; set; }
        public virtual Determination.TaxDeterminationResultEntity TaxDeterminationResult { get; set; } = default!;

        public Guid TaxRegimeId { get; set; }
        public virtual Configuration.TaxRegimeEntity TaxRegime { get; set; } = default!;

        /// <summary>Label printed on invoice: "VAT @ 20%", "GST @ 10%", "QST @ 9.975%"</summary>
        public string DisplayLabel { get; set; } = default!;

        public decimal AppliedRate { get; set; }

        public decimal TaxableAmount { get; set; }

        public decimal TaxAmount { get; set; }

        public string CurrencyCode { get; set; } = default!;

        /// <summary>
        /// For reverse charge: the customer's VAT number printed on invoice
        /// per legal requirement ("Reverse charge: customer VAT DE123456789").
        /// </summary>
        public string? CustomerVatNumberOnInvoice { get; set; }

        /// <summary>Legal note required on invoice, e.g. "VAT reverse charged to recipient per Art. 44 EU Dir 2006/112"</summary>
        public string? LegalNoteForInvoice { get; set; }

        /// <summary>Sequence: 1=federal, 2=state, 3=county, 4=city for US stack.</summary>
        public int SequenceOrder { get; set; } = 1;
    }
}
