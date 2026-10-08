namespace Testorder_service.Models.DTO
{
    public class LabUserProfileCreateDto
    {
        public string FullName { get; set; } = null!;
        public string JobTitle { get; set; } = null!;
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? DefaultRoomId { get; set; }
        public string? Status { get; set; }
    }
}
