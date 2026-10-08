namespace Testorder_service.Events.Models
{
    public class AppointmentCreatedEvent
    {
        public string AppointmentId { get; set; } = string.Empty;
        public string? TestPackageId { get; set; }
        public string? DoctorShiftId { get; set; }
        public string Status { get; set; } = string.Empty;
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime? CreatedAt { get; set; }
        public string? LabAssignmentId { get; set; }
    }
}

