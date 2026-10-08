
using Testorder_service.Models.Entities;

namespace TestOrder.Repositories.Interface
{
    public interface IDoctorShiftRepository
    {
        Task<IEnumerable<DoctorShift>> GetAllAsync();
        Task<DoctorShift?> GetByIdAsync(string id);
        Task AddAsync(DoctorShift shift);
        Task UpdateAsync(DoctorShift shift);
        Task DeleteAsync(string id);
        Task AddRangeAsync(IEnumerable<DoctorShift> entities);

    }
}
