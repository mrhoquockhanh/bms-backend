using System;
using System.Collections.Generic;

namespace Testorder_service.Models.Entities;

public partial class TestResult
{
    public string TestResultId { get; set; } = null!;

    public string AppointmentId { get; set; } = null!;

    public string SampleId { get; set; } = null!;

    public string? ReviewedBy { get; set; }

    public DateTime? ReviewedAt { get; set; }

    public string? Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? ReagentCode { get; set; }

    public decimal? Quantity { get; set; }

    public string? DeviceId { get; set; }

    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    public virtual Appointment Appointment { get; set; } = null!;

    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    public virtual BloodSample Sample { get; set; } = null!;

    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    public virtual ICollection<TestParameter> TestParameters { get; set; } = new List<TestParameter>();
}
