using Microsoft.EntityFrameworkCore;

using TestOrder.Repositories.Interface;
using Testorder_service.Data;
using Testorder_service.Models.Entities;

namespace TestOrder.Repositories.Implement
{
    public class DoctorRepository : IDoctorRepository
    {
        private readonly TestOrderManagementDbContext _context;
        public DoctorRepository(TestOrderManagementDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Doctor>> GetAllAsync()
            => await _context.Doctors.ToListAsync();

        public async Task<Doctor?> GetByIdAsync(string id)
            => await _context.Doctors.FindAsync(id);

        public async Task AddAsync(Doctor doctor)
        {
            _context.Doctors.Add(doctor);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Doctor doctor)
        {
            _context.Doctors.Update(doctor);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(string id)
        {
            var entity = await _context.Doctors.FindAsync(id);
            if (entity != null)
            {
                _context.Doctors.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<IEnumerable<Doctor>> GetByEmailAsync(string email)
        {
            return await _context.Doctors
                .AsNoTracking()
                .Where(d => d.Email == email)
                .ToListAsync();
        }
    }
}
