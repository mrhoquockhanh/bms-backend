namespace TestOrder.Models.DTO
{
    public class DoctorShiftDto
    {
        public string ShiftId { get; set; } = null!;
        public string DoctorId { get; set; } = null!;
        public DateOnly ShiftDate { get; set; }
        public TimeOnly ShiftStart { get; set; }
        public TimeOnly ShiftEnd { get; set; }
        public string? Status { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string? DoctorName { get; set; }
        public string? RoomId { get; set; }
        public string? RoomName { get; set; }
    }
}
