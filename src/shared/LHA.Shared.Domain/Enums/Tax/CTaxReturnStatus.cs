namespace LHA.Shared.Domain
{
    public enum CTaxReturnStatus
    {
        /// <summary>
        /// 0 - Draft
        /// </summary>
        Draft = 0,
        /// <summary>
        /// 1 - UnderReview
        /// </summary>
        UnderReview = 1,
        /// <summary>
        /// 2 - Submitted
        /// </summary>
        Submitted = 2,
        /// <summary>
        /// 3 - Accepted
        /// </summary>
        Accepted = 3,
        /// <summary>
        /// 4 - Rejected
        /// </summary>
        Rejected = 4,
        /// <summary>
        /// 5 - Amended
        /// </summary>
        Amended = 5,
        /// <summary>
        /// 6 - Paid
        /// </summary>
        Paid = 6
    }
}