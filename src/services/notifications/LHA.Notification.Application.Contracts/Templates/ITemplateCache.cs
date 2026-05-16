namespace LHA.Notification.Application.Contracts
{
    public interface ITemplateCache
    {
        Task<string?> GetRenderedAsync(Guid templateId, string locale, string variablesHash, CancellationToken cancellationToken = default);
        Task SetRenderedAsync(Guid templateId, string locale, string variablesHash, string rendered, CancellationToken cancellationToken = default);
        Task InvalidateAsync(Guid templateId, CancellationToken cancellationToken = default);
    }
}
