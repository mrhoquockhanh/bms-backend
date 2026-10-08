using TestOrder.Models.DTO;

namespace TestOrder.Services.Interface
{
    public interface ITestPackageCriterionService
    {
        Task<IEnumerable<TestPackageCriterionDto>> GetAllAsync();
        Task<TestPackageCriterionDto?> GetByIdAsync(string id);
        Task CreateAsync(CreateTestPackageCriterionDto dto);
        Task UpdateAsync(string id, UpdateTestPackageCriterionDto dto);
        Task DeleteAsync(string id);
    }
}
