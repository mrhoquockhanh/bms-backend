using Models;
using Patient_service.Models;

namespace Patient_service;

public partial class MedicalRecord
{
    public string Id { get; set; } = null!;

    public string PatientId { get; set; } = null!;

    public string RecordType { get; set; } = null!;

    public string? Title { get; set; }

    public string? Description { get; set; }

    public string? RecordData { get; set; }

    public string? Status { get; set; }

    public DateOnly? OnsetDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public string? CreatedBy { get; set; }

    public string? VerifiedBy { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual Patient Patient { get; set; } = null!;

    public virtual ICollection<LabTest> LabTests { get; set; } = new List<LabTest>();
}
