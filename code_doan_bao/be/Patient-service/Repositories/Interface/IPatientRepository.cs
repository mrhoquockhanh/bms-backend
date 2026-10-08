using Models.Dto;
using Patient_service.Models;
using Patient_service.Models.Dto;

namespace Patient_service.Repositories.Interface
{
    public interface IPatientRepository
{
        Task<IEnumerable<Patient>> GetAllAsync();
        Task<Patient?> GetByIdAsync(string id);
        Task<Patient?> GetByEmailAsync(string email);
        Task<Patient> AddAsync(PatientDto patient); 
        Task UpdateAsync(PatientUpdate patient);
        Task DeleteAsync(string id);
        Task CreateAccountByPatient(PatientDto patientDto);
        Task<List<Patient>> SearchAsync(string key);
    }
}
