using LHA.Ddd.Domain;

namespace LHA.AspNetCore;

/// <summary>
/// Resolves a human-readable, culture-aware message for a
/// <see cref="BusinessException"/> by looking up
/// <see cref="BusinessException.Code"/> across all registered localization
/// resources and formatting the result with <see cref="BusinessException.Args"/>.
/// </summary>
/// <remarks>
/// Register a concrete implementation by calling
/// <c>services.AddLHABusinessExceptionLocalization()</c> after
/// <c>services.AddLHALocalization(…)</c>.  The handler is resolved as an
/// optional dependency; if it is absent the raw <see cref="Exception.Message"/>
/// is used instead.
/// </remarks>
public interface IBusinessExceptionLocalizer
{
    /// <summary>
    /// Returns the localized message for <paramref name="exception"/>.
    /// Falls back to <see cref="Exception.Message"/> when the key is not found
    /// in any registered resource.
    /// </summary>
    string Localize(BusinessException exception);
}
