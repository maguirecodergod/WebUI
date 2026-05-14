using LHA.EventBus;
using LHA.Notification.Domain.Shared;

namespace LHA.Notification.Application.Contracts
{
    /// <summary>
    /// Event data transfer object for notification created.
    /// </summary>
    [EventName("LHA.Notification.Application.Contracts.EventBuses.NotificationCreatedEto")]
    [EventVersion(version: 1)]
    public sealed record NotificationCreatedEto : IntegrationEvent
    {
        /// <summary>
        /// Notification Id
        /// </summary>
        /// <value>
        /// The notification identifier.
        /// </value>
        public Guid NotificationId { get; set; }
        /// <summary>
        /// Recipient Id
        /// </summary>
        /// <value>
        /// The recipient identifier.
        /// </value>
        public Guid RecipientId { get; set; }
        /// <summary>
        /// Notification Type
        /// </summary>
        /// <value>
        /// The notification type.
        /// </value>
        public CNotificationType Type { get; set; }
        /// <summary>
        /// Notification Priority
        /// </summary>
        /// <value>
        /// The notification priority.
        /// </value>
        public CNotificationPriority Priority { get; set; }
        /// <summary>
        /// Notification Subject
        /// </summary>
        /// <value>
        /// The notification subject.
        /// </value>
        public string? Subject { get; set; }
        /// <summary>
        /// Notification Body
        /// </summary>
        /// <value>
        /// The notification body.
        /// </value>
        public string? Body { get; set; }
        /// <summary>
        /// Template Id
        /// </summary>
        /// <value>
        /// The template identifier.
        /// </value>
        public Guid? TemplateId { get; set; }
        /// <summary>
        /// Template Variables
        /// </summary>
        /// <value>
        /// The template variables.
        /// </value>
        public Dictionary<string, object>? TemplateVariables { get; set; }
        /// <summary>
        /// Action Url
        /// </summary>
        /// <value>
        /// The action url.
        /// </value>
        public string? ActionUrl { get; set; }
        /// <summary>
        /// Data
        /// </summary>
        /// <value>
        /// The data.
        /// </value>
        public Dictionary<string, string>? Data { get; set; }
    }
}