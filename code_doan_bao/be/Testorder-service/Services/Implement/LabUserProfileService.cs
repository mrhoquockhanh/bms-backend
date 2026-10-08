using TestOrder.Models.DTO;
using TestOrder.Repositories.Interface;
using Testorder_service.Models.DTO;
using Testorder_service.Models.Entities;
using Testorder_service.Repositories.Interface;
using Testorder_service.Services.Interface;

namespace TestOrder.Services.Implement
{
    public class LabUserProfileService : ILabUserProfileService
    {
        private readonly ILabUserProfileRepository _repo;

        public LabUserProfileService(ILabUserProfileRepository repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<LabUserProfileReadDto>> GetAllAsync()
        {
            var list = await _repo.GetAllAsync();
            return list.Select(u => new LabUserProfileReadDto
            {
                UserId = u.UserId,
                FullName = u.FullName,
                JobTitle = u.JobTitle,
                Phone = u.Phone,
                Email = u.Email,
                DefaultRoomId = u.DefaultRoomId,
                Status = u.Status,
                CreatedAt = u.CreatedAt,
                UpdatedAt = u.UpdatedAt
            });
        }

        public async Task<LabUserProfileReadDto?> GetByIdAsync(string id)
        {
            var u = await _repo.GetByIdAsync(id);
            if (u == null) return null;

            return new LabUserProfileReadDto
            {
                UserId = u.UserId,
                FullName = u.FullName,
                JobTitle = u.JobTitle,
                Phone = u.Phone,
                Email = u.Email,
                DefaultRoomId = u.DefaultRoomId,
                Status = u.Status,
                CreatedAt = u.CreatedAt,
                UpdatedAt = u.UpdatedAt
            };
        }

        public async Task<LabUserProfileReadDto> CreateAsync(LabUserProfileCreateDto dto)
        {
            var random = new Random();
            var entity = new LabUserProfile
            {
                UserId = $"LAP{random.Next(0, 999999):D6}",
                FullName = dto.FullName,
                JobTitle = dto.JobTitle,
                Phone = dto.Phone,
                Email = dto.Email,
                DefaultRoomId = dto.DefaultRoomId,
                Status = dto.Status ?? "Active",
                CreatedAt = DateTime.Now
            };

            await _repo.CreateAsync(entity);

            return new LabUserProfileReadDto
            {
                UserId = entity.UserId,
                FullName = entity.FullName,
                JobTitle = entity.JobTitle,
                Phone = entity.Phone,
                Email = entity.Email,
                DefaultRoomId = entity.DefaultRoomId,
                Status = entity.Status,
                CreatedAt = entity.CreatedAt
            };
        }

        public async Task UpdateAsync(string id, LabUserProfileUpdateDto dto)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null)
                throw new Exception("Không tìm thấy người dùng.");

            entity.FullName = dto.FullName ?? entity.FullName;
            entity.JobTitle = dto.JobTitle ?? entity.JobTitle;
            entity.Phone = dto.Phone ?? entity.Phone;
            entity.Email = dto.Email ?? entity.Email;
            entity.DefaultRoomId = dto.DefaultRoomId ?? entity.DefaultRoomId;
            entity.Status = dto.Status ?? entity.Status;
            entity.UpdatedAt = DateTime.Now;

            await _repo.UpdateAsync(entity);
        }

        public async Task DeleteAsync(string id)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null)
                throw new Exception("Không tìm thấy người dùng.");

            await _repo.DeleteAsync(entity);
        }
        public Task<LabUserProfile?> GetByEmailAsync(string email)
        {
            return _repo.GetByEmailAsync(email);
        }
    }
}
