using Testorder_service.Models.Entities;

namespace Testorder_service.Repositories.Interface
{
    public interface ILabAssignmentRepository
    {
        Task<IEnumerable<LabAssignment>> GetAllAsync();
        Task<LabAssignment?> GetByIdAsync(string id);
        Task<LabAssignment> CreateAsync(LabAssignment entity);
        Task UpdateAsync(LabAssignment entity);
        Task DeleteAsync(LabAssignment entity);
        Task AddRangeAsync(IEnumerable<LabAssignment> entities);

    }
}
