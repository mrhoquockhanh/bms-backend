using Models.Dto;
using Patient_service.Models;
using Patient_service.Models.Dto;

namespace Service.Interface_service.Repositories.Interface
{
    public interface IPatientService
    {
        Task<IEnumerable<Patient>> GetAllAsync();
        Task<Patient?> GetByIdAsync(string id);
        Task<Patient?> AddAsync(PatientDto patient);
        Task UpdateAsync(PatientUpdate patient);
        Task DeleteAsync(string id);
        Task CreateAccountbyEmailAsync(PatientDto patient);
        Task<List<Patient>> SearchAsync(string key);
        Task<Patient?> GetByEmailAsync(string email);
    }
}
