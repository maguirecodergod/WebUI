using LHA.Notification.Application.Contracts;
using Microsoft.Extensions.Caching.Hybrid;

namespace LHA.Notification.Infrastructure
{
    internal sealed class TemplateCache : ITemplateCache
    {
        private readonly HybridCache _cache;

        public TemplateCache(HybridCache cache)
        {
            _cache = cache;
        }

        public async Task<string?> GetRenderedAsync(Guid templateId, string locale, string variablesHash, CancellationToken cancellationToken = default)
        {
            var key = GetCacheKey(templateId, locale, variablesHash);
            
            // Using GetOrCreateAsync with a factory that returns null if not found.
            // Note: By default, HybridCache might not cache nulls depending on configuration, 
            // which is what we want for a "getter".
            return await _cache.GetOrCreateAsync<string?>(
                key,
                _ => ValueTask.FromResult<string?>(null),
                cancellationToken: cancellationToken
            );
        }

        public async Task SetRenderedAsync(Guid templateId, string locale, string variablesHash, string rendered, CancellationToken cancellationToken = default)
        {
            var key = GetCacheKey(templateId, locale, variablesHash);
            var tags = new[] { GetTemplateTag(templateId) };
            
            await _cache.SetAsync(key, rendered, tags: tags, cancellationToken: cancellationToken);
        }

        public async Task InvalidateAsync(Guid templateId, CancellationToken cancellationToken = default)
        {
            var tag = GetTemplateTag(templateId);
            await _cache.RemoveByTagAsync(tag, cancellationToken);
        }

        private static string GetCacheKey(Guid templateId, string locale, string variablesHash)
        {
            return $"template:{templateId}:{locale}:{variablesHash}";
        }

        private static string GetTemplateTag(Guid templateId)
        {
            return $"template-{templateId}";
        }
    }
}
