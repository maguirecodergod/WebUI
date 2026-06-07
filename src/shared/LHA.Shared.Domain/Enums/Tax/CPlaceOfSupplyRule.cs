namespace LHA.Shared.Domain
{
    /// <summary>
    /// Place-of-supply rule that determined the taxing jurisdiction.
    /// Audit trail for which rule was applied at the time of supply.
    /// </summary>
    public enum CPlaceOfSupplyRule
    {
        /// <summary>
        /// 1 - SupplierLocation: Default for physical goods
        /// </summary>
        SupplierLocation = 1,
        /// <summary>
        /// 2 - CustomerLocation: Digital services (EU VAT 2015, OECD)
        /// </summary>
        CustomerLocation = 2,
        /// <summary>
        /// 3 - ServicePerformed: Restaurant, live events
        /// </summary>
        ServicePerformed = 3,
        /// <summary>
        /// 4 - PropertyLocation: Real estate
        /// </summary>
        PropertyLocation = 4,
        /// <summary>
        /// 5 - DeparturePoint: Passenger transport
        /// </summary>
        DeparturePoint = 5,
        /// <summary>
        /// 6 - OSSRegistration: EU One-Stop-Shop for B2C digital below threshold
        /// </summary>
        OSSRegistration = 6,
        /// <summary>
        /// 7 - FixedEstablishment: Where the customer has a fixed establishment
        /// </summary>
        FixedEstablishment = 7
    }
}