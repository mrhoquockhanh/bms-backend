namespace TestOrder.Models.DTO
{
    public class DoctorCreateDto
    {
        public string FullName { get; set; } = null!;
        public string? Specialization { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
    }
}
