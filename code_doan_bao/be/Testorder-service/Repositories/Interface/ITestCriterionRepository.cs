
using Testorder_service.Models.Entities;

namespace TestOrder.Repositories.Interface
{
    public interface ITestCriterionRepository
    {
        Task<IEnumerable<TestCriterion>> GetAllAsync();
        Task<TestCriterion?> GetByIdAsync(string id);
        Task AddAsync(TestCriterion entity);
        Task UpdateAsync(TestCriterion entity);
        Task DeleteAsync(string id);
        Task SaveChangesAsync();
    }
}
