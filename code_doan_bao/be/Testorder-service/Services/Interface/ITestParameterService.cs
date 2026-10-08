using TestOrder.Models.DTO;
using Testorder_service.Models.Entities;

namespace TestOrder.Services.Interface
{
    public interface ITestParameterService
    {
        Task<IEnumerable<TestParameterDto>> GetAllAsync();
        Task<TestParameterDto?> GetByIdAsync(string id);
        Task CreateAsync(CreateTestParameterDto dto);
        Task UpdateAsync(string id, UpdateTestParameterDto dto);
        Task DeleteAsync(string id);
        Task<List<TestParameter>> GetByTestResultIdAsync(string resultId);
    }
}
