using Microsoft.EntityFrameworkCore;
using Models.Dto;
using Patient_service.Models;
using Patient_service.Models.Dto;
using Patient_service.Repositories.Interface;

namespace Patient_service.Repositories
{
    public class PatientRepository : IPatientRepository
    {
        private readonly PatientServiceContext _context;

        public PatientRepository(PatientServiceContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Patient>> GetAllAsync()
        {
            return await _context.Patients
                .ToListAsync();
        }

        public async Task<Patient?> GetByIdAsync(string id)
        {
            return await _context.Patients
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Patient> AddAsync(PatientDto patientDto)
        {
            
            if (string.IsNullOrEmpty(patientDto.Phone) && await _context.Patients.AnyAsync(p => p.Phone == patientDto.Phone))
            {
                throw new InvalidOperationException($"Phone number '{patientDto.Phone}' already exists.");
            }

          

            var patient = new Patient
            {
                Id = "PAT-" + Guid.NewGuid().ToString("N").Substring(0, 10),
                Fullname = patientDto.Fullname,
                Dob = patientDto.Dob,
                Gender = patientDto.Gender,
                Phone = patientDto.Phone,
                Email = patientDto.Email,
                Address = patientDto.Address,
                BloodType = patientDto.BloodType,
                IdentityNumber = patientDto.IdentityNumber,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Medical_History = patientDto.Medical_History
            };

            _context.Patients.Add(patient);
            await _context.SaveChangesAsync();
            return patient;
        }

        public async Task CreateAccountByPatient(PatientDto patientDto)
        {
            if (string.IsNullOrEmpty(patientDto.Phone) && await _context.Patients.AnyAsync(p => p.Phone == patientDto.Phone))
            {
                throw new InvalidOperationException($"Phone number '{patientDto.Phone}' already exists.");
            }

            if (string.IsNullOrEmpty(patientDto.Email) && await _context.Patients.AnyAsync(p => p.Email == patientDto.Email))
            {
                throw new InvalidOperationException($"Email '{patientDto.Email}' already exists.");
            }

          
            var patient = new Patient
            {
                Id = "PAT-" + Guid.NewGuid().ToString("N").Substring(0, 5),
                Fullname = patientDto.Fullname,
                Dob = patientDto.Dob,
                Gender = patientDto.Gender,
                Phone = patientDto.Phone,
                Email = patientDto.Email,
                Address = patientDto.Address,
                BloodType = patientDto.BloodType,
                IdentityNumber = patientDto.IdentityNumber,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Medical_History = patientDto.Medical_History
            };

            _context.Patients.Add(patient);
            await _context.SaveChangesAsync();
        }


        public async Task UpdateAsync(PatientUpdate patientDto)
        {
            var id = patientDto.Id; // Lấy ID từ DTO để tìm kiếm

            var existingPatient = await _context.Patients
                .FirstOrDefaultAsync(p => p.Id == id);

            if (existingPatient == null)
            {
                throw new KeyNotFoundException($"Patient with ID {id} not found.");
            }


            existingPatient.Fullname = patientDto.Fullname;
            existingPatient.Dob = patientDto.Dob; // Ngày sinh
            existingPatient.Gender = patientDto.Gender;
            existingPatient.Phone = patientDto.Phone;
            existingPatient.Email = patientDto.Email;
            existingPatient.Address = patientDto.Address;
            existingPatient.BloodType = patientDto.BloodType;
            existingPatient.IdentityNumber = patientDto.IdentityNumber;
            existingPatient.Medical_History = patientDto.Medical_History; // Lịch sử bệnh lý
            existingPatient.Createby = patientDto.Createby;

            existingPatient.UpdatedAt = DateTime.UtcNow;

            _context.Patients.Update(existingPatient);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(string id)
        {
            var patient = await _context.Patients.FindAsync(id);

            if (patient == null)
            {
                throw new KeyNotFoundException($"Patient with ID {id} not found.");
            }


            _context.Patients.Remove(patient);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Patient>> SearchAsync(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                return new List<Patient>();

            key = key.Trim().ToLower();

            var query = _context.Patients.AsQueryable();

            query = query.Where(p =>
                (!string.IsNullOrEmpty(p.Fullname) && p.Fullname.ToLower().Contains(key)) ||
                (!string.IsNullOrEmpty(p.Phone) && p.Phone.ToLower().Contains(key)) ||
                (!string.IsNullOrEmpty(p.Email) && p.Email.ToLower().Contains(key)) ||
                (!string.IsNullOrEmpty(p.Address) && p.Address.ToLower().Contains(key)) ||
                (!string.IsNullOrEmpty(p.BloodType) && p.BloodType.ToLower().Contains(key)) ||
                (!string.IsNullOrEmpty(p.IdentityNumber) && p.IdentityNumber.ToLower().Contains(key)) ||
                (!string.IsNullOrEmpty(p.Gender) && p.Gender.ToLower().Contains(key))
            );

            if (DateOnly.TryParse(key, out var dob))
            {
                query = query.Union(_context.Patients.Where(p => p.Dob == dob));
            }

            return await query
                .OrderByDescending(p => p.UpdatedAt ?? p.CreatedAt)
                .ToListAsync();
        }

        public async Task<Patient?> GetByEmailAsync(string email)
        {
            return await _context.Patients
               .FirstOrDefaultAsync(p => p.Email == email);
        }
    }
    }
