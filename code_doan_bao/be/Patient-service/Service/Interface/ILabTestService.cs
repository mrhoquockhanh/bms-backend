using Patient_service.Models;
using Patient_service.Models.Dto;

namespace Service.Interface
{
    public interface ILabTestService
    {
        Task<IEnumerable<LabTest>> GetAllAsync();
        Task<LabTest?> GetByIdAsync(string id);
        Task<LabTest> AddAsync(LabTestDto labTest);
        Task UpdateAsync(LabTestDto labTest, string id);
        Task DeleteAsync(string id);
        Task<List<LabTest>> SearchAsync(string key);
    }
}
