using Microsoft.EntityFrameworkCore;
using TestOrder.Repositories.Interface;
using Testorder_service.Data;
using Testorder_service.Models.Entities;

namespace TestOrder.Repositories.Implement
{
    public class TestParameterRepository : ITestParameterRepository
    {
        private readonly TestOrderManagementDbContext _context;

        public TestParameterRepository(TestOrderManagementDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TestParameter>> GetAllAsync()
            => await _context.TestParameters.ToListAsync();

        public async Task<TestParameter?> GetByIdAsync(string id)
            => await _context.TestParameters.FindAsync(id);

        public async Task<List<TestParameter>> GetByTestResultIdAsync(string resultId)
        {
            return await _context.TestParameters
                .Where(p => p.TestResultId == resultId)
                .ToListAsync();
        }


        public async Task AddAsync(TestParameter entity)
        {
            await _context.TestParameters.AddAsync(entity);
        }

        public async Task UpdateAsync(TestParameter entity)
        {
            _context.TestParameters.Update(entity);
            await Task.CompletedTask;
        }

        public async Task DeleteAsync(string id)
        {
            var existing = await GetByIdAsync(id);
            if (existing != null)
                _context.TestParameters.Remove(existing);
        }

        public async Task SaveChangesAsync()
            => await _context.SaveChangesAsync();

        public async Task AddRangeAsync(List<TestParameter> parameters)
        {
            await _context.TestParameters.AddRangeAsync(parameters);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteByTestResultIdAsync(string testResultId)
        {
            var parameters = _context.TestParameters
                .Where(p => p.TestResultId == testResultId)
                .ToList();

            if (parameters.Any())
            {
                _context.TestParameters.RemoveRange(parameters);
            }
            await _context.SaveChangesAsync();
        }

    }

}

