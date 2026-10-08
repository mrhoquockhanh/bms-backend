namespace TestOrder.Models.DTO
{
    public class DoctorDto
    {
        public string DoctorId { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string? Specialization { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Status { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
