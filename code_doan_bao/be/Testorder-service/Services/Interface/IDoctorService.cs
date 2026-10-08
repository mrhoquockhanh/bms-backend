using TestOrder.Models.DTO;

namespace TestOrder.Services.Interface
{
    public interface IDoctorService
    {
        Task<IEnumerable<DoctorDto>> GetAllAsync();
        Task<DoctorDto?> GetByIdAsync(string id);
        Task<DoctorDto> CreateAsync(DoctorCreateDto dto);
        Task<DoctorDto?> UpdateAsync(DoctorUpdateDto dto);
        Task<bool> DeleteAsync(string id);
        Task<IEnumerable<DoctorDto>> GetByEmailAsync(string email);
    }
}
