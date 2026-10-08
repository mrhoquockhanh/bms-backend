
using Testorder_service.Models.Entities;

namespace TestOrder.Repositories.Interface
{
    public interface ITestParameterRepository
    {
        Task<IEnumerable<TestParameter>> GetAllAsync();
        Task<TestParameter?> GetByIdAsync(string id);
        Task AddAsync(TestParameter entity);
        Task UpdateAsync(TestParameter entity);
        Task DeleteAsync(string id);
        Task AddRangeAsync(List<TestParameter> parameters);
        Task DeleteByTestResultIdAsync(string testResultId);
        Task SaveChangesAsync();
        Task<List<TestParameter>> GetByTestResultIdAsync(string resultId);
    }
}
