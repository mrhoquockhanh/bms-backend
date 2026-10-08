using Microsoft.EntityFrameworkCore;
using Patient_service.Models;
using Patient_service.Models.Dto;
using Repositories.Interface;

namespace Patient_service.Repositories
{
    public class LabCriterionRepository : ILabCriterionRepository
    {
        private readonly PatientServiceContext _context;

        public LabCriterionRepository(PatientServiceContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<LabCriterion>> GetAllAsync()
        {
            return await _context.LabCriteria
                //.Include(c => c.LabTests)
                .ToListAsync();
        }

        public async Task<LabCriterion?> GetByIdAsync(string id)
        {
            return await _context.LabCriteria
                .Include(c => c.LabTests)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task AddAsync(LabCriterionDto criterion)
        {
            var entity = new LabCriterion
            {
                Id = "LC-" + Guid.NewGuid().ToString("N").Substring(0, 3),  
                CriteriaName = criterion.CriteriaName,
                Unit = criterion.Unit,
                ReferenceRange = criterion.ReferenceRange,
                CreatedAt = DateTime.UtcNow
            };

            _context.LabCriteria.Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Models.Dto.LabCriterionDto criterion, string id)
        {
            var entity = await _context.LabCriteria.FindAsync(id);
            if (entity != null)
            {
                entity.CriteriaName = criterion.CriteriaName;
                entity.Unit = criterion.Unit;
                entity.ReferenceRange = criterion.ReferenceRange;
                entity.CreatedAt = criterion.CreatedAt;

                _context.LabCriteria.Update(entity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(string id)
        {
            var entity = await _context.LabCriteria.FindAsync(id);
            if (entity != null)
            {
                _context.LabCriteria.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<LabCriterion>> SearchAsync(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                return new List<LabCriterion>();

            key = key.Trim().ToLower();

            var query = _context.LabCriteria.AsQueryable();

            query = query.Where(c =>
                (!string.IsNullOrEmpty(c.CriteriaName) && c.CriteriaName.ToLower().Contains(key)) ||
                (!string.IsNullOrEmpty(c.Unit) && c.Unit.ToLower().Contains(key)) ||
                (!string.IsNullOrEmpty(c.ReferenceRange) && c.ReferenceRange.ToLower().Contains(key))
            );

            if (DateTime.TryParse(key, out var date))
            {
                query = query.Union(_context.LabCriteria.Where(c => c.CreatedAt.Date == date.Date));
            }

            return await query
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }
    }
}
