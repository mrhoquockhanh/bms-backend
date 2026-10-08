using Microsoft.EntityFrameworkCore;
using TestOrder.Repositories.Interface;
using Testorder_service.Data;
using Testorder_service.Models.Entities;
using Testorder_service.Repositories.Interface;

namespace TestOrder.Repositories.Implement
{
    public class LabUserProfileRepository : ILabUserProfileRepository
    {
        private readonly TestOrderManagementDbContext _context;

        public LabUserProfileRepository(TestOrderManagementDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<LabUserProfile>> GetAllAsync()
        {
            return await _context.LabUserProfiles
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<LabUserProfile?> GetByIdAsync(string id)
        {
            return await _context.LabUserProfiles
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.UserId == id);
        }
        public async Task<LabUserProfile?> GetByEmailAsync(string email)
        {
            return await _context.LabUserProfiles
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email == email);
        }
        public async Task<LabUserProfile> CreateAsync(LabUserProfile entity)
        {
            _context.LabUserProfiles.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task UpdateAsync(LabUserProfile entity)
        {
            _context.LabUserProfiles.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(LabUserProfile entity)
        {
            _context.LabUserProfiles.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}
