namespace TestOrder.Models.DTO
{
    public class AppointmentUpdateDto
    {
        public string? TestPackageId { get; set; }
        public string? DoctorShiftId { get; set; }
        public string? Status { get; set; }
        public string? LabAssignmentId { get; set; }
    }
}
