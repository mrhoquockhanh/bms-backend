namespace Testorder_service.Models.DTO
{
    public class LabAssignmentUpdateDto
    {
        public string? LabRoomId { get; set; }
        public DateOnly? AssignmentDate { get; set; }
        public TimeOnly? ShiftStart { get; set; }
        public TimeOnly? ShiftEnd { get; set; }
        public string? Status { get; set; }
    }
}
