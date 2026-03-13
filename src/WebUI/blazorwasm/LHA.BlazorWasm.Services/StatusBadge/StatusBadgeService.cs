using System.Collections.Concurrent;
using System.Reflection;
using Microsoft.Extensions.Localization;
using LHA.BlazorWasm.Shared.Models.StatusBadge;

namespace LHA.BlazorWasm.Services.StatusBadge;

public class StatusBadgeService : IStatusBadgeService
{
    private readonly ConcurrentDictionary<Type, Dictionary<object, StatusBadgeMetadata>> _manualMappings = new();
    private readonly ConcurrentDictionary<(Type, object), StatusBadgeMetadata> _resolvedCache = new();
    private readonly IStringLocalizerFactory? _localizerFactory;

    public StatusBadgeService(IStringLocalizerFactory? localizerFactory = null)
    {
        _localizerFactory = localizerFactory;
    }

    public void Register<TEnum>(Action<StatusBadgeMappingBuilder<TEnum>> builderAction) where TEnum : struct, Enum
    {
        var builder = new StatusBadgeMappingBuilder<TEnum>();
        builderAction(builder);
        
        var mappings = builder.Build();
        var typeMappings = new Dictionary<object, StatusBadgeMetadata>();
        
        foreach (var kvp in mappings)
        {
            typeMappings[kvp.Key] = kvp.Value;
        }

        _manualMappings[typeof(TEnum)] = typeMappings;
        // Invalidate cache for this type
        foreach (var key in _resolvedCache.Keys)
        {
            if (key.Item1 == typeof(TEnum)) _resolvedCache.TryRemove(key, out _);
        }
    }

    public StatusBadgeMetadata GetMetadata<TEnum>(TEnum value) where TEnum : struct, Enum
    {
        return _resolvedCache.GetOrAdd((typeof(TEnum), value), _ => ResolveMetadata(value));
    }

    private StatusBadgeMetadata ResolveMetadata<TEnum>(TEnum value) where TEnum : struct, Enum
    {
        // 1. Check manual mappings (highest priority/override)
        if (_manualMappings.TryGetValue(typeof(TEnum), out var typeMappings) && 
            typeMappings.TryGetValue(value, out var metadata))
        {
            return ApplyDefaults(value, metadata);
        }

        // 2. Check for StatusBadgeAttribute on the enum field
        var memInfo = typeof(TEnum).GetMember(value.ToString());
        var attribute = memInfo[0].GetCustomAttribute<StatusBadgeAttribute>();
        
        if (attribute != null)
        {
            return ApplyDefaults(value, new StatusBadgeMetadata
            {
                Style = attribute.Style,
                Variant = attribute.Variant,
                Icon = attribute.Icon,
                IsPulse = attribute.IsPulse,
                IsPill = attribute.IsPill,
                Tooltip = attribute.Tooltip
            });
        }

        // 3. Check for Global Convention (Optional: e.g. names containing "Success", "Error")
        var conventionMetadata = ResolveByConvention(value.ToString());
        if (conventionMetadata != null)
        {
            return ApplyDefaults(value, conventionMetadata);
        }

        // 4. Default Fallback
        return ApplyDefaults(value, new StatusBadgeMetadata { Style = BadgeStyle.Secondary });
    }

    private StatusBadgeMetadata ApplyDefaults<TEnum>(TEnum value, StatusBadgeMetadata metadata) where TEnum : struct, Enum
    {
        if (string.IsNullOrEmpty(metadata.Text))
        {
            metadata.Text = GetLocalizedText(value);
        }
        return metadata;
    }

    private StatusBadgeMetadata? ResolveByConvention(string name)
    {
        name = name.ToLower();
        if (name.Contains("success") || name.Contains("paid") || name.Contains("completed") || name.Contains("active"))
            return new StatusBadgeMetadata { Style = BadgeStyle.Success, Variant = BadgeVariant.Soft };
        
        if (name.Contains("error") || name.Contains("fail") || name.Contains("cancel") || name.Contains("deleted"))
            return new StatusBadgeMetadata { Style = BadgeStyle.Danger, Variant = BadgeVariant.Soft };

        if (name.Contains("pending") || name.Contains("warning") || name.Contains("processing"))
            return new StatusBadgeMetadata { Style = BadgeStyle.Warning, Variant = BadgeVariant.Soft };

        return null;
    }

    private string GetLocalizedText<TEnum>(TEnum value) where TEnum : struct, Enum
    {
        if (_localizerFactory != null)
        {
            var localizer = _localizerFactory.Create(typeof(TEnum));
            var localized = localizer[value.ToString()];
            if (!localized.ResourceNotFound) return localized.Value;
        }
        return value.ToString();
    }
}
