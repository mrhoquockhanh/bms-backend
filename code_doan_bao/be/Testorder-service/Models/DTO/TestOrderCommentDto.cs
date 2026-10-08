namespace TestOrder.Models.DTO
{
    public class TestOrderCommentDto
    {
        public string CommentId { get; set; } = null!;
        public string AppointmentId { get; set; } = null!;
        public string CommentText { get; set; } = null!;
        public string CreatedBy { get; set; } = null!;
        public DateTime? CreatedAt { get; set; }
    }
}
