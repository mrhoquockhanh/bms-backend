using Patient_service;
using Patient_service.Models;
using Patient_service.Models.Dto;

namespace Service.Interface
{
    public interface IMedicalRecordService
{
        // CREATE
        Task<IEnumerable<MedicalRecord>> GetAllAsync();
        Task<MedicalRecord?> GetByIdAsync(string id);
        Task<IEnumerable<MedicalRecord>> GetByPatientIdAsync(string patientId);
        Task<MedicalRecord> AddAsync(MedicalRecordDto record);
        Task UpdateAsync(MedicalRecordDto record , string id);
        Task DeleteAsync(string id);
        Task<List<MedicalRecord>> SearchAsync(string key);

    }
}
