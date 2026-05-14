namespace LHA.Shared.Domain.Attributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class EnumMetadataAttribute : Attribute
    {
        /// <summary>
        /// Tên hiển thị của enum
        /// </summary>
        public string DisplayName { get; set; } = string.Empty;
        /// <summary>
        /// Mô tả của enum
        /// </summary>
        public string Description { get; set; } = string.Empty;
        /// <summary>
        /// Icon của enum
        /// </summary>
        public string Icon { get; set; } = string.Empty;
        /// <summary>
        /// Nhóm của enum
        /// </summary>
        public string Category { get; set; } = string.Empty;

        public EnumMetadataAttribute(
            string displayName = "",
            string description = "",
            string icon = "",
            string category = "")
        {
            DisplayName = displayName;
            Description = description;
            Icon = icon;
            Category = category;
        }
    }
}