using System;
using System.Collections.Generic;

namespace Testorder_service.Models.Entities;

public partial class Doctor
{
    public string DoctorId { get; set; } = null!;

    public string FullName { get; set; } = null!;

    public string? Specialization { get; set; }

    public string? Phone { get; set; }

    public string? Email { get; set; }

    public string? Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    public virtual ICollection<DoctorShift> DoctorShifts { get; set; } = new List<DoctorShift>();
}
