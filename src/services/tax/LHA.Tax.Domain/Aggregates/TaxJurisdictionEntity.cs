using LHA.Core;
using LHA.Ddd.Domain;
using LHA.Shared.Domain;

namespace LHA.Tax.Domain.Aggregates
{
    /// <summary>
    /// Represents a taxing authority territory at any level of the hierarchy.
    /// Self-referencing tree allows: US → California → San Francisco.
    /// A single transaction may traverse multiple jurisdiction levels
    /// (GST federal + QST provincial in Canada).
    /// </summary>
    public class TaxJurisdictionEntity : FullAuditedAggregateRoot<Guid>
    {
        /// <summary>ISO 3166-1 alpha-2 for Country, ISO 3166-2 for subdivisions.</summary>
        public string Code { get; set; } = default!;

        public string Name { get; set; } = default!;

        public CJurisdictionLevel Level { get; set; }

        /// <summary>
        /// Which tax regime governs this jurisdiction.
        /// A country may have multiple: e.g., Canada has GST (federal) + PST/QST (provincial).
        /// Each TaxRegime record points back here.
        /// </summary>
        public CTaxRegimeType PrimaryRegime { get; set; }

        /// <summary>Parent in the hierarchy tree.</summary>
        public Guid? ParentJurisdictionId { get; set; }

        public TaxJurisdictionEntity? ParentJurisdiction { get; set; }

        /// <summary>
        /// ISO 4217 currency code for local tax reporting.
        /// Tax amounts must also be stored in this currency for returns.
        /// </summary>
        public string LocalCurrencyCode { get; set; } = default!;

        /// <summary>
        /// IANA timezone — needed to determine tax period (Dec 31 23:59 UTC may be Jan 1 in AU).
        /// </summary>
        public string Timezone { get; set; } = default!;

        /// <summary>
        /// Some jurisdictions are compound: apply both this level AND parent level.
        /// E.g., Quebec: federal GST 5% AND QST 9.975%.
        /// </summary>
        public bool IsCompoundWithParent { get; set; }

        public CMasterStatus Status { get; set; } = CMasterStatus.Active;

        public ICollection<TaxJurisdictionEntity> Children { get; set; } = new List<TaxJurisdictionEntity>();
        public ICollection<Configuration.TaxRegimeEntity> TaxRegimes { get; set; } = new List<Configuration.TaxRegimeEntity>();
        public ICollection<Configuration.TaxRegistrationThresholdEntity> Thresholds { get; set; } = new List<Configuration.TaxRegistrationThresholdEntity>();
    }
}
