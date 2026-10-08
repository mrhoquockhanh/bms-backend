using TestOrder.Models.DTO;

namespace TestOrder.Services.Interface
{
    public interface ITestOrderCommentService
    {
        Task<IEnumerable<TestOrderCommentDto>> GetAllAsync();
        Task<TestOrderCommentDto?> GetByIdAsync(string id);
        Task<IEnumerable<TestOrderCommentDto>> GetByAppointmentIdAsync(string appointmentId);
        Task CreateAsync(CreateTestOrderCommentDto dto);
        Task UpdateAsync(string id, UpdateTestOrderCommentDto dto);
        Task DeleteAsync(string id);
    }
}
