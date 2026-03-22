namespace LHA.Shared.Domain
{
    /// <summary>
    /// Loại cơ thể / đặc điểm tự nhiên hoặc can thiệp thẩm mỹ
    /// Áp dụng cho tất cả người
    /// </summary>
    public enum CBodyType
    {
        /// <summary>
        /// Hoàn toàn tự nhiên, không can thiệp thẩm mỹ
        /// </summary>
        Natural = 0,

        /// <summary>
        /// Có can thiệp phẫu thuật / thẩm mỹ
        /// </summary>
        Surgical = 1,

        /// <summary>
        /// Không xác định / không rõ
        /// </summary>
        Unknown = 99
    }
}