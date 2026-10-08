using System;
using System.Collections.Generic;

namespace Testorder_service.Models.Entities;

public partial class TestOrderComment
{
    public string CommentId { get; set; } = null!;

    public string AppointmentId { get; set; } = null!;

    public string CommentText { get; set; } = null!;

    public string CreatedBy { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    public virtual Appointment Appointment { get; set; } = null!;
}
