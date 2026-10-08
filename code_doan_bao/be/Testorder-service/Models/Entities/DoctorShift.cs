using System;
using System.Collections.Generic;

namespace Testorder_service.Models.Entities;

public partial class DoctorShift
{
    public string ShiftId { get; set; } = null!;

    public string DoctorId { get; set; } = null!;

    public DateOnly ShiftDate { get; set; }

    public TimeOnly ShiftStart { get; set; }

    public TimeOnly ShiftEnd { get; set; }

    public string? Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public string? RoomId { get; set; }

    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    public virtual Doctor Doctor { get; set; } = null!;

    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    public virtual DoctorRoom? Room { get; set; }
}
