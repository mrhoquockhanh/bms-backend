using TestOrder.Models.DTO;
using TestOrder.Repositories.Interface;
using TestOrder.Services.Interface;
using Testorder_service.Models.Entities;

namespace TestOrder.Services.Implement
{
    public class DoctorService : IDoctorService
    {
        private readonly IDoctorRepository _repo;

        public DoctorService(IDoctorRepository repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<DoctorDto>> GetAllAsync()
        {
            var list = await _repo.GetAllAsync();
            return list.Select(d => new DoctorDto
            {
                DoctorId = d.DoctorId,
                FullName = d.FullName,
                Specialization = d.Specialization,
                Phone = d.Phone,
                Email = d.Email,
                Status = d.Status,
                CreatedAt = d.CreatedAt,
                UpdatedAt = d.UpdatedAt
            });
        }

        public async Task<DoctorDto?> GetByIdAsync(string id)
        {
            var d = await _repo.GetByIdAsync(id);
            if (d == null) return null;

            return new DoctorDto
            {
                DoctorId = d.DoctorId,
                FullName = d.FullName,
                Specialization = d.Specialization,
                Phone = d.Phone,
                Email = d.Email,
                Status = d.Status,
                CreatedAt = d.CreatedAt,
                UpdatedAt = d.UpdatedAt
            };
        }

        public async Task<DoctorDto> CreateAsync(DoctorCreateDto dto)
        {
            var random = new Random();
            var doctor = new Doctor
            {
                DoctorId = $"DOC{random.Next(0, 999999):D6}",
                FullName = dto.FullName,
                Specialization = dto.Specialization,
                Phone = dto.Phone,
                Email = dto.Email,
                Status = "Active",
                CreatedAt = DateTime.UtcNow
            };

            await _repo.AddAsync(doctor);
            return await GetByIdAsync(doctor.DoctorId) ?? throw new Exception("Create failed");
        }

        public async Task<DoctorDto?> UpdateAsync(DoctorUpdateDto dto)
        {
            var doctor = await _repo.GetByIdAsync(dto.DoctorId);
            if (doctor == null) return null;

            doctor.FullName = dto.FullName ?? doctor.FullName;
            doctor.Specialization = dto.Specialization ?? doctor.Specialization;
            doctor.Phone = dto.Phone ?? doctor.Phone;
            doctor.Email = dto.Email ?? doctor.Email;
            doctor.Status = dto.Status ?? doctor.Status;
            doctor.UpdatedAt = DateTime.UtcNow;

            await _repo.UpdateAsync(doctor);
            return await GetByIdAsync(dto.DoctorId);
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var doctor = await _repo.GetByIdAsync(id);
            if (doctor == null) return false;

            await _repo.DeleteAsync(id);
            return true;
        }
        public async Task<IEnumerable<DoctorDto>> GetByEmailAsync(string email)
        {
            var list = await _repo.GetByEmailAsync(email);
            return list.Select(d => new DoctorDto
            {
                DoctorId = d.DoctorId,
                FullName = d.FullName,
                Specialization = d.Specialization,
                Phone = d.Phone,
                Email = d.Email,
                Status = d.Status,
                CreatedAt = d.CreatedAt,
                UpdatedAt = d.UpdatedAt
            });
        }

    }
}
