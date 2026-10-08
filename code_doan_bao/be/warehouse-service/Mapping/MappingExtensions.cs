using warehouse_service.Models;
using warehouse_service.Models.DTO;
using warehouse_service.Models.Entities;

namespace warehouse_service.Mapping
{
    public static class MappingExtensions
    {
        // ============================
        // EQUIPMENT
        // ============================

        public static EquipmentDto ToDto(this Equipment e)
        {
            return new EquipmentDto
            {
                Id = e.Id,
                EquipmentName = e.EquipmentName,
                Manufacturer = e.Manufacturer,
                CodeManufacturer = e.CodeManufacturer,
                Status = e.Status,
                LastMaintenanceDate = e.LastMaintenanceDate,
                Description = e.Description
            };
        }

        // Dùng khi POST tạo mới Equipment
        public static Equipment ToEntity(this EquipmentCreateDto dto)
        {
            return new Equipment
            {
                EquipmentName = dto.EquipmentName,
                Manufacturer = dto.Manufacturer,
                CodeManufacturer = dto.CodeManufacturer,
                Status = dto.Status,
                LastMaintenanceDate = dto.LastMaintenanceDate,
                Description = dto.Description
            };
        }

        public static void ApplyUpdate(this Equipment e, EquipmentUpdateDto dto)
        {
            if (dto.Status != null) e.Status = dto.Status;
            if (dto.CodeManufacturer != null) e.CodeManufacturer = dto.CodeManufacturer;
            if (dto.LastMaintenanceDate.HasValue) e.LastMaintenanceDate = dto.LastMaintenanceDate.Value;
        }


        // ============================
        // REAGENT
        // ============================

        public static ReagentDto ToDto(this Reagent r)
        {
            return new ReagentDto
            {
                Id = r.Id,
                ReagentCode = r.ReagentCode,
                ReagentName = r.ReagentName,
                Manufacturer = r.Manufacturer,
                Quantity = r.Quantity,
                ImportDate = r.ImportDate,
                ExpiryDate = r.ExpiryDate,
                Status = r.Status,
                Description = r.Description,
                ImportLot = r.ImportLot,
                Unit = r.Unit
            };
        }

        // Dùng khi POST tạo mới Reagent
        public static Reagent ToEntity(this ReagentCreateDto dto)
        {
            return new Reagent
            {
                ReagentCode = dto.ReagentCode,
                ReagentName = dto.ReagentName,
                Manufacturer = dto.Manufacturer,
                Quantity = dto.Quantity,
                ImportDate = dto.ImportDate,
                ExpiryDate = dto.ExpiryDate,
                Status = dto.Status,
                Description = dto.Description,
                ImportLot = dto.ImportLot,
                Unit = dto.Unit
            };
        }

        public static void ApplyUpdate(this Reagent r, ReagentUpdateDto dto)
        {
            if (dto.Status != null) r.Status = dto.Status;
            if (dto.Quantity.HasValue) r.Quantity = dto.Quantity.Value;
        }
    }
}
