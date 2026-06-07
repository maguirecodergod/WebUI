namespace LHA.Shared.Domain
{
    public enum CJurisdictionLevel
    {
        /// <summary>
        /// 0 - Global: Placeholder root
        /// </summary>
        Global = 0,
        /// <summary>
        /// 1 - Regional: EU, GCC bloc
        /// </summary>
        Regional = 1,
        /// <summary>
        /// 2 - Country: DE, AU, US
        /// </summary>
        Country = 2,
        /// <summary>
        /// 3 - State: Bavaria, Queensland, California
        /// </summary>
        State = 3,
        /// <summary>
        /// 4 - County: US county
        /// </summary>
        County = 4,
        /// <summary>
        /// 5 - City: City / Municipality
        /// </summary>
        City = 5
    }
}