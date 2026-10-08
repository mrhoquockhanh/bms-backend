using System;
using System.Collections.Generic;

namespace Patient_service.Models.Dto
{

    public partial class LabCriterionDto
    {
      

        public string CriteriaName { get; set; } = null!;

        public string? Unit { get; set; }

        public string? ReferenceRange { get; set; }

        public DateTime CreatedAt { get; set; }

      
    }
}
