namespace warehouse_service.Models.DTO
{
    public class ReagentCreateDto
    {
        public string ReagentCode { get; set; } = null!;
        public string ReagentName { get; set; } = null!;
        public string? Manufacturer { get; set; }
        public int Quantity { get; set; }
        public DateTime ImportDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string? Status { get; set; }
        public string? Description { get; set; }
        public string? ImportLot { get; set; }
        public string? Unit { get; set; }
    }
}
