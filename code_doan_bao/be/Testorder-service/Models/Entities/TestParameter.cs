using System;
using System.Collections.Generic;

namespace Testorder_service.Models.Entities;

public partial class TestParameter
{
    public string ParamId { get; set; } = null!;

    public string TestResultId { get; set; } = null!;

    public string CriteriaId { get; set; } = null!;

    public string ParamName { get; set; } = null!;

    public string? Abbreviation { get; set; }

    public string? Value { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    public virtual TestCriterion Criteria { get; set; } = null!;

    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    public virtual TestResult TestResult { get; set; } = null!;
}
