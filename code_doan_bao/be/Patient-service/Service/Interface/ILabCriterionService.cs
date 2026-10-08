using Patient_service.Models;
using Patient_service.Models.Dto;

namespace Service.Interface
{
    public interface ILabCriterionService
    {
        Task<IEnumerable<LabCriterion>> GetAllAsync();
        Task<LabCriterion?> GetByIdAsync(string id);
        Task AddAsync(LabCriterionDto criterion);
        Task UpdateAsync(LabCriterionDto criterion , string id);
        Task DeleteAsync(string id);
        Task<List<LabCriterion>> SearchAsync(string key);

    }
}
