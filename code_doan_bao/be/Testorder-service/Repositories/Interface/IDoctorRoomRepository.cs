using Testorder_service.Models.Entities;

namespace TestOrder.Repositories.Interface
{
    public interface IDoctorRoomRepository
    {
        Task<IEnumerable<DoctorRoom>> GetAllAsync();
        Task<DoctorRoom?> GetByIdAsync(string id);
        Task<DoctorRoom> CreateAsync(DoctorRoom room);
        Task<DoctorRoom?> UpdateAsync(string id, DoctorRoom room);
        Task<bool> DeleteAsync(string id);
    }
}