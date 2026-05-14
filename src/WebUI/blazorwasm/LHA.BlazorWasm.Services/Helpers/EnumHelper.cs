using System.Reflection;
using LHA.Shared.Domain.Attributes;
using LHA.BlazorWasm.Shared.Abstractions.Localization;

namespace LHA.BlazorWasm.Services.Helpers
{
    public static class EnumHelper
    {
        public static string GetDisplayName<TEnum>(this TEnum enumValue, ILocalizationService? localizer = null) where TEnum : Enum
        {
            var type = typeof(TEnum);
            var memberInfo = type.GetMember(enumValue.ToString()).FirstOrDefault();
            if (memberInfo == null) return enumValue.ToString();

            var attribute = memberInfo.GetCustomAttribute<EnumMetadataAttribute>();
            if (attribute != null && !string.IsNullOrEmpty(attribute.DisplayName))
            {
                return localizer != null ? localizer.L(attribute.DisplayName) : attribute.DisplayName;
            }

            return enumValue.ToString();
        }

        public static string GetDescription<TEnum>(this TEnum enumValue, ILocalizationService? localizer = null) where TEnum : Enum
        {
            var type = typeof(TEnum);
            var memberInfo = type.GetMember(enumValue.ToString()).FirstOrDefault();
            if (memberInfo == null) return string.Empty;

            var attribute = memberInfo.GetCustomAttribute<EnumMetadataAttribute>();
            if (attribute == null || string.IsNullOrEmpty(attribute.Description)) return string.Empty;

            return localizer != null ? localizer.L(attribute.Description) : attribute.Description;
        }

        public static string GetIcon<TEnum>(this TEnum enumValue) where TEnum : Enum
        {
            var type = typeof(TEnum);
            var memberInfo = type.GetMember(enumValue.ToString()).FirstOrDefault();
            if (memberInfo == null) return string.Empty;

            var attribute = memberInfo.GetCustomAttribute<EnumMetadataAttribute>();
            return attribute?.Icon ?? string.Empty;
        }

        public static string GetCategory<TEnum>(this TEnum enumValue, ILocalizationService? localizer = null) where TEnum : Enum
        {
            var type = typeof(TEnum);
            var memberInfo = type.GetMember(enumValue.ToString()).FirstOrDefault();
            if (memberInfo == null) return string.Empty;

            var attribute = memberInfo.GetCustomAttribute<EnumMetadataAttribute>();
            if (attribute == null || string.IsNullOrEmpty(attribute.Category)) return string.Empty;

            return localizer != null ? localizer.L(attribute.Category) : attribute.Category;
        }

        public static IEnumerable<EnumItem<TEnum>> GetAll<TEnum>(ILocalizationService? localizer = null) where TEnum : Enum
        {
            return Enum.GetValues(typeof(TEnum))
                .Cast<TEnum>()
                .Select(e => new EnumItem<TEnum>
                {
                    Value = e,
                    DisplayName = e.GetDisplayName(localizer),
                    Description = e.GetDescription(localizer),
                    Icon = e.GetIcon(),
                    Category = e.GetCategory(localizer)
                });
        }
    }

    public class EnumItem<TEnum> where TEnum : Enum
    {
        public TEnum? Value { get; set; }
        public string? DisplayName { get; set; }
        public string? Description { get; set; }
        public string? Icon { get; set; }
        public string? Category { get; set; }
    }
}