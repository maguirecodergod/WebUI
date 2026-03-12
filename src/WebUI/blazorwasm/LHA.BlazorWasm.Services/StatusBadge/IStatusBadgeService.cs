using System;
using System.Collections.Generic;
using LHA.BlazorWasm.Shared.Models.StatusBadge;

namespace LHA.BlazorWasm.Services.StatusBadge;

public interface IStatusBadgeService
{
    /// <summary>
    /// Registers a mapping for a specific enum type.
    /// </summary>
    void Register<TEnum>(Action<StatusBadgeMappingBuilder<TEnum>> builder) where TEnum : struct, Enum;

    /// <summary>
    /// Gets the badge metadata for a specific enum value.
    /// </summary>
    StatusBadgeMetadata GetMetadata<TEnum>(TEnum value) where TEnum : struct, Enum;
}

public class StatusBadgeMappingBuilder<TEnum> where TEnum : struct, Enum
{
    private readonly Dictionary<TEnum, StatusBadgeMetadata> _mappings = new();

    public StatusBadgeMappingBuilder<TEnum> Map(TEnum value, Action<StatusBadgeMetadata> config)
    {
        var metadata = new StatusBadgeMetadata();
        config(metadata);
        _mappings[value] = metadata;
        return this;
    }

    internal IReadOnlyDictionary<TEnum, StatusBadgeMetadata> Build() => _mappings;
}
