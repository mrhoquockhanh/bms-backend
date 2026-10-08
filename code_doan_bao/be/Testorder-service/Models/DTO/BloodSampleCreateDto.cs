namespace TestOrder.Models.DTO
{
    public class BloodSampleCreateDto
    {
        public string AppointmentId { get; set; } = null!;
        public string SampleType { get; set; } = null!;
        public DateTime? CollectedAt { get; set; }
        public string? CollectedBy { get; set; }
        public decimal? Volume { get; set; }
        public string? Status { get; set; }
        public string? StorageLocation { get; set; }
        public string? Notes { get; set; }
        public string? LabUserId { get; set; }
    }
}
