using LHA.Ddd.Application;
using LHA.Ddd.Domain;
using LHA.TenantManagement.Application.Contracts;
using LHA.TenantManagement.Domain;
using LHA.TenantManagement.Domain.Shared;
using LHA.UnitOfWork;

namespace LHA.TenantManagement.Application;

/// <summary>
/// Application service for the Tenant Management module.
/// <para>
/// All write operations use explicit <see cref="IUnitOfWork"/> boundaries with
/// transactional consistency. Read operations use the repository directly.
/// </para>
/// </summary>
public sealed class TenantAppService : ApplicationService, ITenantAppService
{
    private readonly ITenantRepository _tenantRepository;
    private readonly TenantManager _tenantManager;
    private readonly IUnitOfWorkManager _unitOfWorkManager;

    public TenantAppService(
        ITenantRepository tenantRepository,
        TenantManager tenantManager,
        IUnitOfWorkManager unitOfWorkManager)
    {
        _tenantRepository = tenantRepository;
        _tenantManager = tenantManager;
        _unitOfWorkManager = unitOfWorkManager;
    }

    // ─── CRUD (ICrudAppService) ──────────────────────────────────────

    /// <inheritdoc />
    public async Task<TenantDto> GetAsync(Guid id)
    {
        var tenant = await _tenantRepository.GetAsync(id);
        return MapToDto(tenant);
    }

    /// <inheritdoc />
    public async Task<PagedResultDto<TenantDto>> GetListAsync(GetTenantsInput input)
    {
        var totalCount = await _tenantRepository.GetCountAsync(input.Filter, input.Status);
        var tenants = await _tenantRepository.GetListAsync(
            input,
            sorter: input.Sorter,
            filter: input.Filter,
            status: input.Status);

        return new PagedResultDto<TenantDto>(
            totalCount,
            tenants.ConvertAll(MapToDto),
            input.PageNumber,
            input.PageSize);
    }

    /// <inheritdoc />
    public async Task<TenantDto> CreateAsync(CreateTenantInput input)
    {
        await using var uow = _unitOfWorkManager.Begin(
            new UnitOfWorkOptions { IsTransactional = true });

        var tenant = await _tenantManager.CreateAsync(input.Name, input.DatabaseStyle);

        foreach (var (name, value) in input.ConnectionStrings)
        {
            tenant.AddOrUpdateConnectionString(name, value);
        }

        await _tenantRepository.InsertAsync(tenant);
        await uow.CompleteAsync();

        return MapToDto(tenant);
    }

    /// <inheritdoc />
    public async Task<TenantDto> UpdateAsync(Guid id, UpdateTenantInput input)
    {
        await using var uow = _unitOfWorkManager.Begin(
            new UnitOfWorkOptions { IsTransactional = true });

        var tenant = await _tenantRepository.GetAsync(id);

        // Optimistic concurrency check
        if (!string.Equals(tenant.ConcurrencyStamp, input.ConcurrencyStamp, StringComparison.Ordinal))
            throw new DbUpdateConcurrencyException(
                $"The tenant '{id}' has been modified by another process. " +
                $"Expected stamp '{input.ConcurrencyStamp}', current '{tenant.ConcurrencyStamp}'.");

        await _tenantManager.ChangeNameAsync(tenant, input.Name);
        await _tenantRepository.UpdateAsync(tenant);
        await uow.CompleteAsync();

        return MapToDto(tenant);
    }

    /// <inheritdoc />
    public async Task DeleteAsync(Guid id)
    {
        await using var uow = _unitOfWorkManager.Begin(
            new UnitOfWorkOptions { IsTransactional = true });

        await _tenantRepository.DeleteAsync(id);
        await uow.CompleteAsync();
    }

    // ─── Name lookup ─────────────────────────────────────────────────

    /// <inheritdoc />
    public async Task<TenantDto?> FindByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        var normalizedName = name.Trim().ToUpperInvariant();
        var tenant = await _tenantRepository.FindByNameAsync(normalizedName, cancellationToken);

        return tenant is not null ? MapToDto(tenant) : null;
    }

    // ─── Connection Strings ──────────────────────────────────────────

    /// <inheritdoc />
    public async Task<List<TenantConnectionStringDto>> GetConnectionStringsAsync(
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        var tenant = await _tenantRepository.GetAsync(tenantId, cancellationToken);

        return tenant.ConnectionStrings
            .Select(cs => new TenantConnectionStringDto { Name = cs.Name, Value = cs.Value })
            .ToList();
    }

    /// <inheritdoc />
    public async Task<TenantDto> SetConnectionStringAsync(
        Guid tenantId,
        string name,
        string value,
        CancellationToken cancellationToken = default)
    {
        await using var uow = _unitOfWorkManager.Begin(
            new UnitOfWorkOptions { IsTransactional = true });

        var tenant = await _tenantRepository.GetAsync(tenantId, cancellationToken);
        tenant.AddOrUpdateConnectionString(name, value);
        await _tenantRepository.UpdateAsync(tenant);
        await uow.CompleteAsync();

        return MapToDto(tenant);
    }

    /// <inheritdoc />
    public async Task<TenantDto> RemoveConnectionStringAsync(
        Guid tenantId,
        string name,
        CancellationToken cancellationToken = default)
    {
        await using var uow = _unitOfWorkManager.Begin(
            new UnitOfWorkOptions { IsTransactional = true });

        var tenant = await _tenantRepository.GetAsync(tenantId, cancellationToken);
        tenant.RemoveConnectionString(name);
        await _tenantRepository.UpdateAsync(tenant);
        await uow.CompleteAsync();

        return MapToDto(tenant);
    }

    // ─── Activation ──────────────────────────────────────────────────

    /// <inheritdoc />
    public async Task<TenantDto> ActivateAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await using var uow = _unitOfWorkManager.Begin(
            new UnitOfWorkOptions { IsTransactional = true });

        var tenant = await _tenantRepository.GetAsync(id, cancellationToken);
        tenant.Activate();
        await _tenantRepository.UpdateAsync(tenant);
        await uow.CompleteAsync();

        return MapToDto(tenant);
    }

    /// <inheritdoc />
    public async Task<TenantDto> DeactivateAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await using var uow = _unitOfWorkManager.Begin(
            new UnitOfWorkOptions { IsTransactional = true });

        var tenant = await _tenantRepository.GetAsync(id, cancellationToken);
        tenant.Deactivate();
        await _tenantRepository.UpdateAsync(tenant);
        await uow.CompleteAsync();

        return MapToDto(tenant);
    }

    // ─── Mapping ─────────────────────────────────────────────────────

    private static TenantDto MapToDto(TenantEntity tenant) => new()
    {
        Id = tenant.Id,
        Name = tenant.Name,
        Status = tenant.Status,
        DatabaseStyle = tenant.DatabaseStyle,
        ConcurrencyStamp = tenant.ConcurrencyStamp,
        CreationTime = tenant.CreationTime,
        CreatorId = tenant.CreatorId,
        LastModificationTime = tenant.LastModificationTime,
        LastModifierId = tenant.LastModifierId,
        IsDeleted = tenant.IsDeleted,
        DeletionTime = tenant.DeletionTime,
        DeleterId = tenant.DeleterId,
        ConnectionStrings = tenant.ConnectionStrings
            .Select(cs => new TenantConnectionStringDto { Name = cs.Name, Value = cs.Value })
            .ToList()
    };
}

/// <summary>
/// Custom exception for concurrency conflicts detected at the application layer.
/// </summary>
public sealed class DbUpdateConcurrencyException : Exception
{
    public DbUpdateConcurrencyException(string message) : base(message) { }
}
