using TestOrder.Models.DTO;
using TestOrder.Repositories.Interface;
using TestOrder.Services.Interface;
using Testorder_service.Models.Entities;

namespace TestOrder.Services.Implement
{
    public class DoctorShiftService : IDoctorShiftService
    {
        private readonly IDoctorShiftRepository _repo;

        public DoctorShiftService(IDoctorShiftRepository repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<DoctorShiftDto>> GetAllAsync()
        {
            var list = await _repo.GetAllAsync();
            return list.Select(s => new DoctorShiftDto
            {
                ShiftId = s.ShiftId,
                DoctorId = s.DoctorId,
                ShiftDate = s.ShiftDate,
                ShiftStart = s.ShiftStart,
                ShiftEnd = s.ShiftEnd,
                Status = s.Status,
                CreatedAt = s.CreatedAt,
                DoctorName = s.Doctor?.FullName,
                RoomId = s.RoomId,
                RoomName = s.Room?.RoomName
            });
        }

        public async Task<DoctorShiftDto?> GetByIdAsync(string id)
        {
            var s = await _repo.GetByIdAsync(id);
            if (s == null) return null;

            return new DoctorShiftDto
            {
                ShiftId = s.ShiftId,
                DoctorId = s.DoctorId,
                ShiftDate = s.ShiftDate,
                ShiftStart = s.ShiftStart,
                ShiftEnd = s.ShiftEnd,
                Status = s.Status,
                CreatedAt = s.CreatedAt,
                DoctorName = s.Doctor?.FullName,
                RoomId = s.RoomId,
                RoomName = s.Room?.RoomName
            };
        }

        public async Task<DoctorShiftDto> CreateAsync(DoctorShiftCreateDto dto)
        {
            var random = new Random();
            var shift = new DoctorShift
            {
                ShiftId = $"DTS{random.Next(0, 999999):D6}",
                DoctorId = dto.DoctorId,
                ShiftDate = dto.ShiftDate,
                ShiftStart = dto.ShiftStart,
                ShiftEnd = dto.ShiftEnd,
                Status = dto.Status ?? "Active",
                CreatedAt = DateTime.UtcNow,
                RoomId = dto.RoomId
            };

            await _repo.AddAsync(shift);
            return await GetByIdAsync(shift.ShiftId) ?? throw new Exception("Create failed");
        }

        public async Task<DoctorShiftDto?> UpdateAsync(DoctorShiftUpdateDto dto)
        {
            var shift = await _repo.GetByIdAsync(dto.ShiftId);
            if (shift == null) return null;

            if (dto.ShiftDate.HasValue) shift.ShiftDate = dto.ShiftDate.Value;
            
            if (dto.ShiftStart.HasValue) shift.ShiftStart = dto.ShiftStart.Value;
            if (dto.ShiftEnd.HasValue) shift.ShiftEnd = dto.ShiftEnd.Value;
            shift.Status = dto.Status ?? shift.Status;
            shift.DoctorId = dto.DoctorId ?? shift.DoctorId;
            shift.RoomId = dto.RoomId ?? shift.RoomId;
            await _repo.UpdateAsync(shift);
            return await GetByIdAsync(dto.ShiftId);
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var shift = await _repo.GetByIdAsync(id);
            if (shift == null) return false;

            await _repo.DeleteAsync(id);
            return true;
        }

        public async Task<IEnumerable<DoctorShiftDto>> GetByRoomIdAsync(string roomId)
        {
            var shifts = await _repo.GetAllAsync();
            return shifts
                .Where(s => s.RoomId == roomId)
                .Select(s => new DoctorShiftDto
                {
                    ShiftId = s.ShiftId,
                    DoctorId = s.DoctorId,
                    ShiftDate = s.ShiftDate,
                    ShiftStart = s.ShiftStart,
                    ShiftEnd = s.ShiftEnd,
                    Status = s.Status,
                    CreatedAt = s.CreatedAt,
                    DoctorName = s.Doctor?.FullName,
                    RoomId = s.RoomId,
                    RoomName = s.Room?.RoomName
                });
        }
        public async Task<IEnumerable<DoctorShiftDto>> CreateBulkAsync(IEnumerable<DoctorShiftCreateDto> dtos)
        {
            var random = new Random();
            if (dtos == null || !dtos.Any())
                throw new ArgumentException("Danh sách ca làm việc trống.");

            var newShifts = dtos.Select(dto => new DoctorShift
            {
                ShiftId = $"DTS{random.Next(0, 999999):D6}",
                DoctorId = dto.DoctorId,
                ShiftDate = dto.ShiftDate,
                ShiftStart = dto.ShiftStart,
                ShiftEnd = dto.ShiftEnd,
                Status = dto.Status ?? "Active",
                CreatedAt = DateTime.UtcNow,
                RoomId = dto.RoomId
            }).ToList();

            await _repo.AddRangeAsync(newShifts);

            var result = newShifts.Select(s => new DoctorShiftDto
            {
                ShiftId = s.ShiftId,
                DoctorId = s.DoctorId,
                ShiftDate = s.ShiftDate,
                ShiftStart = s.ShiftStart,
                ShiftEnd = s.ShiftEnd,
                Status = s.Status,
                CreatedAt = s.CreatedAt,
                RoomId = s.RoomId
            });

            return result;
        }

    }
}
