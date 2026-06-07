using LHA.Auditing;
using LHA.AuditLog.Domain.Shared;
using LHA.AuditLog.EntityFrameworkCore.Contracts.Options;
using LHA.AuditLog.EntityFrameworkCore.PostgreSQL;
using LHA.Core.Users;
using LHA.EntityFrameworkCore;
using LHA.MultiTenancy;
using LHA.Tax.Domain.Aggregates;
using LHA.Tax.Domain.BusinessRegistrations;
using LHA.Tax.Domain.Configuration;
using LHA.Tax.Domain.Customers;
using LHA.Tax.Domain.Determination;
using LHA.Tax.Domain.Invoicing;
using LHA.Tax.Domain.Reporting;
using Microsoft.EntityFrameworkCore;

namespace LHA.Tax.EntityFrameworkCore
{
    public sealed class TaxDbContext
        : LhaDbContext<TaxDbContext>, IHasEventOutbox, IHasEventInbox
    {

        private readonly LHA.EntityFrameworkCore.Auditing.DataAuditingSaveChangesInterceptor? _auditInterceptor;
        private readonly Microsoft.Extensions.Options.IOptions<AuditLogEntityFrameworkCoreOptions>? _auditOptions;

        public TaxDbContext(
            DbContextOptions<TaxDbContext> options,
            Microsoft.Extensions.Options.IOptions<AuditLogEntityFrameworkCoreOptions>? auditOptions = null,
            LHA.EntityFrameworkCore.Auditing.DataAuditingSaveChangesInterceptor? auditInterceptor = null,
            IAuditPropertySetter? auditPropertySetter = null,
            ICurrentTenant? currentTenant = null,
            ICurrentUser? currentUser = null)
            : base(options, auditPropertySetter, currentTenant, currentUser)
        {
            _auditOptions = auditOptions;
            _auditInterceptor = auditInterceptor;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            if (_auditInterceptor is not null)
            {
                optionsBuilder.AddInterceptors(_auditInterceptor);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Apply configurations from the assembly
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(TaxDbContext).Assembly);

            // 3. Audit Log Module
            var auditMode = _auditOptions?.Value.Mode ?? CAuditLogStoreMode.All;
            modelBuilder.ConfigureAuditLogPostgreSql(auditMode);

            // 5. Event Bus
            modelBuilder.TryConfigureEventBus<TaxDbContext>();

            // ─── Override table names with Tax Service conventions ───

            // Jurisdictions
            modelBuilder.Entity<TaxJurisdictionEntity>().ToTable(DbSchemeConsts.Tax.Jurisdiction);

            // Tax Regimes
            modelBuilder.Entity<TaxRegimeEntity>().ToTable(DbSchemeConsts.Tax.Regime);

            // Tax Rates
            modelBuilder.Entity<TaxRateEntity>().ToTable(DbSchemeConsts.Tax.Rate);

            // Tax Product Categories
            modelBuilder.Entity<TaxProductCategoryEntity>().ToTable(DbSchemeConsts.Tax.ProductCategory);

            // Tax Registration Thresholds
            modelBuilder.Entity<TaxRegistrationThresholdEntity>().ToTable(DbSchemeConsts.Tax.RegistrationThreshold);

            // Customer Tax Profiles
            modelBuilder.Entity<CustomerTaxProfileEntity>().ToTable(DbSchemeConsts.Tax.CustomerTaxProfile);

            // Customer Tax Identifiers
            modelBuilder.Entity<CustomerTaxIdentifierEntity>().ToTable(DbSchemeConsts.Tax.CustomerTaxIdentifier);

            // Customer Tax Exemptions
            modelBuilder.Entity<CustomerTaxExemptionEntity>().ToTable(DbSchemeConsts.Tax.CustomerTaxExemption);

            // Business Tax Registrations
            modelBuilder.Entity<BusinessTaxRegistrationEntity>().ToTable(DbSchemeConsts.Tax.BusinessTaxRegistration);

            // Tax Determination Requests
            modelBuilder.Entity<TaxDeterminationRequestEntity>().ToTable(DbSchemeConsts.Tax.DeterminationRequest);

            // Tax Determination Results
            modelBuilder.Entity<TaxDeterminationResultEntity>().ToTable(DbSchemeConsts.Tax.DeterminationResult);

            // Invoice Tax Lines
            modelBuilder.Entity<InvoiceTaxLineEntity>().ToTable(DbSchemeConsts.Tax.InvoiceTaxLine);

            // Tax Period Summaries
            modelBuilder.Entity<TaxPeriodSummaryEntity>().ToTable(DbSchemeConsts.Tax.PeriodSummary);

            // Event Bus
            modelBuilder.Entity<OutboxMessage>().ToTable(DbSchemeConsts.Event.Outbox);
            modelBuilder.Entity<InboxMessage>().ToTable(DbSchemeConsts.Event.Inbox);

            // Apply global query filters after all entities are configured in the ModelBuilder
            ApplyGlobalFilters(modelBuilder);
        }


        /// <inheritdoc />
        public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

        /// <inheritdoc />
        public DbSet<InboxMessage> InboxMessages => Set<InboxMessage>();

        // ─── Tax Domain DbSets ─────────────────────────────────────
        public DbSet<TaxJurisdictionEntity> TaxJurisdictions => Set<TaxJurisdictionEntity>();
        public DbSet<TaxRegimeEntity> TaxRegimes => Set<TaxRegimeEntity>();
        public DbSet<TaxRateEntity> TaxRates => Set<TaxRateEntity>();
        public DbSet<TaxProductCategoryEntity> TaxProductCategories => Set<TaxProductCategoryEntity>();
        public DbSet<TaxRegistrationThresholdEntity> TaxRegistrationThresholds => Set<TaxRegistrationThresholdEntity>();
        public DbSet<CustomerTaxProfileEntity> CustomerTaxProfiles => Set<CustomerTaxProfileEntity>();
        public DbSet<CustomerTaxIdentifierEntity> CustomerTaxIdentifiers => Set<CustomerTaxIdentifierEntity>();
        public DbSet<CustomerTaxExemptionEntity> CustomerTaxExemptions => Set<CustomerTaxExemptionEntity>();
        public DbSet<BusinessTaxRegistrationEntity> BusinessTaxRegistrations => Set<BusinessTaxRegistrationEntity>();
        public DbSet<TaxDeterminationRequestEntity> TaxDeterminationRequests => Set<TaxDeterminationRequestEntity>();
        public DbSet<TaxDeterminationResultEntity> TaxDeterminationResults => Set<TaxDeterminationResultEntity>();
        public DbSet<InvoiceTaxLineEntity> InvoiceTaxLines => Set<InvoiceTaxLineEntity>();
        public DbSet<TaxPeriodSummaryEntity> TaxPeriodSummaries => Set<TaxPeriodSummaryEntity>();
    }
}