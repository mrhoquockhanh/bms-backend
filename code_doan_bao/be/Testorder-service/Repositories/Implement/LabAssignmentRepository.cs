using Microsoft.EntityFrameworkCore;
using TestOrder.Repositories.Interface;
using Testorder_service.Data;
using Testorder_service.Models.Entities;
using Testorder_service.Repositories.Interface;

namespace TestOrder.Repositories.Implement
{
    public class LabAssignmentRepository : ILabAssignmentRepository
    {
        private readonly TestOrderManagementDbContext _context;

        public LabAssignmentRepository(TestOrderManagementDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<LabAssignment>> GetAllAsync()
        {
            return await _context.LabAssignments
                
                
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<LabAssignment?> GetByIdAsync(string id)
        {
            return await _context.LabAssignments
                
                
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.AssignmentId == id);
        }

        public async Task<LabAssignment> CreateAsync(LabAssignment entity)
        {
            _context.LabAssignments.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task UpdateAsync(LabAssignment entity)
        {
            _context.LabAssignments.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(LabAssignment entity)
        {
            _context.LabAssignments.Remove(entity);
            await _context.SaveChangesAsync();
        }
        public async Task AddRangeAsync(IEnumerable<LabAssignment> entities)
        {
            await _context.LabAssignments.AddRangeAsync(entities);
            await _context.SaveChangesAsync();
        }

    }
}
