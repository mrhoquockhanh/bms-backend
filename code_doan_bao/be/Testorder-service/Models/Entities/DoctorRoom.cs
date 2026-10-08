using System;
using System.Collections.Generic;

namespace Testorder_service.Models.Entities;

public partial class DoctorRoom
{
    public string RoomId { get; set; } = null!;

    public string RoomName { get; set; } = null!;

    public string? RoomLocation { get; set; }

    public DateTime? CreatedAt { get; set; }

    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    public virtual ICollection<DoctorShift> DoctorShifts { get; set; } = new List<DoctorShift>();
}
