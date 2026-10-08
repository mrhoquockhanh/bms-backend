using Microsoft.EntityFrameworkCore;
using TestOrder.Repositories.Interface;
using Testorder_service.Data;
using Testorder_service.Models.Entities;

namespace TestOrder.Repositories.Implement
{
    public class TestOrderCommentRepository : ITestOrderCommentRepository
    {
        private readonly TestOrderManagementDbContext _context;

        public TestOrderCommentRepository(TestOrderManagementDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TestOrderComment>> GetAllAsync()
            => await _context.TestOrderComments.ToListAsync();

        public async Task<TestOrderComment?> GetByIdAsync(string id)
            => await _context.TestOrderComments.FindAsync(id);

        public async Task<IEnumerable<TestOrderComment>> GetByAppointmentIdAsync(string appointmentId)
            => await _context.TestOrderComments
                .Where(c => c.AppointmentId == appointmentId)
                .ToListAsync();

        public async Task AddAsync(TestOrderComment comment)
        {
            await _context.TestOrderComments.AddAsync(comment);
        }

        public async Task UpdateAsync(TestOrderComment comment)
        {
            _context.TestOrderComments.Update(comment);
            await Task.CompletedTask;
        }

        public async Task DeleteAsync(string id)
        {
            var comment = await GetByIdAsync(id);
            if (comment != null)
                _context.TestOrderComments.Remove(comment);
        }

        public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
    }
}
