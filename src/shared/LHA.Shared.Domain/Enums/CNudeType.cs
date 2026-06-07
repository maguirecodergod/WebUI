namespace LHA.Shared.Domain.Enums
{
    public enum CNudeType
    {
        /// <summary>
        /// 0 - None: Không xác định
        /// </summary>
        None = 0,
        /// <summary>
        /// 1 - Uncensored: Không che
        /// </summary>
        Uncensored = 1,
        /// <summary>
        /// 2 - Censored: Có che
        /// </summary>
        Censored = 2,
        /// <summary>
        /// 3 - SemiNude: Bán nude
        /// </summary>
        SemiNude = 3,
        /// <summary>
        /// 4 - FullBody: Toàn thân
        /// </summary>
        FullBody = 4
    }
}