using warehouse_service.Models.DTO;
using warehouse_service.Models;
using warehouse_service.Repositories;
using warehouse_service.Mapping;
using warehouse_service.Services.Interface;

namespace warehouse_service.Services.Implement
{
    public class ReagentService : IReagentService
    {
        private readonly IReagentRepository _repo;

        public ReagentService(IReagentRepository repo)
        {
            _repo = repo;
        }

        public async Task<ReagentDto> CreateAsync(Reagent reagent)
        {
            var exists = await _repo.ExistsByCodeAsync(reagent.ReagentCode);
            if (exists) throw new InvalidOperationException("ReagentCode must be unique");
            await _repo.AddAsync(reagent);
            await _repo.SaveChangesAsync();
            return reagent.ToDto();
        }

        public async Task DeleteAsync(string id)
        {
            var item = await _repo.GetByIdAsync(id);
            if (item == null) throw new KeyNotFoundException("Reagent not found");
            _repo.Remove(item);
            await _repo.SaveChangesAsync();
        }

        public async Task<ReagentDto?> GetByIdAsync(string id)
        {
            var item = await _repo.GetByIdAsync(id);
            return item?.ToDto();
        }

        public async Task<(List<ReagentDto> Items, int Total)> GetPagedAsync(int pageNumber, int pageSize, string? q = null)
        {
            var (items, total) = await _repo.GetPagedAsync(pageNumber, pageSize, q);
            var dtos = items.Select(i => i.ToDto()).ToList();
            return (dtos, total);
        }

        public async Task<ReagentDto> UpdateAsync(string id, ReagentUpdateDto dto)
        {
            var item = await _repo.GetByIdAsync(id);
            if (item == null) throw new KeyNotFoundException("Reagent not found");
            if (dto.Quantity.HasValue && dto.Quantity.Value < 0) throw new ArgumentException("Quantity cannot be negative");
            item.ApplyUpdate(dto);
            _repo.Update(item);
            await _repo.SaveChangesAsync();
            return item.ToDto();
        }

        public async Task<ReagentDto> SubtractQuantityAsync(string id, int amount)
        {
            if (amount < 0) throw new ArgumentException("Amount to subtract must be non-negative");
            var item = await _repo.GetByIdAsync(id);
            if (item == null) throw new KeyNotFoundException("Reagent not found");
            var newQty = item.Quantity - amount;
            if (newQty < 0) throw new ArgumentException("Resulting quantity cannot be negative");
            item.Quantity = newQty;
            _repo.Update(item);
            await _repo.SaveChangesAsync();
            return item.ToDto();
        }
    }
}
