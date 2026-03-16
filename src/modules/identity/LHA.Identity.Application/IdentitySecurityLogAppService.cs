using LHA.Ddd.Application;
using LHA.Identity.Application.Contracts;
using LHA.Identity.Domain;

namespace LHA.Identity.Application;

/// <summary>
/// Application service for querying security logs (read-only).
/// </summary>
public sealed class IdentitySecurityLogAppService : ApplicationService, IIdentitySecurityLogAppService
{
    private readonly IIdentitySecurityLogRepository _securityLogRepository;

    public IdentitySecurityLogAppService(IIdentitySecurityLogRepository securityLogRepository)
    {
        _securityLogRepository = securityLogRepository;
    }

    /// <inheritdoc />
    public async Task<PagedResultDto<IdentitySecurityLogDto>> GetListAsync(
        GetSecurityLogsInput input, CancellationToken ct)
    {
        var totalCount = await _securityLogRepository.GetCountAsync(
            input.Filter, input.UserId, input.Action, ct);

        var items = await _securityLogRepository.GetListAsync(
            filter: input.Filter,
            userId: input.UserId,
            action: input.Action,
            sorting: input.Sorting,
            skipCount: input.SkipCount,
            maxResultCount: input.MaxResultCount,
            cancellationToken: ct);

        return new PagedResultDto<IdentitySecurityLogDto>(
            totalCount,
            items.ConvertAll(MapToDto),
            input.SkipCount,
            input.MaxResultCount);
    }

    private static IdentitySecurityLogDto MapToDto(IdentitySecurityLog log) => new()
    {
        Id = log.Id,
        TenantId = log.TenantId,
        ApplicationName = log.ApplicationName,
        Identity = log.Identity,
        Action = log.Action,
        UserId = log.UserId,
        UserName = log.UserName,
        TenantName = log.TenantName,
        ClientId = log.ClientId,
        CorrelationId = log.CorrelationId,
        ClientIpAddress = log.ClientIpAddress,
        BrowserInfo = log.BrowserInfo,
        CreationTime = log.CreationTime,
        CreatorId = log.CreatorId,
    };
}
