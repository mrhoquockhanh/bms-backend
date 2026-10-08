
using Testorder_service.Models.Entities;

namespace TestOrder.Repositories.Interface
{
    public interface IDoctorRepository
    {
        Task<IEnumerable<Doctor>> GetAllAsync();
        Task<Doctor?> GetByIdAsync(string id);
        Task AddAsync(Doctor doctor);
        Task UpdateAsync(Doctor doctor);
        Task DeleteAsync(string id);
        Task<IEnumerable<Doctor>> GetByEmailAsync(string email);
    }
}
