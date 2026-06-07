using LHA.Auditing;

namespace LHA.Account.Migrator;

/// <summary>
/// No-op implementation of <see cref="IClientInfoProvider"/> for use in migration/seeding context,
/// where there is no HTTP request and no client information is available.
/// </summary>
internal sealed class NullClientInfoProvider : IClientInfoProvider
{
    public string? ClientIpAddress => null;
    public string? BrowserInfo => null;
    public string? CorrelationId => null;
}
