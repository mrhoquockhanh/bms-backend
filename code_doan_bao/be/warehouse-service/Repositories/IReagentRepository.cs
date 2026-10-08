using warehouse_service.Models;

namespace warehouse_service.Repositories
{
    public interface IReagentRepository
    {
        Task<(List<Reagent> Items, int Total)> GetPagedAsync(int pageNumber, int pageSize, string? q = null);
        Task<Reagent?> GetByIdAsync(string id);
        Task AddAsync(Reagent reagent);
        void Update(Reagent reagent);
        void Remove(Reagent reagent);
        Task<bool> ExistsByCodeAsync(string code);
        Task SaveChangesAsync();
    }
}
