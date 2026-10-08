using warehouse_service.Models.DTO;
using warehouse_service.Models.Entities;

namespace warehouse_service.Services.Interface
{
    public interface IEquipmentService
    {
        Task<(List<EquipmentDto> Items, int Total)> GetPagedAsync(int pageNumber, int pageSize, string? q = null);
        Task<EquipmentDto?> GetByIdAsync(string id);
        Task<EquipmentDto> CreateAsync(Equipment equipment);
        Task<EquipmentDto> UpdateStatusAsync(string id, EquipmentUpdateDto dto);
        Task DeleteAsync(string id);
    }
}
