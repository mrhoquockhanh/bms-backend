using TestOrder.Models.DTO;

namespace TestOrder.Services.Interface
{
    public interface ITestCriterionService
    {
        Task<IEnumerable<TestCriterionDto>> GetAllAsync();
        Task<TestCriterionDto?> GetByIdAsync(string id);
        Task CreateAsync(CreateTestCriterionDto dto);
        Task UpdateAsync(string id, UpdateTestCriterionDto dto);
        Task DeleteAsync(string id);
    }
}
