using warehouse_service.Models.DTO;
using warehouse_service.Models;

namespace warehouse_service.Services.Interface
{
    public interface IReagentService
    {
        Task<(List<ReagentDto> Items, int Total)> GetPagedAsync(int pageNumber, int pageSize, string? q = null);
        Task<ReagentDto?> GetByIdAsync(string id);
        Task<ReagentDto> CreateAsync(Reagent reagent);
        Task<ReagentDto> UpdateAsync(string id, ReagentUpdateDto dto);
        Task<ReagentDto> SubtractQuantityAsync(string id, int amount);
        Task DeleteAsync(string id);
    }
}
