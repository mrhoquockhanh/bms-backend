namespace TestOrder.Models.DTO
{
    public class DoctorShiftCreateDto
    {
        public string DoctorId { get; set; } = null!;
        public DateOnly ShiftDate { get; set; }
        public TimeOnly ShiftStart { get; set; }
        public TimeOnly ShiftEnd { get; set; }
        public string? Status { get; set; }
        public string? RoomId { get; set; }
    }
}
