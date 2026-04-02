using LHA.Ddd.Application;
using LHA.Ddd.Domain;
using LHA.Identity.Application.Contracts;
using LHA.Identity.Domain;
using LHA.Shared.Domain.Identity;
using LHA.UnitOfWork;

namespace LHA.Identity.Application;

/// <summary>
/// Application service for claim type management.
/// </summary>
public sealed class IdentityClaimTypeAppService : ApplicationService, IIdentityClaimTypeAppService
{
    private readonly IIdentityClaimTypeRepository _claimTypeRepository;
    private readonly IUnitOfWorkManager _uowManager;

    public IdentityClaimTypeAppService(
        IIdentityClaimTypeRepository claimTypeRepository,
        IUnitOfWorkManager uowManager)
    {
        _claimTypeRepository = claimTypeRepository;
        _uowManager = uowManager;
    }

    /// <inheritdoc />
    public async Task<IdentityClaimTypeDto> GetAsync(Guid id, CancellationToken ct)
    {
        var claimType = await _claimTypeRepository.GetAsync(id, ct);
        return MapToDto(claimType);
    }

    /// <inheritdoc />
    public async Task<PagedResultDto<IdentityClaimTypeDto>> GetListAsync(GetClaimTypesInput input, CancellationToken ct)
    {
        var totalCount = await _claimTypeRepository.GetCountAsync(input.Filter, ct);
        var items = await _claimTypeRepository.GetListAsync(
            input,
            sorter: input.Sorter,
            filter: input.Filter,
            cancellationToken: ct);

        return new PagedResultDto<IdentityClaimTypeDto>(
            totalCount,
            items.ConvertAll(MapToDto),
            input.PageNumber,
            input.PageSize);
    }

    /// <inheritdoc />
    public async Task<IdentityClaimTypeDto> CreateAsync(CreateOrUpdateClaimTypeInput input, CancellationToken ct)
    {
        await using var uow = _uowManager.Begin(
            new UnitOfWorkOptions { IsTransactional = true });

        if (await _claimTypeRepository.AnyAsync(input.Name, null, ct))
            throw new InvalidOperationException($"Claim type '{input.Name}' already exists.");

        var claimType = new IdentityClaimType(
            id: Guid.CreateVersion7(),
            name: input.Name,
            required: input.Required,
            isStatic: false,
            valueType: input.ValueType,
            regex: input.Regex,
            regexDescription: input.RegexDescription,
            description: input.Description);

        await _claimTypeRepository.InsertAsync(claimType, ct);
        await uow.CompleteAsync();

        return MapToDto(claimType);
    }

    /// <inheritdoc />
    public async Task<IdentityClaimTypeDto> UpdateAsync(Guid id, CreateOrUpdateClaimTypeInput input, CancellationToken ct)
    {
        await using var uow = _uowManager.Begin(
            new UnitOfWorkOptions { IsTransactional = true });

        var claimType = await _claimTypeRepository.GetAsync(id, ct);
        claimType.Update(
            name: input.Name,
            required: input.Required,
            valueType: input.ValueType,
            regex: input.Regex,
            regexDescription: input.RegexDescription,
            description: input.Description);

        await _claimTypeRepository.UpdateAsync(claimType, ct);
        await uow.CompleteAsync();

        return MapToDto(claimType);
    }

    /// <inheritdoc />
    public async Task DeleteAsync(Guid id, CancellationToken ct)
    {
        await using var uow = _uowManager.Begin(
            new UnitOfWorkOptions { IsTransactional = true });

        var claimType = await _claimTypeRepository.GetAsync(id, ct);
        if (claimType.IsStatic)
            throw new InvalidOperationException($"Cannot delete static claim type '{claimType.Name}'.");

        await _claimTypeRepository.DeleteAsync(id, ct);
        await uow.CompleteAsync();
    }

    private static IdentityClaimTypeDto MapToDto(IdentityClaimType ct) => new()
    {
        Id = ct.Id,
        Name = ct.Name,
        Required = ct.Required,
        IsStatic = ct.IsStatic,
        ValueType = ct.ValueType,
        Regex = ct.Regex,
        RegexDescription = ct.RegexDescription,
        Description = ct.Description,
        CreationTime = ct.CreationTime,
        CreatorId = ct.CreatorId,
        LastModificationTime = ct.LastModificationTime,
        LastModifierId = ct.LastModifierId,
        IsDeleted = ct.IsDeleted,
        DeletionTime = ct.DeletionTime,
        DeleterId = ct.DeleterId,
    };
}
