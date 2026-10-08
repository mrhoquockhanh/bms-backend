namespace Testorder_service.Models.DTO
{
    public class LabUserProfileReadDto
    {
        public string UserId { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string JobTitle { get; set; } = null!;
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? DefaultRoomId { get; set; }
        public string? Status { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
