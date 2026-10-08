namespace warehouse_service.Models.DTO
{
    public class EquipmentUpdateDto
    {
        public string? Status { get; set; }
        public string? CodeManufacturer { get; set; }
        public DateTime? LastMaintenanceDate { get; set; }
    }
}
