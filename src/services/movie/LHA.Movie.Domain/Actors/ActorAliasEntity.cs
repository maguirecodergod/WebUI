using System.Collections.ObjectModel;
using LHA.Ddd.Domain;
using LHA.MultiTenancy;
using LHA.Shared.Domain;

namespace LHA.Movie.Domain.Actors
{
    /// <summary>
    /// Thông tin thay thế của diễn viên
    /// </summary>
    public class ActorAliasEntity : FullAuditedEntity<Guid>, IMultiTenant
    {
        /// <summary>
        /// Constructor for EF Core
        /// </summary>
        protected ActorAliasEntity() { }

        /// <summary>
        /// Ngày bắt đầu có hiệu lực
        /// </summary>
        public DateOnly? EffectiveFrom { get; private set; }

        /// <summary>
        /// Ngày hết hiệu lực
        /// </summary>
        public DateOnly? EffectiveTo { get; private set; }

        /// <summary>
        /// Ảnh đại diện
        /// </summary>
        public string? Avatar { get; set; }

        /// <summary>
        /// Ảnh bìa
        /// </summary>
        public string? Banner { get; set; }

        /// <summary>
        /// Danh sách ảnh thumbnail
        /// </summary>
        public List<string> Thumbnails { get; set; } = new List<string>();

        /// <summary>
        /// Vòng ngực
        /// </summary>
        public double Bust { get; private set; }
        /// <summary>
        /// Vòng chân ngực
        /// </summary>
        public double UnderBust { get; private set; }
        /// <summary>
        /// Vòng eo
        /// </summary>
        public double Waist { get; private set; }
        /// <summary>
        /// Vòng mông
        /// </summary>
        public double Hip { get; private set; }

        /// <summary>
        /// Kích thước cúp ngực
        /// Cup size tính toán từ Bust - UnderBust
        /// </summary>
        public CCupSizeType CupSize => CalculateCupSize(Bust, UnderBust);

        /// <summary>
        /// Kích thước band ngực
        /// </summary>
        public int BandSize => (int)Math.Round(UnderBust);

        /// <summary>
        /// Loại cơ thể / đặc điểm tự nhiên hoặc can thiệp thẩm mỹ
        /// </summary>
        public CBodyType BodyType { get; private set; }

        /// <summary>
        /// Có phải là ảnh chính không
        /// </summary>
        public bool IsPrimary { get; private set; } = false;

        /// <summary>
        /// Id của tenant
        /// </summary>
        public Guid? TenantId { get; set; } = null;

        /// <summary>
        /// Id của diễn viên
        /// </summary>
        public Guid ActorId { get; private set; }

        /// <summary>
        /// Diễn viên
        /// </summary>
        public virtual ActorEntity Actor { get; private set; } = null!;

        /// <summary>
        /// Danh sách thông tin dịch của tên thay thế
        /// </summary>
        public virtual ICollection<ActorAliasTranslationEntity> Translations { get; private set; } = new Collection<ActorAliasTranslationEntity>();

        internal void SetBust(double bust)
        {
            Bust = bust;
        }

        internal void SetUnderBust(double underBust)
        {
            UnderBust = underBust;
        }

        internal void SetWaist(double waist)
        {
            Waist = waist;
        }

        internal void SetHip(double hip)
        {
            Hip = hip;
        }


        #region extra method
        private static CCupSizeType CalculateCupSize(double bust, double underBust)
        {
            double diff = bust - underBust;

            if (diff < 10) return CCupSizeType.AA;    // Nhỏ hơn A
            if (diff < 12.5) return CCupSizeType.A;   // ~A
            if (diff < 15) return CCupSizeType.B;     // ~B
            if (diff < 17.5) return CCupSizeType.C;   // ~C
            if (diff < 20) return CCupSizeType.D;     // ~D
            if (diff < 22.5) return CCupSizeType.DD;  // ~DD (tương đương E)
            if (diff < 25) return CCupSizeType.E;     // ~E
            if (diff < 27.5) return CCupSizeType.F;   // ~F
            if (diff < 30) return CCupSizeType.FF;    // ~FF (tương đương G)
            if (diff < 32.5) return CCupSizeType.G;   // ~G
            if (diff < 35) return CCupSizeType.GG;    // ~GG (tương đương H)
            if (diff < 37.5) return CCupSizeType.H;   // ~H
            if (diff < 40) return CCupSizeType.HH;    // ~HH (tương đương I)
            if (diff < 42.5) return CCupSizeType.I;   // ~I
            if (diff < 45) return CCupSizeType.J;     // ~J
            if (diff < 47.5) return CCupSizeType.K;   // ~K
            if (diff < 50) return CCupSizeType.L;     // ~L
            return CCupSizeType.M;                     // >50 → M
        }
        #endregion
    }
}