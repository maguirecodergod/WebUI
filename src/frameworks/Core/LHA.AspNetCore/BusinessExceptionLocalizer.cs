using LHA.Ddd.Domain;
using LHA.Localization.Abstraction;
using Microsoft.Extensions.Options;

namespace LHA.AspNetCore;

/// <summary>
/// Default implementation of <see cref="IBusinessExceptionLocalizer"/>.
/// <para>
/// For each <see cref="BusinessException"/> it iterates the registered
/// <see cref="LocalizationResourceOptions.Resources"/> in order and delegates
/// to the per-resource <see cref="IStringLocalizer"/> created by
/// <see cref="ILHAStringLocalizerFactory"/>.  The localizer already handles
/// the full culture-fallback chain (specific → neutral → default).
/// </para>
/// <para>
/// First resource whose entry exists for the current UI culture wins.
/// When no resource contains the key, <see cref="Exception.Message"/> is
/// returned unchanged (which equals <see cref="BusinessException.Code"/> when
/// no explicit message was passed to the constructor).
/// </para>
/// </summary>
internal sealed class BusinessExceptionLocalizer : IBusinessExceptionLocalizer
{
    private readonly ILHAStringLocalizerFactory _localizerFactory;
    private readonly LocalizationResourceOptions _options;

    public BusinessExceptionLocalizer(
        ILHAStringLocalizerFactory localizerFactory,
        IOptions<LocalizationResourceOptions> options)
    {
        _localizerFactory = localizerFactory;
        _options = options.Value;
    }

    /// <inheritdoc />
    public string Localize(BusinessException exception)
    {
        foreach (var descriptor in _options.Resources)
        {
            var localizer = _localizerFactory.CreateLocalizer(descriptor.ResourceType);

            var result = exception.Args is { Length: > 0 }
                ? localizer[exception.Code, exception.Args]
                : localizer[exception.Code];

            // ResourceNotFound == false  →  key was resolved in this or a
            // fallback culture; use it and stop searching.
            if (!result.ResourceNotFound)
                return result.Value;
        }

        // No resource contained the code key – return the raw message so the
        // caller always gets something meaningful.
        return exception.Message;
    }
}
