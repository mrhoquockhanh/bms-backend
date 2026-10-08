namespace TestOrder.Models.DTO
{
    public class DoctorRoomDto
    {
        public string RoomId { get; set; } = null!;
        public string RoomName { get; set; } = null!;
        public string? RoomLocation { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}