using System;
using System.Collections.Generic;

namespace Patient_service.Models
{

    public partial class LabCriterion
    {
        public string Id { get; set; } = null!;

        public string CriteriaName { get; set; } = null!;

        public string? Unit { get; set; }

        public string? ReferenceRange { get; set; }

        public DateTime CreatedAt { get; set; }

        public virtual ICollection<LabTest> LabTests { get; set; } = new List<LabTest>();
    }
}
