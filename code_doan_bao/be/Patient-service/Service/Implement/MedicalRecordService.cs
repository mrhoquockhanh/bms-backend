using Patient_service.Models;
using Service.Interface;
using Patient_service.Repositories;
using Patient_service.Repositories.Interface;
using Patient_service;
using Patient_service.Models.Dto;

namespace Service.Implement
{
    public class MedicalRecordService : IMedicalRecordService
    {
        private readonly IMedicalRecordRepository _repository;

        public MedicalRecordService(IMedicalRecordRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<MedicalRecord>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<MedicalRecord?> GetByIdAsync(string id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<MedicalRecord>> GetByPatientIdAsync(string patientId)
        {
            return await _repository.GetByPatientIdAsync(patientId);
        }

        public async Task<MedicalRecord> AddAsync(MedicalRecordDto record)
        {
             return await _repository.AddAsync(record);
        }

        public async Task UpdateAsync(MedicalRecordDto record, string id )
        {
            await _repository.UpdateAsync(record,id);
        }

        public async Task DeleteAsync(string id)
        {
            await _repository.DeleteAsync(id);
        }

        public Task<List<MedicalRecord>> SearchAsync(string key)
        {
           return _repository.SearchAsync(key);
        }
    }
}
