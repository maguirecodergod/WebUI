using LHA.AuditLog.Domain.Shared;

namespace LHA.AuditLog.EntityFrameworkCore.Contracts.Options
{
    /// <summary>
    /// Holds runtime configuration for the AuditLog EntityFrameworkCore module.
    /// </summary>
    public sealed class AuditLogEntityFrameworkCoreOptions
    {
        public CAuditLogStoreMode Mode { get; set; } = CAuditLogStoreMode.All;
        public Action<Microsoft.EntityFrameworkCore.ModelBuilder>? ModelConfigurator { get; set; }
        public Microsoft.EntityFrameworkCore.AutoTransactionBehavior? AutoTransactionBehavior { get; set; }
    }
}