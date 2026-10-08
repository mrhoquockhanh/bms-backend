namespace warehouse_service.Models.DTO
{
    public class EquipmentDto
    {
        public string Id { get; set; } = null!;
        public string EquipmentName { get; set; } = null!;
        public string? Manufacturer { get; set; }
        public string? CodeManufacturer { get; set; }
        public string Status { get; set; } = null!;
        public DateTime? LastMaintenanceDate { get; set; }
        public string? Description { get; set; }
    }
}
