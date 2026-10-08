using warehouse_service.Models.DTO;
using warehouse_service.Models.Entities;
using warehouse_service.Repositories;
using warehouse_service.Mapping;
using warehouse_service.Services.Interface;

namespace warehouse_service.Services.Implement
{
    public class EquipmentService : IEquipmentService
    {
        private readonly IEquipmentRepository _repo;

        public EquipmentService(IEquipmentRepository repo)
        {
            _repo = repo;
        }

        public async Task<EquipmentDto> CreateAsync(Equipment equipment)
        {
            await _repo.AddAsync(equipment);
            await _repo.SaveChangesAsync();
            return equipment.ToDto();
        }

        public async Task DeleteAsync(string id)
        {
            var item = await _repo.GetByIdAsync(id);
            if (item == null) throw new KeyNotFoundException("Equipment not found");
            _repo.Remove(item);
            await _repo.SaveChangesAsync();
        }

        public async Task<EquipmentDto?> GetByIdAsync(string id)
        {
            var item = await _repo.GetByIdAsync(id);
            return item?.ToDto();
        }

        public async Task<(List<EquipmentDto> Items, int Total)> GetPagedAsync(int pageNumber, int pageSize, string? q = null)
        {
            var (items, total) = await _repo.GetPagedAsync(pageNumber, pageSize, q);
            var dtos = items.Select(i => i.ToDto()).ToList();
            return (dtos, total);
        }

        public async Task<EquipmentDto> UpdateStatusAsync(string id, EquipmentUpdateDto dto)
        {
            var item = await _repo.GetByIdAsync(id);
            if (item == null) throw new KeyNotFoundException("Equipment not found");
            item.ApplyUpdate(dto);
            _repo.Update(item);
            await _repo.SaveChangesAsync();
            return item.ToDto();
        }
    }
}
