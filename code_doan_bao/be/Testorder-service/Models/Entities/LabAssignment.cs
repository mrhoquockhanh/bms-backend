using System;
using System.Collections.Generic;

namespace Testorder_service.Models.Entities;

public partial class LabAssignment
{
    public string AssignmentId { get; set; } = null!;

    public string LabUserId { get; set; } = null!;

    public string? LabRoomId { get; set; }

    public DateOnly AssignmentDate { get; set; }

    public TimeOnly ShiftStart { get; set; }

    public TimeOnly ShiftEnd { get; set; }

    public string? Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    public virtual LabRoom? LabRoom { get; set; }

    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    public virtual LabUserProfile LabUser { get; set; } = null!;
}
