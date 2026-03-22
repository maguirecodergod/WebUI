using System.Collections.ObjectModel;
using LHA.Ddd.Domain;
using LHA.MultiTenancy;
using LHA.Shared.Domain;

namespace LHA.Movie.Domain.Actors
{
    public class ActorEntity : FullAuditedAggregateRoot<Guid>, IMultiTenant
    {
        /// <summary>
        /// Constructor for EF Core
        /// </summary>
        protected ActorEntity() { }

        /// <summary>
        /// Giới tính
        /// </summary>
        public CSexType Sex { get; private set; }

        /// <summary>
        /// Ngày sinh
        /// </summary>
        public DateOnly? BirthDate { get; private set; }

        /// <summary>
        /// Ngày nghỉ hưu
        /// </summary>
        public DateOnly? RetirementDate { get; private set; } = null;

        /// <summary>
        /// Nhóm máu
        /// </summary>
        public CBloodType BloodType { get; private set; }

        /// <summary>
        /// Quốc tịch
        /// </summary>
        public CCountryType Nationality { get; private set; }

        /// <summary>
        /// Địa chỉ hiện tại
        /// </summary>
        public string? CurrentAddress { get; private set; }

        /// <summary>
        /// Id của tenant
        /// </summary>
        public Guid? TenantId { get; set; } = null;

        /// <summary>
        /// Danh sách các tên thay thế của diễn viên
        /// </summary>
        public virtual ICollection<ActorAliasEntity> ActorAliases { get; private set; } = new Collection<ActorAliasEntity>();
    }
}