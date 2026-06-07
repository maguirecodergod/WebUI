namespace LHA.Shared.Domain
{
    public enum CTaxRegimeType
    {
        /// <summary>
        /// 1 - VAT: Value-Added Tax (EU, UK, most of world)
        /// </summary>
        VAT = 1,
        /// <summary>
        /// 2 - GST: Goods and Services Tax (AU, NZ, IN, CA federal)
        /// </summary>
        GST = 2,
        /// <summary>
        /// 3 - SalesTax: Sales Tax (US-style: single-stage, no input credit)
        /// </summary>
        SalesTax = 3,
        /// <summary>
        /// 4 - ConsumptionTax: Consumption Tax (Japan JCT)
        /// </summary>
        ConsumptionTax = 4,
        /// <summary>
        /// 5 - ExciseDuty: Excise Duty (Alcohol, tobacco, fuel – layered on top)
        /// </summary>
        ExciseDuty = 5,
        /// <summary>
        /// 6 - WithholdingTax: Withholding Tax (Some APAC service payments)
        /// </summary>
        WithholdingTax = 6,
        /// <summary>
        /// 0 - None: No indirect tax (Bahrain free zones, etc.)
        /// </summary>
        None = 0
    }
}