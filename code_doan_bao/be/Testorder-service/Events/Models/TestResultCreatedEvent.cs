namespace Testorder_service.Events.Models
{
    public class TestResultCreatedEvent
    {
        public string TestResultId { get; set; } = string.Empty;
        public string AppointmentId { get; set; } = string.Empty;
        public string SampleId { get; set; } = string.Empty;
        public string? ReviewedBy { get; set; }
        public DateTime? ReviewedAt { get; set; }
        public string? Status { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}

