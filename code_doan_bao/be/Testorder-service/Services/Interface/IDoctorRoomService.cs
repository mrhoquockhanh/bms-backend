using TestOrder.Models.DTO;

namespace TestOrder.Services.Interface
{
    public interface IDoctorRoomService
    {
        Task<IEnumerable<DoctorRoomDto>> GetAllAsync();
        Task<DoctorRoomDto?> GetByIdAsync(string id);
        Task<DoctorRoomDto> CreateAsync(DoctorRoomCreateDto dto);
        Task<DoctorRoomDto?> UpdateAsync(string id, DoctorRoomUpdateDto dto);
        Task<bool> DeleteAsync(string id);
    }
}