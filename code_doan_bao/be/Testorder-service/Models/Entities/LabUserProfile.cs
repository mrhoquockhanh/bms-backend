using System;
using System.Collections.Generic;

namespace Testorder_service.Models.Entities;

public partial class LabUserProfile
{
    public string UserId { get; set; } = null!;

    public string FullName { get; set; } = null!;

    public string JobTitle { get; set; } = null!;

    public string? Phone { get; set; }

    public string? Email { get; set; }

    public string? DefaultRoomId { get; set; }

    public string? Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    public virtual ICollection<BloodSample> BloodSamples { get; set; } = new List<BloodSample>();

    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    public virtual LabRoom? DefaultRoom { get; set; }

    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    public virtual ICollection<LabAssignment> LabAssignments { get; set; } = new List<LabAssignment>();
}
