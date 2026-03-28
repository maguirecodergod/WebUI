using LHA.Ddd.Application;

namespace LHA.Mega.Application.Contracts.Account;

public class GetMegaAccountsInput : PagedAndSortedResultRequestDto
{
    public string? Filter { get; set; }
    public bool? IsActive { get; set; }
}
