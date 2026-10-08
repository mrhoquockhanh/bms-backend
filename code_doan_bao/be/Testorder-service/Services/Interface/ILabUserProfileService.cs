using Testorder_service.Models.DTO;
using Testorder_service.Models.Entities;

namespace Testorder_service.Services.Interface
{
    public interface ILabUserProfileService
    {

        Task<IEnumerable<LabUserProfileReadDto>> GetAllAsync();
        Task<LabUserProfileReadDto?> GetByIdAsync(string id);
        Task<LabUserProfileReadDto> CreateAsync(LabUserProfileCreateDto dto);
        Task UpdateAsync(string id, LabUserProfileUpdateDto dto);
        Task DeleteAsync(string id);
        Task<LabUserProfile?> GetByEmailAsync(string email);
    }
}
