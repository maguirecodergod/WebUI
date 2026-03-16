using LHA.Ddd.Application;
using LHA.EventBus;
using LHA.Mega.Application.Contracts.Account;
using LHA.Mega.Domain.Account;
using LHA.Mega.Domain.Shared.Events;
using LHA.UnitOfWork;

namespace LHA.Mega.Application.Account;

public sealed class MegaAccountAppService(
    IMegaAccountRepository repository,
    IUnitOfWorkManager uowManager,
    IEventBus eventBus) : IMegaAccountAppService
{
    public async Task<MegaAccountDto> GetAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await repository.GetAsync(id, ct);
        return MapToDto(entity);
    }

    public async Task<PagedResultDto<MegaAccountDto>> GetListAsync(
        string? filter, bool? isActive, string? sorting,
        int skipCount = 0, int maxResultCount = 10, CancellationToken ct = default)
    {
        var totalCount = await repository.GetCountAsync(filter, isActive, ct);
        var items = await repository.GetListAsync(filter, isActive, sorting, skipCount, maxResultCount, ct);
        return new PagedResultDto<MegaAccountDto>(
            totalCount, items.ConvertAll(MapToDto), skipCount, maxResultCount);
    }

    public async Task<MegaAccountDto> CreateAsync(CreateMegaAccountInput input, CancellationToken ct = default)
    {
        var existing = await repository.FindByCodeAsync(input.Code, ct);
        if (existing is not null)
            throw new InvalidOperationException($"Account with code '{input.Code}' already exists.");

        var entity = new MegaAccountEntity(Guid.CreateVersion7(), input.Code, input.Name)
            .SetPhoneNumber(input.PhoneNumber)
            .SetEmail(input.Email)
            .SetAddress(input.Address);

        using var uow = uowManager.Begin(isTransactional: true);
        await repository.InsertAsync(entity, ct);

        await eventBus.PublishAsync(new MegaAccountCreatedEvent
        {
            AccountId = entity.Id,
            Code = entity.Code,
            Name = entity.Name,
            PhoneNumber = entity.PhoneNumber,
            Email = entity.Email,
            Address = entity.Address,
            TenantId = entity.TenantId,
        }, new EventPublishOptions()
        {
            UseOutbox = true
        }, ct);

        await uow.CompleteAsync(ct);

        return MapToDto(entity);
    }

    public async Task<MegaAccountDto> UpdateAsync(Guid id, UpdateMegaAccountInput input, CancellationToken ct = default)
    {
        var entity = await repository.GetAsync(id, ct);

        entity.ConcurrencyStamp = input.ConcurrencyStamp;
        entity.SetName(input.Name)
              .SetPhoneNumber(input.PhoneNumber)
              .SetEmail(input.Email)
              .SetAddress(input.Address)
              .SetActive(input.IsActive);

        using var uow = uowManager.Begin(isTransactional: true);
        await repository.UpdateAsync(entity, ct);

        await eventBus.PublishAsync(new MegaAccountUpdatedEvent
        {
            AccountId = entity.Id,
            Name = entity.Name,
            PhoneNumber = entity.PhoneNumber,
            Email = entity.Email,
            Address = entity.Address,
            IsActive = entity.IsActive,
            TenantId = entity.TenantId,
        }, ct);

        await uow.CompleteAsync(ct);

        return MapToDto(entity);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        using var uow = uowManager.Begin(isTransactional: true);
        await repository.DeleteAsync(id, ct);

        await eventBus.PublishAsync(new MegaAccountDeletedEvent
        {
            AccountId = id,
        }, ct);

        await uow.CompleteAsync(ct);
    }

    private static MegaAccountDto MapToDto(MegaAccountEntity e) => new()
    {
        Id = e.Id,
        TenantId = e.TenantId,
        Code = e.Code,
        Name = e.Name,
        PhoneNumber = e.PhoneNumber,
        Email = e.Email,
        Address = e.Address,
        IsActive = e.IsActive,
        CreationTime = e.CreationTime,
        CreatorId = e.CreatorId,
        LastModificationTime = e.LastModificationTime,
        LastModifierId = e.LastModifierId,
        IsDeleted = e.IsDeleted,
        DeletionTime = e.DeletionTime,
        DeleterId = e.DeleterId,
    };
}
