using TestOrder.Models.DTO;
using Testorder_service.Models.DTO;

namespace TestOrder.Services.Interface
{
    public interface ITestResultService
    {
        Task<IEnumerable<TestResultDto>> GetAllAsync();
        Task<TestResultDto?> GetByIdAsync(string id);
        Task CreateAsync(CreateFullTestResultDto dto);
        Task UpdateAsync(string id, UpdateTestResultDto dto);
        Task DeleteAsync(string id);
        Task<IEnumerable<TestResultDto>> GetByAppointmentIdAsync(string appointmentId);

    }
}
