namespace iam_service.Events.Models
{
    /// <summary>
    /// Kafka event model for email notifications
    /// Published by Testorder-service, consumed by iam-service
    /// </summary>
    public class EmailNotificationEvent
    {
        public string EventType { get; set; } = string.Empty; // "TestResultWaitingReview" | "AppointmentScheduled"
        public string ToEmail { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        
        // Additional context data for email templates
        public Dictionary<string, string>? Metadata { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}

