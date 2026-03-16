using LHA.Ddd.Domain;
using LHA.MultiTenancy;

namespace LHA.Mega.Domain.Account;

public class MegaAccountEntity : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public Guid? TenantId { get; set; }
    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string? PhoneNumber { get; private set; }
    public string? Email { get; private set; }
    public string? Address { get; private set; }
    public bool IsActive { get; private set; } = true;

    protected MegaAccountEntity() { }

    public MegaAccountEntity(Guid id, string code, string name)
        : base()
    {
        Id = id;
        SetCode(code);
        SetName(name);
    }

    public MegaAccountEntity SetCode(string code)
    {
        Code = code ?? throw new ArgumentNullException(nameof(code));
        return this;
    }

    public MegaAccountEntity SetName(string name)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        return this;
    }

    public MegaAccountEntity SetPhoneNumber(string? phoneNumber)
    {
        PhoneNumber = phoneNumber;
        return this;
    }

    public MegaAccountEntity SetEmail(string? email)
    {
        Email = email;
        return this;
    }

    public MegaAccountEntity SetAddress(string? address)
    {
        Address = address;
        return this;
    }

    public MegaAccountEntity SetActive(bool isActive)
    {
        IsActive = isActive;
        return this;
    }
}