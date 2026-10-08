using warehouse_service.Models.Entities;

namespace warehouse_service.Repositories
{
    public interface IEquipmentRepository
    {
        Task<(List<Equipment> Items, int Total)> GetPagedAsync(int pageNumber, int pageSize, string? q = null);
        Task<Equipment?> GetByIdAsync(string id);
        Task AddAsync(Equipment equipment);
        void Update(Equipment equipment);
        void Remove(Equipment equipment);
        Task SaveChangesAsync();
    }
}
