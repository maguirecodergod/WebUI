using LHA.Shared.Domain.Attributes;

namespace LHA.Shared.Contracts.AuditLog;

public enum CServiceType
{
    [EnumMetadata(DisplayName = "ServiceType.Account", Icon = "bi bi-person-badge")]
    Account = 1,
    
    [EnumMetadata(DisplayName = "ServiceType.Notification", Icon = "bi bi-bell")]
    Notification = 2,
    
    [EnumMetadata(DisplayName = "ServiceType.Mega", Icon = "bi bi-grid-3x3-gap")]
    Mega = 3,
    
    [EnumMetadata(DisplayName = "ServiceType.Movie", Icon = "bi bi-film")]
    Movie = 4
}
