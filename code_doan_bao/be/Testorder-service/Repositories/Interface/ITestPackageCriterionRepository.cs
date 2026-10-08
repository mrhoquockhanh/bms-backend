
using Testorder_service.Models.Entities;

namespace TestOrder.Repositories.Interface
{
    public interface ITestPackageCriterionRepository
    {
        Task<IEnumerable<TestPackageCriterion>> GetAllAsync();
        Task<TestPackageCriterion?> GetByIdAsync(string id);
        Task AddAsync(TestPackageCriterion entity);
        Task UpdateAsync(TestPackageCriterion entity);
        Task DeleteAsync(string id);
        Task SaveChangesAsync();
    }
}
