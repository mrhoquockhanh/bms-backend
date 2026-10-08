namespace TestOrder.Models.DTO
{
    public class CreateTestOrderCommentDto
    {
        public string AppointmentId { get; set; } = null!;
        public string CommentText { get; set; } = null!;
        public string CreatedBy { get; set; } = null!;
    }
}
