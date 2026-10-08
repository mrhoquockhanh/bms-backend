namespace Models.Dto
{
    public class PatientDto
    {
        public string? Fullname { get; set; } = null!;

        public DateOnly? Dob { get; set; }

        public string? Gender { get; set; }

        public string? Phone { get; set; }

        public string? Email { get; set; }

        public string? Address { get; set; }

        public string? BloodType { get; set; }

        public string? IdentityNumber { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
        public string Medical_History { get; set; }
        public string? Createby { get; set; }

    }
}