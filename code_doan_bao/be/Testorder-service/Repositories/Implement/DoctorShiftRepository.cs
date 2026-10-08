using Microsoft.EntityFrameworkCore;

using TestOrder.Repositories.Interface;
using Testorder_service.Data;
using Testorder_service.Models.Entities;

namespace TestOrder.Repositories.Implement
{
    public class DoctorShiftRepository : IDoctorShiftRepository
    {
        private readonly TestOrderManagementDbContext _context;
        public DoctorShiftRepository(TestOrderManagementDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<DoctorShift>> GetAllAsync()
            => await _context.DoctorShifts.ToListAsync();

        public async Task<DoctorShift?> GetByIdAsync(string id)
            => await _context.DoctorShifts
                                          .FirstOrDefaultAsync(s => s.ShiftId == id);

        public async Task AddAsync(DoctorShift shift)
        {
            _context.DoctorShifts.Add(shift);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(DoctorShift shift)
        {
            _context.DoctorShifts.Update(shift);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(string id)
        {
            var entity = await _context.DoctorShifts.FindAsync(id);
            if (entity != null)
            {
                _context.DoctorShifts.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
        public async Task AddRangeAsync(IEnumerable<DoctorShift> entities)
        {
            await _context.DoctorShifts.AddRangeAsync(entities);
            await _context.SaveChangesAsync();
        }

    }
}
