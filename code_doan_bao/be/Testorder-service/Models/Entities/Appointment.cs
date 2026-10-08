using System;
using System.Collections.Generic;

namespace Testorder_service.Models.Entities;

public partial class Appointment
{
    public string AppointmentId { get; set; } = null!;

    public string? TestPackageId { get; set; }

    public string? DoctorShiftId { get; set; }

    public string Status { get; set; } = null!;

    public string CreatedBy { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public string? LabAssignmentId { get; set; }

    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    public virtual ICollection<BloodSample> BloodSamples { get; set; } = new List<BloodSample>();

    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    public virtual DoctorShift? DoctorShift { get; set; }

    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    public virtual LabAssignment? LabAssignment { get; set; }

    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    public virtual ICollection<TestOrderComment> TestOrderComments { get; set; } = new List<TestOrderComment>();

    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    public virtual TestPackage? TestPackage { get; set; }

    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    public virtual ICollection<TestResult> TestResults { get; set; } = new List<TestResult>();
}
