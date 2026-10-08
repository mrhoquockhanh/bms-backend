using Microsoft.EntityFrameworkCore;
using TestOrder.Repositories.Interface;
using Testorder_service.Data;
using Testorder_service.Models.Entities;

namespace TestOrder.Repositories.Implement
{
    public class TestPackageCriterionRepository : ITestPackageCriterionRepository
    {
        private readonly TestOrderManagementDbContext _context;

        public TestPackageCriterionRepository(TestOrderManagementDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TestPackageCriterion>> GetAllAsync()
            => await _context.TestPackageCriteria.ToListAsync();

        public async Task<TestPackageCriterion?> GetByIdAsync(string id)
            => await _context.TestPackageCriteria.FindAsync(id);

        public async Task AddAsync(TestPackageCriterion entity)
        {
            await _context.TestPackageCriteria.AddAsync(entity);
        }

        public async Task UpdateAsync(TestPackageCriterion entity)
        {
            _context.TestPackageCriteria.Update(entity);
            await Task.CompletedTask;
        }

        public async Task DeleteAsync(string id)
        {
            var existing = await GetByIdAsync(id);
            if (existing != null)
                _context.TestPackageCriteria.Remove(existing);
        }

        public async Task AddRangeAsync(IEnumerable<TestPackageCriterion> entities)
        {
            await _context.TestPackageCriteria.AddRangeAsync(entities);
        }


        public async Task SaveChangesAsync()
            => await _context.SaveChangesAsync();
    }
}
