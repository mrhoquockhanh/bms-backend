using Microsoft.EntityFrameworkCore;
using TestOrder.Repositories.Interface;
using Testorder_service.Data;
using Testorder_service.Models.Entities;

namespace TestOrder.Repositories.Implement
{
    public class TestCriterionRepository : ITestCriterionRepository
    {
        private readonly TestOrderManagementDbContext _context;

        public TestCriterionRepository(TestOrderManagementDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TestCriterion>> GetAllAsync()
            => await _context.TestCriteria.ToListAsync();

        public async Task<TestCriterion?> GetByIdAsync(string id)
            => await _context.TestCriteria.FindAsync(id);

        public async Task AddAsync(TestCriterion entity)
        {
            await _context.TestCriteria.AddAsync(entity);
        }

        public async Task UpdateAsync(TestCriterion entity)
        {
            _context.TestCriteria.Update(entity);
            await Task.CompletedTask;
        }

        public async Task DeleteAsync(string id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
                _context.TestCriteria.Remove(entity);
        }

        public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
    }
}
