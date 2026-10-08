using Testorder_service.Models.DTO;

namespace Testorder_service.Services.Interface
{
    public interface ILabAssignmentService
    {
        Task<IEnumerable<LabAssignmentReadDto>> GetAllAsync();
        Task<LabAssignmentReadDto?> GetByIdAsync(string id);
        Task<LabAssignmentReadDto> CreateAsync(LabAssignmentCreateDto dto);
        Task UpdateAsync(string id, LabAssignmentUpdateDto dto);
        Task DeleteAsync(string id);
        Task<IEnumerable<LabAssignmentReadDto>> CreateBulkAsync(IEnumerable<LabAssignmentCreateDto> dtos);

    }
}
