# Project Structure

_Generated automatically on 2026-03-23 11:22:37_

```
.
+-- .vscode
|   \-- settings.json
+-- contracts
|   \-- proto
|       +-- lha
|       |   +-- common
|       |   |   \-- v1
|       |   |       +-- common.proto
|       |   |       +-- errors.proto
|       |   |       \-- streaming.proto
|       |   \-- services
|       |       +-- account
|       |       |   \-- v1
|       |       |       \-- permission_registration_service.proto
|       |       \-- user
|       |           \-- v1
|       |               \-- user_service.proto
|       \-- buf.yaml
+-- dockers
|   \-- docker-compose.yml
+-- src
|   +-- frameworks
|   |   +-- Auditing
|   |   |   +-- LHA.Auditing
|   |   |   |   +-- AuditingManager.cs
|   |   |   |   +-- AuditingOptions.cs
|   |   |   |   +-- AuditingServiceCollectionExtensions.cs
|   |   |   |   +-- AuditLogAction.cs
|   |   |   |   +-- AuditLogContributionContext.cs
|   |   |   |   +-- AuditLogEntry.cs
|   |   |   |   +-- AuditPropertySetter.cs
|   |   |   |   +-- EntityChangeEntry.cs
|   |   |   |   +-- EntityPropertyChange.cs
|   |   |   |   +-- IAuditingManager.cs
|   |   |   |   +-- IAuditingStore.cs
|   |   |   |   +-- IAuditLogContributor.cs
|   |   |   |   +-- IAuditLogSaveHandle.cs
|   |   |   |   +-- IAuditLogScope.cs
|   |   |   |   +-- IAuditPropertySetter.cs
|   |   |   |   +-- IAuditSerializer.cs
|   |   |   |   +-- IAuditUserProvider.cs
|   |   |   |   +-- JsonAuditSerializer.cs
|   |   |   |   +-- LHA.Auditing.csproj
|   |   |   |   +-- LoggingAuditingStore.cs
|   |   |   |   \-- NullAuditUserProvider.cs
|   |   |   \-- LHA.Auditing.Contracts
|   |   |       +-- Attributes
|   |   |       |   +-- AuditedAttribute.cs
|   |   |       |   \-- DisableAuditingAttribute.cs
|   |   |       +-- CEntityChangeType.cs
|   |   |       +-- IAuditedObject.cs
|   |   |       +-- IClientInfoProvider.cs
|   |   |       +-- ICreationAuditedObject.cs
|   |   |       +-- IDeletionAuditedObject.cs
|   |   |       +-- IFullAuditedObject.cs
|   |   |       +-- IHasCreationTime.cs
|   |   |       +-- IHasDeletionTime.cs
|   |   |       +-- IHasEntityVersion.cs
|   |   |       +-- IHasModificationTime.cs
|   |   |       +-- IMayHaveCreator.cs
|   |   |       +-- IModificationAuditedObject.cs
|   |   |       +-- IMustHaveCreator.cs
|   |   |       +-- ISoftDelete.cs
|   |   |       \-- LHA.Auditing.Contracts.csproj
|   |   +-- BackgroundJob
|   |   |   +-- LHA.BackgroundJob
|   |   |   |   +-- BackgroundJobEntity.cs
|   |   |   |   +-- BackgroundJobWorker.cs
|   |   |   |   +-- BackgroundJobWorkerOptions.cs
|   |   |   |   +-- DefaultBackgroundJobManager.cs
|   |   |   |   +-- DefaultBackgroundJobServiceCollectionExtensions.cs
|   |   |   |   +-- IBackgroundJobStore.cs
|   |   |   |   +-- InMemoryBackgroundJobStore.cs
|   |   |   |   \-- LHA.BackgroundJob.csproj
|   |   |   +-- LHA.BackgroundJob.Abstraction
|   |   |   |   +-- BackgroundJobConfiguration.cs
|   |   |   |   +-- BackgroundJobExecuter.cs
|   |   |   |   +-- BackgroundJobExecutionException.cs
|   |   |   |   +-- BackgroundJobNameAttribute.cs
|   |   |   |   +-- BackgroundJobOptions.cs
|   |   |   |   +-- BackgroundJobServiceCollectionExtensions.cs
|   |   |   |   +-- CBackgroundJobPriority.cs
|   |   |   |   +-- IBackgroundJob.cs
|   |   |   |   +-- IBackgroundJobExecuter.cs
|   |   |   |   +-- IBackgroundJobManager.cs
|   |   |   |   +-- IBackgroundJobSerializer.cs
|   |   |   |   +-- JobExecutionContext.cs
|   |   |   |   +-- JsonBackgroundJobSerializer.cs
|   |   |   |   +-- LHA.BackgroundJob.Abstraction.csproj
|   |   |   |   \-- NullBackgroundJobManager.cs
|   |   |   +-- LHA.BackgroundJob.Hangfire
|   |   |   |   +-- HangfireBackgroundJobManager.cs
|   |   |   |   +-- HangfireBackgroundJobServiceCollectionExtensions.cs
|   |   |   |   +-- HangfireJobExecutionAdapter.cs
|   |   |   |   \-- LHA.BackgroundJob.Hangfire.csproj
|   |   |   +-- LHA.BackgroundJob.Quartz
|   |   |   |   +-- LHA.BackgroundJob.Quartz.csproj
|   |   |   |   +-- QuartzBackgroundJobManager.cs
|   |   |   |   +-- QuartzBackgroundJobOptions.cs
|   |   |   |   +-- QuartzBackgroundJobServiceCollectionExtensions.cs
|   |   |   |   \-- QuartzJobExecutionAdapter.cs
|   |   |   \-- LHA.BackgroundJob.RabbitMQ
|   |   |       +-- LHA.BackgroundJob.RabbitMQ.csproj
|   |   |       +-- RabbitMqBackgroundJobManager.cs
|   |   |       +-- RabbitMqBackgroundJobOptions.cs
|   |   |       +-- RabbitMqBackgroundJobServiceCollectionExtensions.cs
|   |   |       \-- RabbitMqJobConsumer.cs
|   |   +-- BackgroundWorker
|   |   |   +-- LHA.BackgroundWorker
|   |   |   |   +-- BackgroundWorkerBase.cs
|   |   |   |   +-- BackgroundWorkerHostedService.cs
|   |   |   |   +-- BackgroundWorkerHostingExtensions.cs
|   |   |   |   +-- LHA.BackgroundWorker.csproj
|   |   |   |   \-- PeriodicBackgroundWorker.cs
|   |   |   +-- LHA.BackgroundWorker.Abstraction
|   |   |   |   +-- BackgroundWorkerOptions.cs
|   |   |   |   +-- BackgroundWorkerServiceCollectionExtensions.cs
|   |   |   |   +-- DefaultBackgroundWorkerManager.cs
|   |   |   |   +-- IBackgroundWorker.cs
|   |   |   |   +-- IBackgroundWorkerManager.cs
|   |   |   |   \-- LHA.BackgroundWorker.Abstraction.csproj
|   |   |   +-- LHA.BackgroundWorker.Hangfire
|   |   |   |   +-- HangfireBackgroundWorkerServiceCollectionExtensions.cs
|   |   |   |   +-- HangfirePeriodicBackgroundWorker.cs
|   |   |   |   +-- HangfireWorkerJobExecutor.cs
|   |   |   |   \-- LHA.BackgroundWorker.Hangfire.csproj
|   |   |   \-- LHA.BackgroundWorker.Quartz
|   |   |       +-- LHA.BackgroundWorker.Quartz.csproj
|   |   |       +-- QuartzBackgroundWorkerServiceCollectionExtensions.cs
|   |   |       +-- QuartzPeriodicBackgroundWorker.cs
|   |   |       \-- QuartzWorkerJobAdapter.cs
|   |   +-- Caching
|   |   |   +-- LHA.Caching
|   |   |   |   +-- TenantCache
|   |   |   |   |   \-- CachedTenantStore.cs
|   |   |   |   +-- UserCache
|   |   |   |   |   +-- CachedUserItem.cs
|   |   |   |   |   \-- CachedUserLookupService.cs
|   |   |   |   +-- CacheKeyNormalizer.cs
|   |   |   |   +-- CacheNameAttribute.cs
|   |   |   |   +-- CachingOptions.cs
|   |   |   |   +-- CachingServiceCollectionExtensions.cs
|   |   |   |   +-- ICacheKeyNormalizer.cs
|   |   |   |   +-- ICacheSupportsMultipleItems.cs
|   |   |   |   +-- IDistributedCacheSerializer.cs
|   |   |   |   +-- ITypedDistributedCache.cs
|   |   |   |   +-- ITypedHybridCache.cs
|   |   |   |   +-- JsonDistributedCacheSerializer.cs
|   |   |   |   +-- LHA.Caching.csproj
|   |   |   |   +-- TypedDistributedCache.cs
|   |   |   |   \-- TypedHybridCache.cs
|   |   |   \-- LHA.Caching.StackExchangeRedis
|   |   |       +-- LHA.Caching.StackExchangeRedis.csproj
|   |   |       +-- RedisCachingServiceCollectionExtensions.cs
|   |   |       +-- RedisDistributedCache.cs
|   |   |       \-- RedisExtensions.cs
|   |   +-- Core
|   |   |   +-- LHA.AspNetCore
|   |   |   |   +-- Authorization
|   |   |   |   |   +-- PermissionAuthorizationHandler.cs
|   |   |   |   |   +-- PermissionAuthorizationPolicyProvider.cs
|   |   |   |   |   \-- PermissionRequirement.cs
|   |   |   |   +-- Localization
|   |   |   |   |   +-- AspNetCoreResource.cs
|   |   |   |   |   +-- en.json
|   |   |   |   |   \-- vi.json
|   |   |   |   +-- Security
|   |   |   |   |   +-- HttpContextAuditUserProvider.cs
|   |   |   |   |   +-- HttpContextCurrentUser.cs
|   |   |   |   |   +-- LhaClaimTypes.cs
|   |   |   |   |   \-- RuntimeCurrentContext.cs
|   |   |   |   +-- Versioning
|   |   |   |   |   \-- LhaApiVersioningExtensions.cs
|   |   |   |   +-- BusinessExceptionLocalizer.cs
|   |   |   |   +-- DependencyInjection.cs
|   |   |   |   +-- GlobalExceptionHandler.cs
|   |   |   |   +-- HttpContextClientInfoProvider.cs
|   |   |   |   +-- IBusinessExceptionLocalizer.cs
|   |   |   |   +-- LHA.AspNetCore.csproj
|   |   |   |   +-- LocalDateTimeOffsetJsonConverter.cs
|   |   |   |   \-- UnitOfWorkMiddleware.cs
|   |   |   \-- LHA.Core
|   |   |       +-- Disposable
|   |   |       |   +-- NullAsyncDisposable.cs
|   |   |       |   \-- NullDisposable.cs
|   |   |       +-- Enums
|   |   |       |   +-- CMasterStatus.cs
|   |   |       |   +-- CSearchCombineModeType.cs
|   |   |       |   \-- CSearchOperatorType.cs
|   |   |       +-- Threading
|   |   |       |   \-- KeyedLock.cs
|   |   |       +-- Users
|   |   |       |   +-- AsyncLocalCurrentUserAccessor.cs
|   |   |       |   +-- CurrentUserDefaults.cs
|   |   |       |   +-- CurrentUserExtensions.cs
|   |   |       |   +-- ICurrentUser.cs
|   |   |       |   +-- ICurrentUserAccessor.cs
|   |   |       |   +-- NullCurrentUser.cs
|   |   |       |   \-- UserServiceCollectionExtensions.cs
|   |   |       \-- LHA.Core.csproj
|   |   +-- Ddd
|   |   |   +-- LHA.Ddd.Application
|   |   |   |   +-- ApplicationService.cs
|   |   |   |   +-- CrudAppService.cs
|   |   |   |   +-- LHA.Ddd.Application.csproj
|   |   |   |   +-- PagedQueryableExtensions.cs
|   |   |   |   \-- ReadOnlyAppService.cs
|   |   |   +-- LHA.Ddd.Application.Contracts
|   |   |   |   +-- ApiResponse.cs
|   |   |   |   +-- AuditedEntityDto.cs
|   |   |   |   +-- CreationAuditedEntityDto.cs
|   |   |   |   +-- EntityDto.cs
|   |   |   |   +-- FullAuditedEntityDto.cs
|   |   |   |   +-- IApplicationService.cs
|   |   |   |   +-- ICrudAppService.cs
|   |   |   |   +-- IListResult.cs
|   |   |   |   +-- IPagedResult.cs
|   |   |   |   +-- IPagedResultRequest.cs
|   |   |   |   +-- IReadOnlyAppService.cs
|   |   |   |   +-- LHA.Ddd.Application.Contracts.csproj
|   |   |   |   +-- ListResultDto.cs
|   |   |   |   +-- PagedAndSortedResultRequestDto.cs
|   |   |   |   +-- PagedResultDto.cs
|   |   |   |   \-- PagedResultRequestDto.cs
|   |   |   +-- LHA.Ddd.Domain
|   |   |   |   +-- AggregateRoot.cs
|   |   |   |   +-- AuditedAggregateRoot.cs
|   |   |   |   +-- AuditedEntity.cs
|   |   |   |   +-- BusinessException.cs
|   |   |   |   +-- CreationAuditedAggregateRoot.cs
|   |   |   |   +-- CreationAuditedEntity.cs
|   |   |   |   +-- DomainService.cs
|   |   |   |   +-- Entity.cs
|   |   |   |   +-- EntityNotFoundException.cs
|   |   |   |   +-- FullAuditedAggregateRoot.cs
|   |   |   |   +-- FullAuditedEntity.cs
|   |   |   |   +-- IDomainService.cs
|   |   |   |   +-- IReadOnlyRepository.cs
|   |   |   |   +-- IRepository.cs
|   |   |   |   +-- ISpecification.cs
|   |   |   |   +-- LHA.Ddd.Domain.csproj
|   |   |   |   \-- ValueObject.cs
|   |   |   \-- LHA.Ddd.Domain.Shared
|   |   |       +-- DomainEventRecord.cs
|   |   |       +-- IDomainEvent.cs
|   |   |       +-- IEntity.cs
|   |   |       +-- IHasConcurrencyStamp.cs
|   |   |       +-- IHasDomainEvents.cs
|   |   |       \-- LHA.Ddd.Domain.Shared.csproj
|   |   +-- DistributedLocking
|   |   |   +-- LHA.DistributedLocking
|   |   |   |   +-- LHA.DistributedLocking.csproj
|   |   |   |   +-- MedallionDistributedLock.cs
|   |   |   |   +-- MedallionDistributedLockHandle.cs
|   |   |   |   +-- MedallionDistributedLockHealthCheck.cs
|   |   |   |   \-- MedallionDistributedLockServiceCollectionExtensions.cs
|   |   |   \-- LHA.DistributedLocking.Abstraction
|   |   |       +-- DistributedLockAcquisitionException.cs
|   |   |       +-- DistributedLockExtensions.cs
|   |   |       +-- DistributedLockKeyNormalizer.cs
|   |   |       +-- DistributedLockOptions.cs
|   |   |       +-- DistributedLockServiceCollectionExtensions.cs
|   |   |       +-- IDistributedLock.cs
|   |   |       +-- IDistributedLockHandle.cs
|   |   |       +-- IDistributedLockHealthCheck.cs
|   |   |       +-- IDistributedLockKeyNormalizer.cs
|   |   |       +-- LHA.DistributedLocking.Abstraction.csproj
|   |   |       +-- LocalDistributedLock.cs
|   |   |       +-- LocalDistributedLockHandle.cs
|   |   |       +-- NullDistributedLock.cs
|   |   |       \-- NullDistributedLockHandle.cs
|   |   +-- EntityFrameworkCore
|   |   |   +-- LHA.EntityFrameworkCore
|   |   |   |   +-- EfCoreDatabaseApi.cs
|   |   |   |   +-- EfCoreInboxStore.cs
|   |   |   |   +-- EfCoreOutboxStore.cs
|   |   |   |   +-- EfCoreRepository.cs
|   |   |   |   +-- EfCoreTransactionApi.cs
|   |   |   |   +-- EntityTypeBuilderExtensions.cs
|   |   |   |   +-- IDbContextProvider.cs
|   |   |   |   +-- IEfCoreRepository.cs
|   |   |   |   +-- IHasCurrentUnitOfWork.cs
|   |   |   |   +-- IHasEventInbox.cs
|   |   |   |   +-- IHasEventOutbox.cs
|   |   |   |   +-- InboxMessage.cs
|   |   |   |   +-- LHA.EntityFrameworkCore.csproj
|   |   |   |   +-- LhaDbContext.cs
|   |   |   |   +-- LhaDbContextConfigurationContext.cs
|   |   |   |   +-- LhaDbContextOptions.cs
|   |   |   |   +-- ModelBuilderEventBusExtensions.cs
|   |   |   |   +-- OutboxMessage.cs
|   |   |   |   +-- QueryableExtensions.cs
|   |   |   |   +-- ServiceCollectionExtensions.cs
|   |   |   |   \-- UnitOfWorkDbContextProvider.cs
|   |   |   \-- LHA.EntityFrameworkCore.PostgreSQL
|   |   |       +-- LHA.EntityFrameworkCore.PostgreSQL.csproj
|   |   |       +-- NpgsqlConfigurationContextExtensions.cs
|   |   |       +-- NpgsqlConnectionStringChecker.cs
|   |   |       \-- NpgsqlDbContextOptionsExtensions.cs
|   |   +-- EventBus
|   |   |   +-- LHA.EventBus
|   |   |   |   +-- DefaultEventNameResolver.cs
|   |   |   |   +-- EventUpgraderPipeline.cs
|   |   |   |   +-- InboxProcessor.cs
|   |   |   |   +-- InMemoryEventBus.cs
|   |   |   |   +-- InMemoryEventBusServiceCollectionExtensions.cs
|   |   |   |   +-- LHA.EventBus.csproj
|   |   |   |   +-- OutboxProcessor.cs
|   |   |   |   \-- SagaOrchestrator.cs
|   |   |   +-- LHA.EventBus.Abstraction
|   |   |   |   +-- Attributes
|   |   |   |   |   +-- EventNameAttribute.cs
|   |   |   |   |   \-- EventVersionAttribute.cs
|   |   |   |   +-- Configurations
|   |   |   |   |   \-- Options
|   |   |   |   |       +-- EventBusOptions.cs
|   |   |   |   |       \-- EventPublishOptions.cs
|   |   |   |   +-- Contracts
|   |   |   |   |   +-- Contexts
|   |   |   |   |   |   \-- EventContext.cs
|   |   |   |   |   +-- DTOs
|   |   |   |   |   |   +-- EventMetadata.cs
|   |   |   |   |   |   +-- InboxMessageInfo.cs
|   |   |   |   |   |   +-- OutboxMessageInfo.cs
|   |   |   |   |   |   \-- SagaStepResult.cs
|   |   |   |   |   +-- Enums
|   |   |   |   |   |   \-- CSagaStepStatus.cs
|   |   |   |   |   \-- ETOs
|   |   |   |   |       \-- EventEnvelope.cs
|   |   |   |   +-- Interfaces
|   |   |   |   |   +-- IEventBus.cs
|   |   |   |   |   +-- IEventHandler.cs
|   |   |   |   |   +-- IEventNameResolver.cs
|   |   |   |   |   +-- IEventUpgrader.cs
|   |   |   |   |   +-- IInboxStore.cs
|   |   |   |   |   +-- IIntegrationEvent.cs
|   |   |   |   |   +-- IOutboxStore.cs
|   |   |   |   |   +-- ISaga.cs
|   |   |   |   |   \-- ISagaStep.cs
|   |   |   |   +-- Stores
|   |   |   |   |   +-- NullInboxStore.cs
|   |   |   |   |   \-- NullOutboxStore.cs
|   |   |   |   +-- DependencyInjection.cs
|   |   |   |   +-- IntegrationEvent.cs
|   |   |   |   \-- LHA.EventBus.Abstraction.csproj
|   |   |   +-- LHA.EventBus.Kafka
|   |   |   |   +-- DependencyInjection.cs
|   |   |   |   +-- KafkaEventBus.cs
|   |   |   |   +-- KafkaEventBusOptions.cs
|   |   |   |   +-- KafkaEventConsumerBackgroundService.cs
|   |   |   |   +-- KafkaOutboxProcessor.cs
|   |   |   |   +-- KafkaOutboxProcessorBackgroundService.cs
|   |   |   |   \-- LHA.EventBus.Kafka.csproj
|   |   |   \-- LHA.EventBus.RabbitMQ
|   |   |       +-- DependencyInjection.cs
|   |   |       +-- LHA.EventBus.RabbitMQ.csproj
|   |   |       +-- RabbitMqEventBus.cs
|   |   |       +-- RabbitMqEventBusOptions.cs
|   |   |       +-- RabbitMqEventConsumerBackgroundService.cs
|   |   |       \-- RabbitMqOutboxProcessor.cs
|   |   +-- Grpc
|   |   |   +-- documents
|   |   |   |   \-- README.md
|   |   |   +-- LHA.Grpc.Client
|   |   |   |   +-- Interceptors
|   |   |   |   |   +-- ClientLoggingInterceptor.cs
|   |   |   |   |   \-- DeadlinePropagationInterceptor.cs
|   |   |   |   +-- Streaming
|   |   |   |   |   \-- StreamingExtensions.cs
|   |   |   |   +-- DependencyInjection.cs
|   |   |   |   +-- GrpcClientOptions.cs
|   |   |   |   \-- LHA.Grpc.Client.csproj
|   |   |   +-- LHA.Grpc.Contracts
|   |   |   |   \-- LHA.Grpc.Contracts.csproj
|   |   |   \-- LHA.Grpc.Server
|   |   |       +-- Interceptors
|   |   |       |   +-- ExceptionInterceptor.cs
|   |   |       |   +-- LoggingInterceptor.cs
|   |   |       |   +-- MetricsInterceptor.cs
|   |   |       |   \-- RateLimitInterceptor.cs
|   |   |       +-- Streaming
|   |   |       |   \-- ChunkedStreamHelper.cs
|   |   |       +-- DependencyInjection.cs
|   |   |       +-- GrpcServerOptions.cs
|   |   |       \-- LHA.Grpc.Server.csproj
|   |   +-- Localization
|   |   |   +-- LHA.Localization
|   |   |   |   +-- DependencyInjection.cs
|   |   |   |   +-- EmbeddedJsonLocalizationResourceReader.cs
|   |   |   |   +-- LHA.Localization.csproj
|   |   |   |   +-- LHAStringLocalizer.cs
|   |   |   |   +-- LHAStringLocalizerFactory.cs
|   |   |   |   \-- LocalizationResourceManager.cs
|   |   |   \-- LHA.Localization.Abstraction
|   |   |       +-- ILHAStringLocalizerFactory.cs
|   |   |       +-- ILocalizationResource.cs
|   |   |       +-- ILocalizationResourceContributor.cs
|   |   |       +-- ILocalizationResourceReader.cs
|   |   |       +-- LHA.Localization.Abstraction.csproj
|   |   |       +-- LocalizationResourceAttribute.cs
|   |   |       +-- LocalizationResourceDescriptor.cs
|   |   |       \-- LocalizationResourceOptions.cs
|   |   +-- MessageBroker
|   |   |   +-- LHA.MessageBroker.Abstraction
|   |   |   |   +-- IMessageBrokerHealthCheck.cs
|   |   |   |   +-- IMessageHandler.cs
|   |   |   |   +-- IMessagePublisher.cs
|   |   |   |   +-- IMessageSerializer.cs
|   |   |   |   +-- LHA.MessageBroker.Abstraction.csproj
|   |   |   |   +-- MessageContext.cs
|   |   |   |   +-- MessageEnvelope.cs
|   |   |   |   +-- MessageHeaders.cs
|   |   |   |   +-- PublishResult.cs
|   |   |   |   +-- SystemTextJsonMessageSerializer.cs
|   |   |   |   \-- TenantTopicStrategy.cs
|   |   |   +-- LHA.MessageBroker.Kafka
|   |   |   |   +-- DependencyInjection.cs
|   |   |   |   +-- KafkaConnectionFactory.cs
|   |   |   |   +-- KafkaConsumerBackgroundService.cs
|   |   |   |   +-- KafkaHealthCheck.cs
|   |   |   |   +-- KafkaMetadataExtensions.cs
|   |   |   |   +-- KafkaOptions.cs
|   |   |   |   +-- KafkaProducer.cs
|   |   |   |   \-- LHA.MessageBroker.Kafka.csproj
|   |   |   \-- LHA.MessageBroker.RabbitMQ
|   |   |       +-- DependencyInjection.cs
|   |   |       +-- LHA.MessageBroker.RabbitMQ.csproj
|   |   |       +-- RabbitMqConnectionManager.cs
|   |   |       +-- RabbitMqConsumerBackgroundService.cs
|   |   |       +-- RabbitMqHealthCheck.cs
|   |   |       +-- RabbitMqMessagePublisher.cs
|   |   |       +-- RabbitMqMetadataExtensions.cs
|   |   |       \-- RabbitMqOptions.cs
|   |   +-- MultiTenancy
|   |   |   +-- LHA.MultiTenancy
|   |   |   |   +-- ConfigurationTenantStore.cs
|   |   |   |   +-- LHA.MultiTenancy.csproj
|   |   |   |   +-- MultiTenancyHostingServiceCollectionExtensions.cs
|   |   |   |   +-- MultiTenantConnectionStringResolver.cs
|   |   |   |   \-- TenantResolveContributorBase.cs
|   |   |   \-- LHA.MultiTenancy.Abstraction
|   |   |       +-- Localization
|   |   |       |   +-- en.json
|   |   |       |   +-- MultiTenancyResource.cs
|   |   |       |   \-- vi.json
|   |   |       +-- AsyncLocalCurrentTenantAccessor.cs
|   |   |       +-- BasicTenantInfo.cs
|   |   |       +-- CMultiTenancySidesType.cs
|   |   |       +-- CTenantCircuitStateType.cs
|   |   |       +-- CurrentTenant.cs
|   |   |       +-- CurrentTenantExtensions.cs
|   |   |       +-- DataResidencyRegion.cs
|   |   |       +-- ICurrentTenant.cs
|   |   |       +-- ICurrentTenantAccessor.cs
|   |   |       +-- IgnoreMultiTenancyAttribute.cs
|   |   |       +-- IMultiTenant.cs
|   |   |       +-- ITenantCircuitBreaker.cs
|   |   |       +-- ITenantResolveContributor.cs
|   |   |       +-- ITenantResolver.cs
|   |   |       +-- ITenantStore.cs
|   |   |       +-- LHA.MultiTenancy.Abstraction.csproj
|   |   |       +-- MultiTenancyOptions.cs
|   |   |       +-- MultiTenancyServiceCollectionExtensions.cs
|   |   |       +-- NullTenantStore.cs
|   |   |       +-- TenantCircuitBreaker.cs
|   |   |       +-- TenantCircuitBreakerOptions.cs
|   |   |       +-- TenantConfiguration.cs
|   |   |       +-- TenantResolveContext.cs
|   |   |       +-- TenantResolver.cs
|   |   |       \-- TenantResolveResult.cs
|   |   +-- Scheduling
|   |   |   +-- LHA.Scheduling.Abstraction
|   |   |   |   +-- CronPresets.cs
|   |   |   |   +-- IJobScheduler.cs
|   |   |   |   +-- IRecurringJobManager.cs
|   |   |   |   +-- IScheduledJob.cs
|   |   |   |   +-- ISchedulingHealthCheck.cs
|   |   |   |   +-- JobContext.cs
|   |   |   |   +-- JobOptions.cs
|   |   |   |   +-- JobResult.cs
|   |   |   |   +-- LHA.Scheduling.Abstraction.csproj
|   |   |   |   \-- RecurringJobDefinition.cs
|   |   |   +-- LHA.Scheduling.Hangfire
|   |   |   |   +-- HangfireHealthCheck.cs
|   |   |   |   +-- HangfireJobExecutor.cs
|   |   |   |   +-- HangfireJobScheduler.cs
|   |   |   |   +-- HangfireMetadataExtensions.cs
|   |   |   |   +-- HangfireRecurringJobManager.cs
|   |   |   |   +-- HangfireSchedulingOptions.cs
|   |   |   |   +-- HangfireServiceCollectionExtensions.cs
|   |   |   |   \-- LHA.Scheduling.Hangfire.csproj
|   |   |   \-- LHA.Scheduling.Quartz
|   |   |       +-- LHA.Scheduling.Quartz.csproj
|   |   |       +-- QuartzHealthCheck.cs
|   |   |       +-- QuartzJobAdapter.cs
|   |   |       +-- QuartzJobScheduler.cs
|   |   |       +-- QuartzMetadataExtensions.cs
|   |   |       +-- QuartzRecurringJobManager.cs
|   |   |       +-- QuartzSchedulingOptions.cs
|   |   |       \-- QuartzServiceCollectionExtensions.cs
|   |   +-- Swagger
|   |   |   \-- LHA.Swagger
|   |   |       +-- DependencyInjection.cs
|   |   |       +-- LHA.Swagger.csproj
|   |   |       +-- LhaSwaggerOptions.cs
|   |   |       \-- Transformers.cs
|   |   \-- UnitOfWork
|   |       \-- LHA.UnitOfWork
|   |           +-- AmbientUnitOfWork.cs
|   |           +-- ChildUnitOfWork.cs
|   |           +-- IDatabaseApi.cs
|   |           +-- ISupportsRollback.cs
|   |           +-- ISupportsSavingChanges.cs
|   |           +-- ITransactionApi.cs
|   |           +-- IUnitOfWork.cs
|   |           +-- IUnitOfWorkEventPublisher.cs
|   |           +-- IUnitOfWorkManager.cs
|   |           +-- LHA.UnitOfWork.csproj
|   |           +-- UnitOfWork.cs
|   |           +-- UnitOfWorkAttribute.cs
|   |           +-- UnitOfWorkDefaultOptions.cs
|   |           +-- UnitOfWorkEventArgs.cs
|   |           +-- UnitOfWorkEventRecord.cs
|   |           +-- UnitOfWorkExtensions.cs
|   |           +-- UnitOfWorkFailedEventArgs.cs
|   |           +-- UnitOfWorkManager.cs
|   |           +-- UnitOfWorkOptions.cs
|   |           \-- UnitOfWorkServiceCollectionExtensions.cs
|   +-- modules
|   |   +-- audit-log
|   |   |   +-- LHA.AuditLog.Application
|   |   |   |   +-- AuditLogAppService.cs
|   |   |   |   +-- DependencyInjection.cs
|   |   |   |   +-- GlobalUsings.cs
|   |   |   |   \-- LHA.AuditLog.Application.csproj
|   |   |   +-- LHA.AuditLog.Application.Contracts
|   |   |   |   +-- IAuditLogAppService.cs
|   |   |   |   \-- LHA.AuditLog.Application.Contracts.csproj
|   |   |   +-- LHA.AuditLog.BackgroundWorker
|   |   |   |   +-- appsettings.json
|   |   |   |   +-- LHA.AuditLog.BackgroundWorker.csproj
|   |   |   |   +-- Program.cs
|   |   |   |   \-- Worker.cs
|   |   |   +-- LHA.AuditLog.Consumer
|   |   |   |   +-- appsettings.json
|   |   |   |   +-- LHA.AuditLog.Consumer.csproj
|   |   |   |   \-- Program.cs
|   |   |   +-- LHA.AuditLog.Domain
|   |   |   |   +-- AuditLogActionEntity.cs
|   |   |   |   +-- AuditLogEntity.cs
|   |   |   |   +-- EntityChangeEntity.cs
|   |   |   |   +-- EntityPropertyChangeEntity.cs
|   |   |   |   +-- IAuditLogRepository.cs
|   |   |   |   \-- LHA.AuditLog.Domain.csproj
|   |   |   +-- LHA.AuditLog.Domain.Shared
|   |   |   |   +-- AuditLogConsts.cs
|   |   |   |   \-- LHA.AuditLog.Domain.Shared.csproj
|   |   |   +-- LHA.AuditLog.EntityFrameworkCore
|   |   |   |   +-- Migrations
|   |   |   |   |   +-- 20260305035343_InitAuditLogDb.cs
|   |   |   |   |   +-- 20260305035343_InitAuditLogDb.Designer.cs
|   |   |   |   |   \-- AuditLogDbContextModelSnapshot.cs
|   |   |   |   +-- AuditLogDbContext.cs
|   |   |   |   +-- AuditLogDbContextFactory.cs
|   |   |   |   +-- AuditLogDbContextModelCreatingExtensions.cs
|   |   |   |   +-- AuditLogFactory.cs
|   |   |   |   +-- DependencyInjection.cs
|   |   |   |   +-- EfCoreAuditingStore.cs
|   |   |   |   +-- EfCoreAuditLogRepository.cs
|   |   |   |   \-- LHA.AuditLog.EntityFrameworkCore.csproj
|   |   |   +-- LHA.AuditLog.HttpApi
|   |   |   |   +-- AuditLogEndpoints.cs
|   |   |   |   +-- GlobalUsings.cs
|   |   |   |   \-- LHA.AuditLog.HttpApi.csproj
|   |   |   +-- LHA.AuditLog.HttpApi.Host
|   |   |   |   +-- appsettings.json
|   |   |   |   +-- LHA.AuditLog.HttpApi.Host.csproj
|   |   |   |   \-- Program.cs
|   |   |   \-- LHA.AuditLog.Migrator
|   |   |       +-- appsettings.json
|   |   |       +-- LHA.AuditLog.Migrator.csproj
|   |   |       \-- Program.cs
|   |   +-- identity
|   |   |   +-- LHA.Identity.Application
|   |   |   |   +-- AuthAppService.cs
|   |   |   |   +-- BCryptPasswordHasher.cs
|   |   |   |   +-- DependencyInjection.cs
|   |   |   |   +-- GlobalUsings.cs
|   |   |   |   +-- IdentityClaimTypeAppService.cs
|   |   |   |   +-- IdentityRoleAppService.cs
|   |   |   |   +-- IdentitySecurityLogAppService.cs
|   |   |   |   +-- IdentityUserAppService.cs
|   |   |   |   +-- JwtTokenService.cs
|   |   |   |   +-- LHA.Identity.Application.csproj
|   |   |   |   +-- PermissionAppService.cs
|   |   |   |   \-- UpperInvariantLookupNormalizer.cs
|   |   |   +-- LHA.Identity.Application.Contracts
|   |   |   |   +-- IdentityAppServiceInterfaces.cs
|   |   |   |   +-- IdentityEtos.cs
|   |   |   |   \-- LHA.Identity.Application.Contracts.csproj
|   |   |   +-- LHA.Identity.BackgroundWorker
|   |   |   |   +-- appsettings.json
|   |   |   |   +-- LHA.Identity.BackgroundWorker.csproj
|   |   |   |   +-- Program.cs
|   |   |   |   \-- Worker.cs
|   |   |   +-- LHA.Identity.Consumer
|   |   |   |   +-- appsettings.json
|   |   |   |   +-- LHA.Identity.Consumer.csproj
|   |   |   |   +-- Program.cs
|   |   |   |   \-- Worker.cs
|   |   |   +-- LHA.Identity.Domain
|   |   |   |   +-- IdentityClaimType.cs
|   |   |   |   +-- IdentityDomainEvents.cs
|   |   |   |   +-- IdentityPermissionGrant.cs
|   |   |   |   +-- IdentityRole.cs
|   |   |   |   +-- IdentityRoleManager.cs
|   |   |   |   +-- IdentitySecurityLog.cs
|   |   |   |   +-- IdentityServiceAbstractions.cs
|   |   |   |   +-- IdentityUser.cs
|   |   |   |   +-- IdentityUserManager.cs
|   |   |   |   +-- IdentityUserSubEntities.cs
|   |   |   |   +-- IIdentityClaimTypeRepository.cs
|   |   |   |   +-- IIdentityRoleRepository.cs
|   |   |   |   +-- IIdentitySecurityLogRepository.cs
|   |   |   |   +-- IIdentityUserRepository.cs
|   |   |   |   +-- IPermissionGrantRepository.cs
|   |   |   |   \-- LHA.Identity.Domain.csproj
|   |   |   +-- LHA.Identity.Domain.Shared
|   |   |   |   +-- Localization
|   |   |   |   |   +-- Identity
|   |   |   |   |   |   +-- en.json
|   |   |   |   |   |   \-- vi.json
|   |   |   |   |   \-- IdentityResource.cs
|   |   |   |   +-- IdentityConsts.cs
|   |   |   |   +-- IdentityDomainErrorCodes.cs
|   |   |   |   +-- IdentityEnums.cs
|   |   |   |   \-- LHA.Identity.Domain.Shared.csproj
|   |   |   +-- LHA.Identity.EntityFrameworkCore
|   |   |   |   +-- Migrations
|   |   |   |   |   +-- 20260307002019_InitIdentityDb.cs
|   |   |   |   |   +-- 20260307002019_InitIdentityDb.Designer.cs
|   |   |   |   |   \-- IdentityDbContextModelSnapshot.cs
|   |   |   |   +-- DependencyInjection.cs
|   |   |   |   +-- EfCoreIdentityClaimTypeRepository.cs
|   |   |   |   +-- EfCoreIdentityRoleRepository.cs
|   |   |   |   +-- EfCoreIdentitySecurityLogRepository.cs
|   |   |   |   +-- EfCoreIdentityUserRepository.cs
|   |   |   |   +-- EfCorePermissionGrantRepository.cs
|   |   |   |   +-- IdentityDbContext.cs
|   |   |   |   +-- IdentityDbContextFactory.cs
|   |   |   |   +-- IdentityDbContextModelCreatingExtensions.cs
|   |   |   |   \-- LHA.Identity.EntityFrameworkCore.csproj
|   |   |   +-- LHA.Identity.HttpApi
|   |   |   |   +-- Endpoints
|   |   |   |   |   +-- AuthEndpoints.cs
|   |   |   |   |   +-- ClaimTypeAndSecurityLogEndpoints.cs
|   |   |   |   |   +-- PermissionEndpoints.cs
|   |   |   |   |   +-- RoleEndpoints.cs
|   |   |   |   |   \-- UserEndpoints.cs
|   |   |   |   +-- GlobalUsings.cs
|   |   |   |   \-- LHA.Identity.HttpApi.csproj
|   |   |   +-- LHA.Identity.HttpApi.Host
|   |   |   |   +-- ApiEndpoint.http
|   |   |   |   +-- appsettings.Development.json
|   |   |   |   +-- appsettings.json
|   |   |   |   +-- LHA.Identity.HttpApi.Host.csproj
|   |   |   |   \-- Program.cs
|   |   |   \-- LHA.Identity.Migrator
|   |   |       +-- appsettings.json
|   |   |       +-- LHA.Identity.Migrator.csproj
|   |   |       \-- Program.cs
|   |   +-- permission-management
|   |   |   +-- LHA.PermissionManagement.Application
|   |   |   |   +-- DependencyInjection.cs
|   |   |   |   +-- GlobalUsings.cs
|   |   |   |   +-- LHA.PermissionManagement.Application.csproj
|   |   |   |   +-- PermissionDefinitionAppService.cs
|   |   |   |   +-- PermissionGrantAppService.cs
|   |   |   |   +-- PermissionGroupAppService.cs
|   |   |   |   \-- PermissionTemplateAppService.cs
|   |   |   +-- LHA.PermissionManagement.Application.Contracts
|   |   |   |   +-- IPermissionAppServices.cs
|   |   |   |   +-- LHA.PermissionManagement.Application.Contracts.csproj
|   |   |   |   \-- PermissionEtos.cs
|   |   |   +-- LHA.PermissionManagement.BackgroundWorker
|   |   |   |   +-- appsettings.Development.json
|   |   |   |   +-- appsettings.json
|   |   |   |   +-- LHA.PermissionManagement.BackgroundWorker.csproj
|   |   |   |   +-- Program.cs
|   |   |   |   \-- Worker.cs
|   |   |   +-- LHA.PermissionManagement.Consumer
|   |   |   |   +-- appsettings.Development.json
|   |   |   |   +-- appsettings.json
|   |   |   |   +-- LHA.PermissionManagement.Consumer.csproj
|   |   |   |   +-- Program.cs
|   |   |   |   \-- Worker.cs
|   |   |   +-- LHA.PermissionManagement.Domain
|   |   |   |   +-- LHA.PermissionManagement.Domain.csproj
|   |   |   |   +-- PermissionDefinition.cs
|   |   |   |   +-- PermissionDomainEvents.cs
|   |   |   |   +-- PermissionGrant.cs
|   |   |   |   +-- PermissionGroup.cs
|   |   |   |   +-- PermissionGroupItem.cs
|   |   |   |   +-- PermissionTemplate.cs
|   |   |   |   +-- PermissionTemplateItem.cs
|   |   |   |   \-- Repositories.cs
|   |   |   +-- LHA.PermissionManagement.Domain.Shared
|   |   |   |   +-- LHA.PermissionManagement.Domain.Shared.csproj
|   |   |   |   \-- PermissionConsts.cs
|   |   |   +-- LHA.PermissionManagement.EntityFrameworkCore
|   |   |   |   +-- Migrations
|   |   |   |   |   +-- 20260305094314_InitPermissionDb.cs
|   |   |   |   |   +-- 20260305094314_InitPermissionDb.Designer.cs
|   |   |   |   |   \-- PermissionManagementDbContextModelSnapshot.cs
|   |   |   |   +-- DependencyInjection.cs
|   |   |   |   +-- EfCoreRepositories.cs
|   |   |   |   +-- LHA.PermissionManagement.EntityFrameworkCore.csproj
|   |   |   |   +-- PermissionManagementDbContext.cs
|   |   |   |   +-- PermissionManagementDbContextFactory.cs
|   |   |   |   \-- PermissionManagementDbContextModelCreatingExtensions.cs
|   |   |   +-- LHA.PermissionManagement.HttpApi
|   |   |   |   +-- GlobalUsings.cs
|   |   |   |   +-- LHA.PermissionManagement.HttpApi.csproj
|   |   |   |   \-- PermissionEndpoints.cs
|   |   |   +-- LHA.PermissionManagement.HttpApi.Host
|   |   |   |   +-- appsettings.Development.json
|   |   |   |   +-- appsettings.json
|   |   |   |   +-- LHA.PermissionManagement.HttpApi.Host.csproj
|   |   |   |   \-- Program.cs
|   |   |   \-- LHA.PermissionManagement.Migrator
|   |   |       +-- appsettings.json
|   |   |       +-- LHA.PermissionManagement.Migrator.csproj
|   |   |       \-- Program.cs
|   |   \-- tenant-management
|   |       +-- LHA.TenantManagement.Application
|   |       |   +-- DependencyInjection.cs
|   |       |   +-- GlobalUsings.cs
|   |       |   +-- LHA.TenantManagement.Application.csproj
|   |       |   \-- TenantAppService.cs
|   |       +-- LHA.TenantManagement.Application.Contracts
|   |       |   +-- ITenantAppService.cs
|   |       |   +-- LHA.TenantManagement.Application.Contracts.csproj
|   |       |   \-- TenantEtos.cs
|   |       +-- LHA.TenantManagement.BackgroundWorker
|   |       |   +-- appsettings.Development.json
|   |       |   +-- appsettings.json
|   |       |   +-- LHA.TenantManagement.BackgroundWorker.csproj
|   |       |   +-- Program.cs
|   |       |   \-- Worker.cs
|   |       +-- LHA.TenantManagement.Consumer
|   |       |   +-- appsettings.Development.json
|   |       |   +-- appsettings.json
|   |       |   +-- LHA.TenantManagement.Consumer.csproj
|   |       |   +-- Program.cs
|   |       |   \-- Worker.cs
|   |       +-- LHA.TenantManagement.Domain
|   |       |   +-- ITenantRepository.cs
|   |       |   +-- LHA.TenantManagement.Domain.csproj
|   |       |   +-- Tenant.cs
|   |       |   +-- TenantConnectionString.cs
|   |       |   +-- TenantDomainEvents.cs
|   |       |   \-- TenantManager.cs
|   |       +-- LHA.TenantManagement.Domain.Shared
|   |       |   +-- CMultiTenancyDatabaseStyle.cs
|   |       |   +-- LHA.TenantManagement.Domain.Shared.csproj
|   |       |   \-- TenantConsts.cs
|   |       +-- LHA.TenantManagement.EntityFrameworkCore
|   |       |   +-- Migrations
|   |       |   |   +-- 20260304021950_InitTenantDb.cs
|   |       |   |   +-- 20260304021950_InitTenantDb.Designer.cs
|   |       |   |   \-- TenantManagementDbContextModelSnapshot.cs
|   |       |   +-- DependencyInjection.cs
|   |       |   +-- EfCoreTenantRepository.cs
|   |       |   +-- EfCoreTenantStore.cs
|   |       |   +-- LHA.TenantManagement.EntityFrameworkCore.csproj
|   |       |   +-- TenantManagementDbContext.cs
|   |       |   +-- TenantManagementDbContextFactory.cs
|   |       |   \-- TenantManagementDbContextModelCreatingExtensions.cs
|   |       +-- LHA.TenantManagement.HttpApi
|   |       |   +-- GlobalUsings.cs
|   |       |   +-- LHA.TenantManagement.HttpApi.csproj
|   |       |   \-- TenantEndpoints.cs
|   |       +-- LHA.TenantManagement.HttpApi.Host
|   |       |   +-- appsettings.Development.json
|   |       |   +-- appsettings.json
|   |       |   +-- LHA.TenantManagement.HttpApi.Host.csproj
|   |       |   \-- Program.cs
|   |       \-- LHA.TenantManagement.Migrator
|   |           +-- appsettings.json
|   |           +-- LHA.TenantManagement.Migrator.csproj
|   |           \-- Program.cs
|   +-- services
|   |   +-- account
|   |   |   +-- LHA.Account.Application
|   |   |   |   +-- Permissions
|   |   |   |   |   \-- PermissionRegistrationService.cs
|   |   |   |   +-- DependencyInjection.cs
|   |   |   |   \-- LHA.Account.Application.csproj
|   |   |   +-- LHA.Account.Application.Contracts
|   |   |   |   +-- Permissions
|   |   |   |   |   +-- AccountPermissions.cs
|   |   |   |   |   +-- IPermissionRegistrationService.cs
|   |   |   |   |   \-- RegisterServicePermissionsInput.cs
|   |   |   |   \-- LHA.Account.Application.Contracts.csproj
|   |   |   +-- LHA.Account.Consumer
|   |   |   |   +-- EventHandlers
|   |   |   |   |   +-- MegaAccountCreatedEventHandler.cs
|   |   |   |   |   +-- MegaAccountDeletedEventHandler.cs
|   |   |   |   |   \-- MegaAccountUpdatedEventHandler.cs
|   |   |   |   +-- appsettings.Development.json
|   |   |   |   +-- appsettings.json
|   |   |   |   +-- LHA.Account.Consumer.csproj
|   |   |   |   +-- Program.cs
|   |   |   |   \-- Worker.cs
|   |   |   +-- LHA.Account.Cron
|   |   |   |   +-- appsettings.Development.json
|   |   |   |   +-- appsettings.json
|   |   |   |   +-- LHA.Account.Cron.csproj
|   |   |   |   +-- Program.cs
|   |   |   |   \-- Worker.cs
|   |   |   +-- LHA.Account.Domain
|   |   |   |   \-- LHA.Account.Domain.csproj
|   |   |   +-- LHA.Account.Domain.Shared
|   |   |   |   +-- Localization
|   |   |   |   |   \-- AccountResource.cs
|   |   |   |   \-- LHA.Account.Domain.Shared.csproj
|   |   |   +-- LHA.Account.EntityFrameworkCore
|   |   |   |   +-- Contracts
|   |   |   |   |   \-- DbSchemeConsts.cs
|   |   |   |   +-- Migrations
|   |   |   |   |   +-- 20260305220139_InitAccountDb.cs
|   |   |   |   |   +-- 20260305220139_InitAccountDb.Designer.cs
|   |   |   |   |   \-- AccountDbContextModelSnapshot.cs
|   |   |   |   +-- AccountDbContext.cs
|   |   |   |   +-- AccountDbContextFactory.cs
|   |   |   |   +-- DependencyInjection.cs
|   |   |   |   \-- LHA.Account.EntityFrameworkCore.csproj
|   |   |   +-- LHA.Account.HttpApi
|   |   |   |   +-- Endpoints
|   |   |   |   |   +-- AccountEndpoints.cs
|   |   |   |   |   \-- InternalEndpoints.cs
|   |   |   |   +-- Grpc
|   |   |   |   |   \-- PermissionRegistrationGrpcService.cs
|   |   |   |   \-- LHA.Account.HttpApi.csproj
|   |   |   +-- LHA.Account.HttpApi.Host
|   |   |   |   +-- Properties
|   |   |   |   |   \-- launchSettings.json
|   |   |   |   +-- appsettings.Development.json
|   |   |   |   +-- appsettings.json
|   |   |   |   +-- LHA.Account.HttpApi.Host.csproj
|   |   |   |   \-- Program.cs
|   |   |   \-- LHA.Account.Migrator
|   |   |       +-- appsettings.json
|   |   |       +-- LHA.Account.Migrator.csproj
|   |   |       \-- Program.cs
|   |   +-- meaga
|   |   |   +-- LHA.Mega.Application
|   |   |   |   +-- Account
|   |   |   |   |   \-- MegaAccountAppService.cs
|   |   |   |   +-- DependencyInjection.cs
|   |   |   |   \-- LHA.Mega.Application.csproj
|   |   |   +-- LHA.Mega.Application.Contracts
|   |   |   |   +-- Account
|   |   |   |   |   +-- CreateMegaAccountInput.cs
|   |   |   |   |   +-- IMegaAccountAppService.cs
|   |   |   |   |   +-- MegaAccountDto.cs
|   |   |   |   |   \-- UpdateMegaAccountInput.cs
|   |   |   |   +-- Constants
|   |   |   |   |   \-- Permissions
|   |   |   |   |       \-- MegaPermissionConsts.cs
|   |   |   |   \-- LHA.Mega.Application.Contracts.csproj
|   |   |   +-- LHA.Mega.Consumer
|   |   |   |   +-- appsettings.Development.json
|   |   |   |   +-- appsettings.json
|   |   |   |   +-- LHA.Mega.Consumer.csproj
|   |   |   |   +-- Program.cs
|   |   |   |   \-- Worker.cs
|   |   |   +-- LHA.Mega.Cron
|   |   |   |   +-- appsettings.Development.json
|   |   |   |   +-- appsettings.json
|   |   |   |   +-- LHA.Mega.Cron.csproj
|   |   |   |   +-- Program.cs
|   |   |   |   \-- Worker.cs
|   |   |   +-- LHA.Mega.Domain
|   |   |   |   +-- Account
|   |   |   |   |   +-- IMegaAccountRepository.cs
|   |   |   |   |   \-- MegaAccountEntity.cs
|   |   |   |   \-- LHA.Mega.Domain.csproj
|   |   |   +-- LHA.Mega.Domain.Shared
|   |   |   |   +-- Events
|   |   |   |   |   +-- MegaAccountCreatedEvent.cs
|   |   |   |   |   +-- MegaAccountDeletedEvent.cs
|   |   |   |   |   \-- MegaAccountUpdatedEvent.cs
|   |   |   |   \-- LHA.Mega.Domain.Shared.csproj
|   |   |   +-- LHA.Mega.EntityFrameworkCore
|   |   |   |   +-- Account
|   |   |   |   |   \-- EfCoreMegaAccountRepository.cs
|   |   |   |   +-- Migrations
|   |   |   |   |   +-- 20260306011502_InitMegaDb.cs
|   |   |   |   |   +-- 20260306011502_InitMegaDb.Designer.cs
|   |   |   |   |   \-- MegaDbContextModelSnapshot.cs
|   |   |   |   +-- DependencyInjection.cs
|   |   |   |   +-- LHA.Mega.EntityFrameworkCore.csproj
|   |   |   |   +-- MegaDbContext.cs
|   |   |   |   +-- MegaDbContextFactory.cs
|   |   |   |   \-- MegaDbContextModelCreatingExtensions.cs
|   |   |   +-- LHA.Mega.HttpApi
|   |   |   |   +-- LHA.Mega.HttpApi.csproj
|   |   |   |   +-- MegaAccountEndpoints.cs
|   |   |   |   \-- MegaEndpoints.cs
|   |   |   +-- LHA.Mega.HttpApi.Host
|   |   |   |   +-- appsettings.Development.json
|   |   |   |   +-- appsettings.json
|   |   |   |   +-- LHA.Mega.HttpApi.Host.csproj
|   |   |   |   \-- Program.cs
|   |   |   \-- LHA.Mega.Migrator
|   |   |       +-- appsettings.json
|   |   |       +-- LHA.Mega.Migrator.csproj
|   |   |       \-- Program.cs
|   |   \-- movie
|   |       +-- LHA.Movie.Application
|   |       |   \-- LHA.Movie.Application.csproj
|   |       +-- LHA.Movie.Application.Contracts
|   |       |   \-- LHA.Movie.Application.Contracts.csproj
|   |       +-- LHA.Movie.Consumer
|   |       |   \-- LHA.Movie.Consumer.csproj
|   |       +-- LHA.Movie.Cron
|   |       |   \-- LHA.Movie.Cron.csproj
|   |       +-- LHA.Movie.Domain
|   |       |   +-- Actors
|   |       |   |   +-- ActorAliasEntity.cs
|   |       |   |   +-- ActorAliasTranslationEntity.cs
|   |       |   |   \-- ActorEntity.cs
|   |       |   \-- LHA.Movie.Domain.csproj
|   |       +-- LHA.Movie.Domain.Shared
|   |       |   \-- LHA.Movie.Domain.Shared.csproj
|   |       +-- LHA.Movie.EntityFrameworkCore
|   |       |   \-- LHA.Movie.EntityFrameworkCore.csproj
|   |       +-- LHA.Movie.HttpApi
|   |       |   \-- LHA.Movie.HttpApi.csproj
|   |       +-- LHA.Movie.HttpApi.Host
|   |       |   \-- LHA.Movie.HttpApi.Host.csproj
|   |       \-- LHA.Movie.Migrator
|   |           +-- appsettings.json
|   |           +-- LHA.Movie.Migrator.csproj
|   |           \-- Program.cs
|   +-- shared
|   |   +-- LHA.Shared.Contracts
|   |   |   +-- AuditLog
|   |   |   |   +-- AuditLogDtos.cs
|   |   |   |   +-- AuditLogInputs.cs
|   |   |   |   \-- AuditLogPermissions.cs
|   |   |   +-- Identity
|   |   |   |   +-- IdentityDtos.cs
|   |   |   |   +-- IdentityInputs.cs
|   |   |   |   \-- IdentityPermissions.cs
|   |   |   +-- PermissionManagement
|   |   |   |   +-- PermissionDtos.cs
|   |   |   |   +-- PermissionInputs.cs
|   |   |   |   \-- PermissionManagementPermissions.cs
|   |   |   +-- TenantManagement
|   |   |   |   +-- TenantDto.cs
|   |   |   |   +-- TenantInputs.cs
|   |   |   |   \-- TenantManagementPermissions.cs
|   |   |   \-- LHA.Shared.Contracts.csproj
|   |   \-- LHA.Shared.Domain
|   |       +-- Enums
|   |       |   +-- CBloodType.cs
|   |       |   +-- CBodyType.cs
|   |       |   +-- CCountryType.cs
|   |       |   +-- CCupSizeType.cs
|   |       |   +-- CLanguageType.cs
|   |       |   +-- CNudeType.cs
|   |       |   \-- CSexType.cs
|   |       \-- LHA.Shared.Domain.csproj
|   \-- WebUI
|       \-- blazorwasm
|           +-- LHA.BlazorWasm.App
|           |   +-- Layout
|           |   |   +-- EmptyLayout.razor
|           |   |   +-- MainLayout.razor
|           |   |   +-- MainLayout.razor.css
|           |   |   +-- NavMenu.razor
|           |   |   +-- NavMenu.razor.css
|           |   |   +-- SuperAdminLayout.razor
|           |   |   \-- TenantAdminLayout.razor
|           |   +-- Pages
|           |   |   +-- Auth
|           |   |   |   +-- TenantRegister.razor
|           |   |   |   \-- TenantRegister.razor.css
|           |   |   +-- Counter.razor
|           |   |   +-- EditorExample.razor
|           |   |   +-- Home.razor
|           |   |   +-- NotFound.razor
|           |   |   +-- Test.razor
|           |   |   \-- Weather.razor
|           |   +-- _Imports.razor
|           |   +-- App.razor
|           |   +-- LHA.BlazorWasm.App.csproj
|           |   +-- MockAccessTokenProvider.cs
|           |   +-- Program.cs
|           |   \-- StatusBadgeModuleRegistration.cs
|           +-- LHA.BlazorWasm.Components
|           |   +-- Badges
|           |   |   +-- StatusBadge.razor
|           |   |   +-- StatusBadge.razor.cs
|           |   |   \-- StatusBadge.razor.css
|           |   +-- Breadcrumb
|           |   |   +-- Breadcrumb.razor
|           |   |   +-- Breadcrumb.razor.cs
|           |   |   +-- Breadcrumb.razor.css
|           |   |   +-- BreadcrumbItem.razor
|           |   |   +-- BreadcrumbItem.razor.cs
|           |   |   \-- BreadcrumbItemModel.cs
|           |   +-- Buttons
|           |   |   +-- Button.razor
|           |   |   +-- Button.razor.cs
|           |   |   +-- Button.razor.css
|           |   |   +-- CButtonIconPosition.cs
|           |   |   +-- CButtonSize.cs
|           |   |   +-- CButtonStyle.cs
|           |   |   \-- CButtonType.cs
|           |   +-- Emoji
|           |   |   +-- CEmojiCategory.cs
|           |   |   +-- EmojiCategoryBar.razor
|           |   |   +-- EmojiGrid.razor
|           |   |   +-- EmojiItem.razor
|           |   |   +-- EmojiModel.cs
|           |   |   +-- EmojiPicker.razor
|           |   |   +-- EmojiPicker.razor.cs
|           |   |   +-- EmojiPicker.razor.css
|           |   |   \-- EmojiSearch.razor
|           |   +-- Errors
|           |   |   +-- GlobalErrorBoundary.razor
|           |   |   +-- GlobalErrorBoundary.razor.cs
|           |   |   +-- GlobalErrorBoundary.razor.css
|           |   |   +-- LhaErrorBoundaryBase.cs
|           |   |   +-- NotFoundPage.razor
|           |   |   +-- NotFoundPage.razor.cs
|           |   |   \-- NotFoundPage.razor.css
|           |   +-- Form
|           |   |   +-- CFormFieldLayout.cs
|           |   |   +-- FormField.razor
|           |   |   +-- FormField.razor.cs
|           |   |   +-- FormField.razor.css
|           |   |   +-- FormHelp.razor
|           |   |   +-- FormLabel.razor
|           |   |   \-- FormMessage.razor
|           |   +-- LanguageSelector
|           |   |   +-- LanguageSelector.razor
|           |   |   +-- LanguageSelector.razor.cs
|           |   |   +-- LanguageSelector.razor.css
|           |   |   \-- LanguageSelector.razor.js
|           |   +-- Pickers
|           |   |   +-- Core
|           |   |   |   +-- CalendarView.razor
|           |   |   |   +-- CValidationStatus.cs
|           |   |   |   +-- DateRange.cs
|           |   |   |   +-- DateUtils.cs
|           |   |   |   +-- IPickerValueConverter.cs
|           |   |   |   +-- PickerBase.cs
|           |   |   |   +-- PickerPopup.razor
|           |   |   |   +-- PickerState.cs
|           |   |   |   +-- TimeView.razor
|           |   |   |   \-- ValidationMessage.razor
|           |   |   +-- DatePicker
|           |   |   |   +-- DatePicker.razor
|           |   |   |   +-- DatePicker.razor.cs
|           |   |   |   \-- DatePicker.razor.css
|           |   |   +-- DateRangePicker
|           |   |   |   +-- DateRangePicker.razor
|           |   |   |   +-- DateRangePicker.razor.cs
|           |   |   |   \-- DateRangePicker.razor.css
|           |   |   +-- DateTimePicker
|           |   |   |   +-- DateTimePicker.razor
|           |   |   |   +-- DateTimePicker.razor.cs
|           |   |   |   \-- DateTimePicker.razor.css
|           |   |   +-- DateTimeRangePicker
|           |   |   |   +-- DateTimeRangePicker.razor
|           |   |   |   +-- DateTimeRangePicker.razor.cs
|           |   |   |   \-- DateTimeRangePicker.razor.css
|           |   |   +-- TimePicker
|           |   |   |   +-- TimePicker.razor
|           |   |   |   +-- TimePicker.razor.cs
|           |   |   |   \-- TimePicker.razor.css
|           |   |   \-- TimeRangePicker
|           |   |       +-- TimeRangePicker.razor
|           |   |       +-- TimeRangePicker.razor.cs
|           |   |       \-- TimeRangePicker.razor.css
|           |   +-- RichTextEditor
|           |   |   +-- Components
|           |   |   |   +-- CodeBlockDialog.razor
|           |   |   |   +-- CodeBlockDialog.razor.cs
|           |   |   |   +-- ColorPickerPopup.razor
|           |   |   |   +-- ColorPickerPopup.razor.cs
|           |   |   |   +-- DragDropDialog.razor
|           |   |   |   +-- DragDropDialog.razor.cs
|           |   |   |   +-- EditorContent.razor
|           |   |   |   +-- EditorStatusBar.razor
|           |   |   |   +-- EditorToolbar.razor
|           |   |   |   +-- EditorToolbar.razor.cs
|           |   |   |   +-- Icons.cs
|           |   |   |   +-- ImageDialog.razor
|           |   |   |   +-- ImageDialog.razor.cs
|           |   |   |   +-- LinkDialog.razor
|           |   |   |   +-- LinkDialog.razor.cs
|           |   |   |   +-- RichTextEditor.razor
|           |   |   |   +-- RichTextEditor.razor.cs
|           |   |   |   +-- SpecialCharsDialog.razor
|           |   |   |   +-- SpecialCharsDialog.razor.cs
|           |   |   |   +-- SpecialCharsPopup.razor
|           |   |   |   +-- SpecialCharsPopup.razor.cs
|           |   |   |   +-- TableDialog.razor
|           |   |   |   +-- TableDialog.razor.cs
|           |   |   |   +-- ToolbarButton.razor
|           |   |   |   +-- ToolbarButton.razor.cs
|           |   |   |   +-- ToolbarDropdown.razor
|           |   |   |   +-- ToolbarDropdown.razor.cs
|           |   |   |   \-- ToolbarSeparator.razor
|           |   |   +-- Interop
|           |   |   |   \-- RichTextEditorInterop.cs
|           |   |   \-- Models
|           |   |       +-- CEditorCommand.cs
|           |   |       +-- EditorEnums.cs
|           |   |       +-- EditorOptions.cs
|           |   |       +-- EditorState.cs
|           |   |       \-- ToolbarConfig.cs
|           |   +-- Section
|           |   |   +-- CSectionVariant.cs
|           |   |   +-- Section.razor
|           |   |   +-- Section.razor.cs
|           |   |   \-- Section.razor.css
|           |   +-- Select
|           |   |   +-- CSelectMode.cs
|           |   |   +-- CSelectPlacement.cs
|           |   |   +-- Select.razor
|           |   |   +-- Select.razor.cs
|           |   |   +-- Select.razor.css
|           |   |   +-- Select.razor.js
|           |   |   +-- SelectItem.razor
|           |   |   +-- SelectItem.razor.cs
|           |   |   +-- SelectOption.cs
|           |   |   +-- SelectSearch.razor
|           |   |   +-- SelectState.cs
|           |   |   \-- SelectVirtualList.razor
|           |   +-- Sidebar
|           |   |   +-- Models
|           |   |   |   +-- CSidebarState.cs
|           |   |   |   \-- SidebarItemModel.cs
|           |   |   +-- Sidebar.razor
|           |   |   +-- Sidebar.razor.cs
|           |   |   +-- Sidebar.razor.css
|           |   |   +-- Sidebar.razor.js
|           |   |   +-- SidebarItem.razor
|           |   |   +-- SidebarItem.razor.cs
|           |   |   \-- SidebarItem.razor.css
|           |   +-- Skeleton
|           |   |   +-- CSkeletonAnimation.cs
|           |   |   +-- CSkeletonVariant.cs
|           |   |   +-- Skeleton.razor
|           |   |   +-- Skeleton.razor.cs
|           |   |   \-- Skeleton.razor.css
|           |   +-- Switch
|           |   |   +-- CSwitchLabelPosition.cs
|           |   |   +-- CSwitchSize.cs
|           |   |   +-- Switch.razor
|           |   |   +-- Switch.razor.cs
|           |   |   \-- Switch.razor.css
|           |   +-- ThemeSwitch
|           |   |   +-- CThemeSwitchVariant.cs
|           |   |   +-- ThemeSwitch.razor
|           |   |   +-- ThemeSwitch.razor.cs
|           |   |   \-- ThemeSwitch.razor.css
|           |   +-- Toast
|           |   |   +-- Toast.razor
|           |   |   +-- Toast.razor.cs
|           |   |   +-- Toast.razor.css
|           |   |   +-- ToastContainer.razor
|           |   |   +-- ToastContainer.razor.cs
|           |   |   \-- ToastContainer.razor.css
|           |   +-- Tooltip
|           |   |   +-- CTooltipPlacement.cs
|           |   |   +-- CTooltipTrigger.cs
|           |   |   +-- Tooltip.razor
|           |   |   +-- Tooltip.razor.cs
|           |   |   +-- Tooltip.razor.css
|           |   |   \-- Tooltip.razor.js
|           |   +-- Topbar
|           |   |   +-- ITopbarService.cs
|           |   |   +-- NotificationBell.razor
|           |   |   +-- NotificationBell.razor.cs
|           |   |   +-- NotificationBell.razor.css
|           |   |   +-- Topbar.razor
|           |   |   +-- Topbar.razor.cs
|           |   |   +-- Topbar.razor.css
|           |   |   +-- TopbarItem.razor
|           |   |   +-- TopbarItem.razor.cs
|           |   |   +-- TopbarModels.cs
|           |   |   +-- TopbarService.cs
|           |   |   \-- TopbarState.cs
|           |   +-- _Imports.razor
|           |   +-- ComponentExtensions.cs
|           |   +-- LHA.BlazorWasm.Components.csproj
|           |   \-- LhaComponentBase.cs
|           +-- LHA.BlazorWasm.HttpApi.Client
|           |   +-- Abstractions
|           |   |   +-- IAccessTokenProvider.cs
|           |   |   +-- IApiClient.cs
|           |   |   +-- IApiErrorHandler.cs
|           |   |   \-- IClientContextProvider.cs
|           |   +-- Clients
|           |   |   \-- ExampleApiClient.cs
|           |   +-- Core
|           |   |   +-- ApiClientBase.cs
|           |   |   +-- ApiError.cs
|           |   |   +-- ApiException.cs
|           |   |   +-- DefaultApiErrorHandler.cs
|           |   |   \-- DefaultClientContextProvider.cs
|           |   +-- Extensions
|           |   |   \-- HttpApiClientExtensions.cs
|           |   +-- Handlers
|           |   |   +-- AuthMessageHandler.cs
|           |   |   +-- ContextMessageHandler.cs
|           |   |   +-- LoggingMessageHandler.cs
|           |   |   +-- RetryMessageHandler.cs
|           |   |   \-- SecureHttpHandler.cs
|           |   +-- Options
|           |   |   \-- HttpApiClientOptions.cs
|           |   +-- Serialization
|           |   |   \-- JsonOptionsProvider.cs
|           |   \-- LHA.BlazorWasm.HttpApi.Client.csproj
|           +-- LHA.BlazorWasm.Modules
|           |   \-- LHA.BlazorWasm.Modules.csproj
|           +-- LHA.BlazorWasm.Services
|           |   +-- ErrorHandling
|           |   |   +-- ErrorReporter.cs
|           |   |   +-- ErrorReportingExtensions.cs
|           |   |   +-- IErrorReporter.cs
|           |   |   \-- ToastApiErrorHandler.cs
|           |   +-- Localization
|           |   |   +-- CLanguageCode.cs
|           |   |   +-- CLanguageSelectorMode.cs
|           |   |   +-- LanguageOption.cs
|           |   |   +-- LanguageProvider.cs
|           |   |   +-- LocalizationExtensions.cs
|           |   |   +-- LocalizationOptions.cs
|           |   |   \-- LocalizationService.cs
|           |   +-- StatusBadge
|           |   |   +-- IStatusBadgeService.cs
|           |   |   +-- StatusBadgeConfiguration.cs
|           |   |   \-- StatusBadgeService.cs
|           |   +-- Storage
|           |   |   +-- ILocalStorageService.cs
|           |   |   +-- LocalStorageService.cs
|           |   |   +-- StorageExtensions.cs
|           |   |   \-- StorageOptions.cs
|           |   +-- Theme
|           |   |   +-- CThemeMode.cs
|           |   |   +-- IThemeService.cs
|           |   |   +-- ThemeExtensions.cs
|           |   |   +-- ThemeService.cs
|           |   |   \-- ThemeState.cs
|           |   +-- Toast
|           |   |   +-- CToastLevel.cs
|           |   |   +-- IToastService.cs
|           |   |   +-- ToastExtensions.cs
|           |   |   +-- ToastMessage.cs
|           |   |   +-- ToastService.cs
|           |   |   \-- ToastState.cs
|           |   \-- LHA.BlazorWasm.Services.csproj
|           +-- LHA.BlazorWasm.Shared
|           |   +-- Abstractions
|           |   |   \-- Localization
|           |   |       \-- ILocalizationService.cs
|           |   +-- Constants
|           |   |   +-- Formatters
|           |   |   |   \-- DateTimeFormatter.cs
|           |   |   \-- CustomHttpHeaderNames.cs
|           |   +-- Models
|           |   |   +-- Localization
|           |   |   |   \-- LocalizationState.cs
|           |   |   +-- StatusBadge
|           |   |   |   \-- StatusBadgeModels.cs
|           |   |   \-- ExampleEnums.cs
|           |   \-- LHA.BlazorWasm.Shared.csproj
|           \-- LHA.Security
|               +-- Device
|               |   \-- DeviceFingerprintService.cs
|               +-- Encryption
|               |   +-- AesEncryptionService.cs
|               |   \-- RsaEncryptionService.cs
|               +-- Keys
|               |   \-- KeyRotationService.cs
|               +-- Middleware
|               |   \-- SecureRequestMiddleware.cs
|               +-- Options
|               |   \-- SecurityOptions.cs
|               +-- ReplayProtection
|               |   \-- ReplayProtectionService.cs
|               +-- Signing
|               |   \-- RequestSigner.cs
|               \-- LHA.Security.csproj
+-- test
|   \-- Test.API
|       +-- Properties
|       |   \-- launchSettings.json
|       +-- appsettings.Development.json
|       +-- appsettings.json
|       +-- Program.cs
|       +-- Test.API.csproj
|       \-- Test.API.http
+-- .dockerignore
+-- .gitignore
+-- Dockerfile
+-- LHA.Solution.slnx
+-- nginx.conf
\-- render.yaml
```
