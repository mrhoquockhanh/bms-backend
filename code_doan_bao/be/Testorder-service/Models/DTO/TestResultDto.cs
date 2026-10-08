namespace TestOrder.Models.DTO
{
    public class TestResultDto
    {
        public string TestResultId { get; set; } = null!;
        public string AppointmentId { get; set; } = null!;
        public string SampleId { get; set; } = null!;
       
        public string? ReagentCode { get; set; }
        public int Quantity { get; set; }
        public string DeviceId { get; set; } = null!;
        public string? ReviewedBy { get; set; }
        public DateTime? ReviewedAt { get; set; }
        public string? Status { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
