using Patient_service.Models;
using Patient_service.Models.Dto;
using Patient_service.Repositories;
using Repositories.Interface;
using Service.Interface;

namespace Service.Implement
{
    public class LabCriterionService : ILabCriterionService
    {
        private readonly ILabCriterionRepository _repository;

        public LabCriterionService(ILabCriterionRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<LabCriterion>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<LabCriterion?> GetByIdAsync(string id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task AddAsync(LabCriterionDto criterion)
        {
            await _repository.AddAsync(criterion);
        }

        public async Task UpdateAsync(LabCriterionDto criterion, string id)
        {
            await _repository.UpdateAsync(criterion,id);
        }

        public async Task DeleteAsync(string id)
        {
            await _repository.DeleteAsync(id);
        }

        public Task<List<LabCriterion>> SearchAsync(string key)
        {
            return _repository.SearchAsync(key);
        }
    }
}
