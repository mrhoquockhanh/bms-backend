namespace Patient_service.Models
{

    public partial class LabTest
    {
        public string Id { get; set; } = null!;

        public string TestName { get; set; } = null!;

        public string? Description { get; set; }

        public decimal Price { get; set; }

        public DateTime CreatedAt { get; set; }

        public virtual ICollection<LabCriterion> Criteria { get; set; } = new List<LabCriterion>();

        public virtual ICollection<MedicalRecord> MedicalRecords { get; set; } = new List<MedicalRecord>();
    }
}
