namespace LHA.Shared.Domain
{
    public enum CTaxRegimeType
    {
        /// <summary>
        /// Value-Added Tax (EU, UK, most of world)
        /// </summary>
        VAT = 1,
        /// <summary>
        /// Goods and Services Tax (AU, NZ, IN, CA federal)
        /// </summary>
        GST = 2,
        /// <summary>
        /// Sales Tax (US-style: single-stage, no input credit)
        /// </summary>
        SalesTax = 3,
        /// <summary>
        /// Consumption Tax (Japan JCT)
        /// </summary>
        ConsumptionTax = 4,
        /// <summary>
        /// Excise Duty (Alcohol, tobacco, fuel – layered on top)
        /// </summary>
        ExciseDuty = 5,
        /// <summary>
        /// Withholding Tax (Some APAC service payments)
        /// </summary>
        WithholdingTax = 6,
        /// <summary>
        /// No indirect tax (Bahrain free zones, etc.)
        /// </summary>
        None = 0
    }
}