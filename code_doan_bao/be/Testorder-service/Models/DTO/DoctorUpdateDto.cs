namespace TestOrder.Models.DTO
{
    public class DoctorUpdateDto
    {
        public string DoctorId { get; set; } = null!;
        public string? FullName { get; set; }
        public string? Specialization { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Status { get; set; }
    }
}
