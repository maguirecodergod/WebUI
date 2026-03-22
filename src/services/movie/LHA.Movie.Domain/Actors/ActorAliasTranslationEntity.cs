using LHA.Ddd.Domain;
using LHA.MultiTenancy;
using LHA.Shared.Domain;

namespace LHA.Movie.Domain.Actors
{
    /// <summary>
    /// Thông tin dịch của tên thay thế
    /// </summary>
    public class ActorAliasTranslationEntity : FullAuditedEntity<Guid>, IMultiTenant
    {
        /// <summary>
        /// Constructor for EF Core
        /// </summary>
        protected ActorAliasTranslationEntity() { }

        /// <summary>
        /// Tên thay thế
        /// </summary>
        public string Name { get; private set; } = null!;

        /// <summary>
        /// Mô tả
        /// </summary>
        public string? Description { get; private set; }

        /// <summary>
        /// Loại ngôn ngữ
        /// </summary>
        public CLanguageType LanguageType { get; private set; }

        /// <summary>
        /// Id của tên thay thế
        /// </summary>
        public Guid ActorAliasId { get; private set; }

        /// <summary>
        /// Id của tenant
        /// </summary>
        public Guid? TenantId { get; set; } = null;
    }
}