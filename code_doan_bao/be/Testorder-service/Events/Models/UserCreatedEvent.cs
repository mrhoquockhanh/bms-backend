namespace Testorder_service.Events.Models
{
    public class UserCreatedEvent
    {
        public string UserId { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int RoleId { get; set; }
        public int Status { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}

