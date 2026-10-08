using TestOrder.Models.DTO;
using TestOrder.Repositories.Interface;
using TestOrder.Services.Interface;
using Testorder_service.Models.Entities;

namespace TestOrder.Services.Implement
{
    public class DoctorRoomService : IDoctorRoomService
    {
        private readonly IDoctorRoomRepository _repository;

        public DoctorRoomService(IDoctorRoomRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<DoctorRoomDto>> GetAllAsync()
        {
            var rooms = await _repository.GetAllAsync();
            return rooms.Select(MapToDto);
        }

        public async Task<DoctorRoomDto?> GetByIdAsync(string id)
        {
            var room = await _repository.GetByIdAsync(id);
            return room == null ? null : MapToDto(room);
        }

        private async Task<string> GenerateRoomId()
        {
            var rooms = await _repository.GetAllAsync();
            var existingIds = rooms.Select(r => r.RoomId)
                                 .Where(id => id.StartsWith("R"))
                                 .Select(id => int.TryParse(id[1..], out int num) ? num : 0)
                                 .DefaultIfEmpty(0)
                                 .Max();
            return $"R{(existingIds + 1):D3}";
        }

        public async Task<DoctorRoomDto> CreateAsync(DoctorRoomCreateDto dto)
        {
            var room = new DoctorRoom
            {
                RoomId = await GenerateRoomId(),
                RoomName = dto.RoomName,
                RoomLocation = dto.RoomLocation,
                CreatedAt = DateTime.UtcNow
            };

            var createdRoom = await _repository.CreateAsync(room);
            return MapToDto(createdRoom);
        }

        public async Task<DoctorRoomDto?> UpdateAsync(string id, DoctorRoomUpdateDto dto)
        {
            var room = new DoctorRoom
            {
                RoomName = dto.RoomName,
                RoomLocation = dto.RoomLocation
            };

            var updatedRoom = await _repository.UpdateAsync(id, room);
            return updatedRoom == null ? null : MapToDto(updatedRoom);
        }

        public async Task<bool> DeleteAsync(string id)
        {
            return await _repository.DeleteAsync(id);
        }

        private static DoctorRoomDto MapToDto(DoctorRoom room)
        {
            return new DoctorRoomDto
            {
                RoomId = room.RoomId,
                RoomName = room.RoomName,
                RoomLocation = room.RoomLocation,
                CreatedAt = room.CreatedAt
            };
        }
    }
}