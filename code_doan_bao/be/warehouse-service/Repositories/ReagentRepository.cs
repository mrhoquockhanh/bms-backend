using Microsoft.EntityFrameworkCore;
using warehouse_service.Data;
using warehouse_service.Models;

namespace warehouse_service.Repositories
{
    public class ReagentRepository : IReagentRepository
    {
        private readonly WarehouseDbContext _context;

        public ReagentRepository(WarehouseDbContext context)
        {
            _context = context;
        }

        private async Task<string> GenerateNextIdAsync()
        {
            var last = await _context.Reagents
                .Where(r => r.Id.StartsWith("RE"))
                .OrderByDescending(r => r.Id)
                .FirstOrDefaultAsync();

            long nextNumber = 1;
            if (last != null)
            {
                var suffix = last.Id.Length > 2 ? last.Id.Substring(2) : "0";
                if (!long.TryParse(suffix, out long parsed)) parsed = 0;
                nextNumber = parsed + 1;
            }

            return $"RE{nextNumber:D6}";
        }

        public async Task AddAsync(Reagent reagent)
        {
            if (string.IsNullOrWhiteSpace(reagent.Id))
            {
                reagent.Id = await GenerateNextIdAsync();
            }

            await _context.Reagents.AddAsync(reagent);
        }

        public async Task<(List<Reagent> Items, int Total)> GetPagedAsync(int pageNumber, int pageSize, string? q = null)
        {
            var query = _context.Reagents.AsQueryable();
            if (!string.IsNullOrWhiteSpace(q))
            {
                var search = q.Trim();
                query = query.Where(r =>
                    (r.ReagentCode != null && r.ReagentCode.Contains(search)) ||
                    (r.ReagentName != null && r.ReagentName.Contains(search)) ||
                    (r.Manufacturer != null && r.Manufacturer.Contains(search)) ||
                    (r.Status != null && r.Status.Contains(search)) ||
                    (r.Description != null && r.Description.Contains(search)) ||
                    (r.ImportLot != null && r.ImportLot.Contains(search))
                );
            }

            var total = await query.CountAsync();

            var items = query
     .AsEnumerable()
     .OrderBy(r =>
     {
         if (r.Id != null && r.Id.StartsWith("RE") && long.TryParse(r.Id.Substring(2), out var n))
             return n;
         return long.MaxValue;
     })
     .Skip((pageNumber - 1) * pageSize)
     .Take(pageSize)
     .ToList(); // ✔️ sửa ở đây


            return (items, total);
        }

        public async Task<Reagent?> GetByIdAsync(string id)
        {
            return await _context.Reagents.FindAsync(id);
        }

        public async Task<bool> ExistsByCodeAsync(string code)
        {
            return await _context.Reagents.AnyAsync(r => r.ReagentCode == code);
        }

        public void Remove(Reagent reagent)
        {
            _context.Reagents.Remove(reagent);
        }

        public void Update(Reagent reagent)
        {
            _context.Reagents.Update(reagent);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
