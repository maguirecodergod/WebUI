namespace LHA.BlazorWasm.Shared.Models
{
    /// <summary>
    /// Yes/No enum used across all entities to indicate binary status.
    /// </summary>
    public enum CYesNo
    {
        /// <summary>
        /// 0 - Yes
        /// </summary>
        [StatusBadge(CBadgeStyle.Success, Variant = CBadgeVariant.Soft, Icon = "bi bi-check-lg")]
        Yes,

        /// <summary>
        /// 1 - No
        /// </summary>
        [StatusBadge(CBadgeStyle.Muted, Variant = CBadgeVariant.Soft, Icon = "bi bi-x-lg")]
        No
    }
}