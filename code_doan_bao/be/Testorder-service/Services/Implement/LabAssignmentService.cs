using TestOrder.Models.DTO;
using TestOrder.Repositories.Interface;
using TestOrder.Services.Interface;
using Testorder_service.Models.DTO;
using Testorder_service.Models.Entities;
using Testorder_service.Repositories.Interface;
using Testorder_service.Services.Interface;

namespace TestOrder.Services.Implement
{
    public class LabAssignmentService : ILabAssignmentService
    {
        private readonly ILabAssignmentRepository _repo;

        public LabAssignmentService(ILabAssignmentRepository repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<LabAssignmentReadDto>> GetAllAsync()
        {
            var list = await _repo.GetAllAsync();
            return list.Select(a => new LabAssignmentReadDto
            {
                AssignmentId = a.AssignmentId,
                LabUserId = a.LabUserId,
                LabRoomId = a.LabRoomId,
                AssignmentDate = a.AssignmentDate,
                ShiftStart = a.ShiftStart,
                ShiftEnd = a.ShiftEnd,
                Status = a.Status,
                CreatedAt = a.CreatedAt
            });
        }

        public async Task<LabAssignmentReadDto?> GetByIdAsync(string id)
        {
            var a = await _repo.GetByIdAsync(id);
            if (a == null) return null;
            return new LabAssignmentReadDto
            {
                AssignmentId = a.AssignmentId,
                LabUserId = a.LabUserId,
                LabRoomId = a.LabRoomId,
                AssignmentDate = a.AssignmentDate,
                ShiftStart = a.ShiftStart,
                ShiftEnd = a.ShiftEnd,
                Status = a.Status,
                CreatedAt = a.CreatedAt
            };
        }

        public async Task<LabAssignmentReadDto> CreateAsync(LabAssignmentCreateDto dto)
        {
            var random = new Random();
            var entity = new LabAssignment
            {
                AssignmentId = $"ASG{random.Next(0, 999999):D6}",
                LabUserId = dto.LabUserId,
                LabRoomId = dto.LabRoomId,
                AssignmentDate = dto.AssignmentDate,
                ShiftStart = dto.ShiftStart,
                ShiftEnd = dto.ShiftEnd,
                Status = dto.Status ?? "Active",
                CreatedAt = DateTime.UtcNow
            };

            var created = await _repo.CreateAsync(entity);
            return new LabAssignmentReadDto
            {
                AssignmentId = created.AssignmentId,
                LabUserId = created.LabUserId,
                LabRoomId = created.LabRoomId,
                AssignmentDate = created.AssignmentDate,
                ShiftStart = created.ShiftStart,
                ShiftEnd = created.ShiftEnd,
                Status = created.Status,
                CreatedAt = created.CreatedAt
            };
        }

        public async Task UpdateAsync(string id, LabAssignmentUpdateDto dto)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null)
                throw new KeyNotFoundException($"Không tìm thấy phân công với ID: {id}");

            if (dto.LabRoomId != null) entity.LabRoomId = dto.LabRoomId;
            if (dto.AssignmentDate.HasValue) entity.AssignmentDate = dto.AssignmentDate.Value;
            if (dto.ShiftStart.HasValue) entity.ShiftStart = dto.ShiftStart.Value;
            if (dto.ShiftEnd.HasValue) entity.ShiftEnd = dto.ShiftEnd.Value;
            if (dto.Status != null) entity.Status = dto.Status;

            await _repo.UpdateAsync(entity);
        }

        public async Task DeleteAsync(string id)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null)
                throw new KeyNotFoundException($"Không tìm thấy phân công với ID: {id}");

            await _repo.DeleteAsync(entity);
        }
        public async Task<IEnumerable<LabAssignmentReadDto>> CreateBulkAsync(IEnumerable<LabAssignmentCreateDto> dtos)
        {
            var random = new Random();
            if (dtos == null || !dtos.Any())
                throw new ArgumentException("Danh sách phân công trống.");

            var entities = dtos.Select(dto => new LabAssignment
            {
                AssignmentId = $"ASG{random.Next(0, 999999):D6}",
                LabUserId = dto.LabUserId,
                LabRoomId = dto.LabRoomId,
                AssignmentDate = dto.AssignmentDate,
                ShiftStart = dto.ShiftStart,
                ShiftEnd = dto.ShiftEnd,
                Status = dto.Status ?? "Active",
                CreatedAt = DateTime.UtcNow
            }).ToList();

            await _repo.AddRangeAsync(entities);

            return entities.Select(a => new LabAssignmentReadDto
            {
                AssignmentId = a.AssignmentId,
                LabUserId = a.LabUserId,
                LabRoomId = a.LabRoomId,
                AssignmentDate = a.AssignmentDate,
                ShiftStart = a.ShiftStart,
                ShiftEnd = a.ShiftEnd,
                Status = a.Status,
                CreatedAt = a.CreatedAt
            });
        }

    }
}
