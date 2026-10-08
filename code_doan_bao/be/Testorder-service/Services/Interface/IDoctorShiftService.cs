using TestOrder.Models.DTO;

namespace TestOrder.Services.Interface
{
    public interface IDoctorShiftService
    {
        Task<IEnumerable<DoctorShiftDto>> GetAllAsync();
        Task<DoctorShiftDto?> GetByIdAsync(string id);
        Task<DoctorShiftDto> CreateAsync(DoctorShiftCreateDto dto);
        Task<DoctorShiftDto?> UpdateAsync(DoctorShiftUpdateDto dto);
        Task<bool> DeleteAsync(string id);
        Task<IEnumerable<DoctorShiftDto>> GetByRoomIdAsync(string roomId);
        Task<IEnumerable<DoctorShiftDto>> CreateBulkAsync(IEnumerable<DoctorShiftCreateDto> dtos);

    }
}
