using Testorder_service.Models.Entities;

namespace Testorder_service.Repositories.Interface
{
    public interface ILabUserProfileRepository
    {
        Task<IEnumerable<LabUserProfile>> GetAllAsync();
        Task<LabUserProfile?> GetByIdAsync(string id);
        Task<LabUserProfile> CreateAsync(LabUserProfile entity);
        Task UpdateAsync(LabUserProfile entity);
        Task DeleteAsync(LabUserProfile entity);
        Task<LabUserProfile?> GetByEmailAsync(string email);
    }
}
