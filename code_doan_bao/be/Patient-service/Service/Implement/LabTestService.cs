using Patient_service.Models;
using Patient_service.Models.Dto;
using Patient_service.Repositories;
using Patient_service.Repositories.Interface;
using Service.Interface;

namespace Service.Implement
{
    public class LabTestService : ILabTestService
    {
        private readonly ILabTestRepository _repository;

        public LabTestService(ILabTestRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<LabTest>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<LabTest?> GetByIdAsync(string id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<LabTest> AddAsync(LabTestDto labTest)
        {
           return await _repository.AddAsync(labTest);
        }

        public async Task UpdateAsync(LabTestDto labTest, string id)
        {
            await _repository.UpdateAsync(labTest, id);
        }

        public async Task DeleteAsync(string id)
        {
            await _repository.DeleteAsync(id);
        }

        public Task<List<LabTest>> SearchAsync(string key) // Fix: Corrected the return type to match the repository method
        {
            return _repository.SearchAsync(key);
        }

      
    }
}
