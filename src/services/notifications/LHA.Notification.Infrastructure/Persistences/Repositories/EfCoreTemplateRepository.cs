using LHA.EntityFrameworkCore;
using LHA.Notification.Domain;
using LHA.Notification.Domain.Repositories;
using LHA.Notification.Domain.Shared;
using Microsoft.EntityFrameworkCore;

namespace LHA.Notification.Infrastructure.Persistences.Repositories;

/// <summary>
/// EF Core (MongoDB) implementation of <see cref="ITemplateRepository"/>.
/// </summary>
public sealed class EfCoreTemplateRepository
    : EfCoreRepository<NotificationDbContext, TemplateEntity, Guid>, ITemplateRepository
{
    public EfCoreTemplateRepository(IDbContextProvider<NotificationDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    /// <inheritdoc />
    public async Task<TemplateEntity?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .FirstOrDefaultAsync(t => t.Code == code, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<TemplateEntity>> GetByTypeAsync(CNotificationType type, CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .Where(t => t.Type == type)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<TemplateEntity>> GetActiveAsync(CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .Where(t => t.IsActive)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<TemplateEntity>> GetByTagsAsync(List<string> tags, CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .Where(t => t.Tags.Any(tag => tags.Contains(tag)))
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async IAsyncEnumerable<TemplateEntity> GetByTenantCursorAsync(
        Guid? tenantId, int batchSize, [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        var lastId = Guid.Empty;

        while (true)
        {
            var batch = await dbSet
                .Where(t => t.TenantId == tenantId && t.Id.CompareTo(lastId) > 0)
                .OrderBy(t => t.Id)
                .Take(batchSize)
                .ToListAsync(cancellationToken);

            if (batch.Count == 0)
                yield break;

            foreach (var item in batch)
            {
                yield return item;
            }

            lastId = batch[^1].Id;
        }
    }

    /// <inheritdoc />
    public async Task<IEnumerable<TemplateEntity>> SearchAsync(string query, CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .Where(t => t.Name.Contains(query) || t.Code.Contains(query) || t.Description!.Contains(query))
            .ToListAsync(cancellationToken);
    }
}
