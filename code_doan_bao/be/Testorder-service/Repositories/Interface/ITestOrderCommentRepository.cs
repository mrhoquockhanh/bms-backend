
using Testorder_service.Models.Entities;

namespace TestOrder.Repositories.Interface
{
    public interface ITestOrderCommentRepository
    {
        Task<IEnumerable<TestOrderComment>> GetAllAsync();
        Task<TestOrderComment?> GetByIdAsync(string id);
        Task<IEnumerable<TestOrderComment>> GetByAppointmentIdAsync(string appointmentId);
        Task AddAsync(TestOrderComment comment);
        Task UpdateAsync(TestOrderComment comment);
        Task DeleteAsync(string id);
        Task SaveChangesAsync();

    }
}
