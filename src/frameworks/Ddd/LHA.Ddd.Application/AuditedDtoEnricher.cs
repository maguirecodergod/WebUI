using System.Collections;
using System.Reflection;
using LHA.Caching;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using LHA.Core.Users;

namespace LHA.Ddd.Application;

public class AuditedDtoEnricher : IAuditedDtoEnricher
{
    private readonly ITypedDistributedCache<AuditorCacheItem, Guid> _cache;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<AuditedDtoEnricher> _logger;

    // Cache to store the properties of type AuditActionDto for each DTO type to avoid repeated reflection overhead
    private static readonly System.Collections.Concurrent.ConcurrentDictionary<Type, PropertyInfo[]> _typePropertiesCache = new();

    private static readonly Dictionary<Guid, AuditorCacheItem> _systemAuditors = new()
    {
        { CurrentUserDefaults.SystemUserId, new AuditorCacheItem { 
            Id = CurrentUserDefaults.SystemUserId, 
            Name = CurrentUserDefaults.SystemUserName, 
            Email = CurrentUserDefaults.SystemUserEmail 
        } },
        { CurrentUserDefaults.AdminUserId, new AuditorCacheItem { 
            Id = CurrentUserDefaults.AdminUserId, 
            Name = CurrentUserDefaults.AdminUserName, 
            Email = CurrentUserDefaults.AdminUserEmail 
        } },
        { CurrentUserDefaults.AnonymousUserId, new AuditorCacheItem { 
            Id = CurrentUserDefaults.AnonymousUserId, 
            Name = CurrentUserDefaults.AnonymousUserName, 
            Email = "" 
        } }
    };

    public AuditedDtoEnricher(
        ITypedDistributedCache<AuditorCacheItem, Guid> cache,
        IServiceProvider serviceProvider,
        ILogger<AuditedDtoEnricher> logger)
    {
        _cache = cache;
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task EnrichAsync(object? obj)
    {
        if (obj == null) return;

        var items = ExtractItems(obj);
        if (items.Count == 0) return;

        var auditActions = new List<AuditActionDto>();

        foreach (var item in items)
        {
            if (item == null) continue;
            var type = item.GetType();
            var properties = _typePropertiesCache.GetOrAdd(type, t => 
                t.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                 .Where(p => p.PropertyType == typeof(AuditActionDto) && p.CanRead)
                 .ToArray());

            foreach (var prop in properties)
            {
                if (prop.GetValue(item) is AuditActionDto actionDto && actionDto.Actor != null && actionDto.Actor.Id.HasValue)
                {
                    auditActions.Add(actionDto);
                }
            }
        }

        if (auditActions.Count == 0) return;

        var finalData = new Dictionary<Guid, AuditorCacheItem>();
        var actorIdsToFetch = new List<Guid>();

        foreach (var action in auditActions)
        {
            var actorId = action.Actor!.Id!.Value;
            if (_systemAuditors.TryGetValue(actorId, out var systemAuditor))
            {
                finalData[actorId] = systemAuditor;
            }
            else
            {
                actorIdsToFetch.Add(actorId);
            }
        }

        if (actorIdsToFetch.Count > 0)
        {
            var cachedItems = await _cache.GetManyAsync(actorIdsToFetch.Distinct());
            var missingIds = new List<Guid>();

            foreach (var kvp in cachedItems)
            {
                if (kvp.Value != null)
                {
                    finalData[kvp.Key] = kvp.Value;
                }
                else
                {
                    missingIds.Add(kvp.Key);
                }
            }

            if (missingIds.Count > 0)
            {
                var dataProvider = _serviceProvider.GetService<IAuditorDataProvider>();
                if (dataProvider != null)
                {
                    try
                    {
                        var fetchedData = await dataProvider.GetAuditorsAsync(missingIds);
                        if (fetchedData != null && fetchedData.Count > 0)
                        {
                            var itemsToCache = new List<KeyValuePair<Guid, AuditorCacheItem>>();
                            foreach (var fetched in fetchedData)
                            {
                                finalData[fetched.Key] = fetched.Value;
                                itemsToCache.Add(new KeyValuePair<Guid, AuditorCacheItem>(fetched.Key, fetched.Value));
                            }

                            if (itemsToCache.Count > 0)
                            {
                                await _cache.SetManyAsync(itemsToCache, new DistributedCacheEntryOptions
                                {
                                    AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1)
                                });
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Failed to fetch missing auditors from data provider.");
                    }
                }
            }
        }

        // Now map back to the DTOs
        foreach (var action in auditActions)
        {
            var actorId = action.Actor!.Id!.Value;
            if (finalData.TryGetValue(actorId, out var cacheItem))
            {
                action.Actor.Name = cacheItem.Name;
                action.Actor.Avatar = cacheItem.Avatar;
                action.Actor.Email = cacheItem.Email;
            }
        }
    }

    private IReadOnlyList<object> ExtractItems(object obj)
    {
        var list = new List<object>();

        if (obj is IEnumerable enumerable && obj is not string)
        {
            foreach (var item in enumerable)
            {
                if (item != null) list.Add(item);
            }
            return list;
        }

        // Handle ListResultDto, PagedResultDto, or ApiResponse wrappers dynamically
        var type = obj.GetType();
        
        // Check for "Items" property
        var itemsProp = type.GetProperty("Items", BindingFlags.Public | BindingFlags.Instance);
        if (itemsProp != null && itemsProp.GetValue(obj) is IEnumerable innerEnumerable && innerEnumerable is not string)
        {
            foreach (var item in innerEnumerable)
            {
                if (item != null) list.Add(item);
            }
            return list;
        }
        
        // Check for "Data" property (for ApiResponse wrappers)
        var dataProp = type.GetProperty("Data", BindingFlags.Public | BindingFlags.Instance);
        if (dataProp != null)
        {
            var dataValue = dataProp.GetValue(obj);
            if (dataValue != null)
            {
                // Recursively extract if Data is a list or contains Items
                return ExtractItems(dataValue);
            }
        }

        return new[] { obj };
    }
}
