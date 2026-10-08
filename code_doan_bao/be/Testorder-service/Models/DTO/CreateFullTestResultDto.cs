namespace Testorder_service.Models.DTO
{
    public class CreateFullTestResultDto
    {
        public string? TestResultId { get; set; }
        public string AppointmentId { get; set; } = null!;
        public string SampleId { get; set; } = null!;
        public string? ReagentCode { get; set; }
        public int Quantity { get; set; }
        public string DeviceId { get; set; } = null!;
        public string? ReviewedBy { get; set; }
        public string? Status { get; set; }

        // ----- Danh sách TestParameter -----
        public List<TestParameterItemDto> Parameters { get; set; } = new();
    }
}
