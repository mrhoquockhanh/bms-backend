namespace TestOrder.Models.DTO
{
    public class UpdateTestResultDto
    {
        public string? ReagentCode { get; set; }
        public int Quantity { get; set; }
        public string DeviceId { get; set; }
        public string? ReviewedBy { get; set; }
        public DateTime? ReviewedAt { get; set; }
        public string? Status { get; set; }
        public List<UpdateTestParameterDto>? Parameters { get; set; }
    }
}
