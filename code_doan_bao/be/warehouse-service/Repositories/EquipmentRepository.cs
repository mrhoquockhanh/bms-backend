using Microsoft.EntityFrameworkCore;
using warehouse_service.Data;
using warehouse_service.Models.Entities;
using System.Text.RegularExpressions;

namespace warehouse_service.Repositories
{
    public class EquipmentRepository : IEquipmentRepository
    {
        private readonly WarehouseDbContext _context;

        public EquipmentRepository(WarehouseDbContext context)
        {
            _context = context;
        }

        // Generate next EQ ID in format EQ###### (6 digits)
        private async Task<string> GenerateNextIdAsync()
        {
            // get the max existing numeric suffix for IDs starting with EQ
            var last = await _context.Equipment
                .Where(e => e.Id.StartsWith("EQ"))
                .OrderByDescending(e => e.Id)
                .FirstOrDefaultAsync();

            long nextNumber = 1;
            if (last != null)
            {
                var suffix = last.Id.Length > 2 ? last.Id.Substring(2) : "0";
                if (!long.TryParse(suffix, out long parsed)) parsed = 0;
                nextNumber = parsed + 1;
            }

            return $"EQ{nextNumber:D6}";
        }

        public async Task AddAsync(Equipment equipment)
        {
            if (string.IsNullOrWhiteSpace(equipment.Id))
            {
                equipment.Id = await GenerateNextIdAsync();
            }

            await _context.Equipment.AddAsync(equipment);
        }

        public async Task<(List<Equipment> Items, int Total)> GetPagedAsync(int pageNumber, int pageSize, string? q = null)
        {
            var query = _context.Equipment.AsQueryable();
            if (!string.IsNullOrWhiteSpace(q))
            {
                var search = q.Trim();
                query = query.Where(e =>
                    (e.EquipmentName != null && e.EquipmentName.Contains(search)) ||
                    (e.Manufacturer != null && e.Manufacturer.Contains(search)) ||
                    (e.Status != null && e.Status.Contains(search)) ||
                    (e.Description != null && e.Description.Contains(search))
                );
            }

            var total = await query.CountAsync();

            // ordering: try to order by numeric suffix when possible
            var items = query
     .AsEnumerable()
     .OrderBy(e =>
     {
         if (e.Id != null && e.Id.StartsWith("EQ") && long.TryParse(e.Id.Substring(2), out var n))
             return n;
         return long.MaxValue;
     })
     .Skip((pageNumber - 1) * pageSize)
     .Take(pageSize)
     .ToList(); // ✔️ sửa ở đây


            return (items, total);
        }

        public async Task<Equipment?> GetByIdAsync(string id)
        {
            return await _context.Equipment.FindAsync(id);
        }

        public void Remove(Equipment equipment)
        {
            _context.Equipment.Remove(equipment);
        }

        public void Update(Equipment equipment)
        {
            _context.Equipment.Update(equipment);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
