namespace LHA.Shared.Domain
{
    public enum CJurisdictionLevel
    {
        /// <summary>
        /// Placeholder root
        /// </summary>
        Global = 0,
        /// <summary>
        /// EU, GCC bloc
        /// </summary>
        Regional = 1,
        /// <summary>
        /// DE, AU, US
        /// </summary>
        Country = 2,
        /// <summary>
        /// Bavaria, Queensland, California
        /// </summary>
        State = 3,
        /// <summary>
        /// US county
        /// </summary>
        County = 4,
        /// <summary>
        /// City / Municipality
        /// </summary>
        City = 5
    }
}