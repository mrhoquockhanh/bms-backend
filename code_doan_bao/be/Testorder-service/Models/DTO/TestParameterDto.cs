namespace TestOrder.Models.DTO
{
    public class TestParameterDto
    {
        public string ParamId { get; set; } = null!;
        public string TestResultId { get; set; } = null!;
        public string CriteriaId { get; set; } = null!;
        public string ParamName { get; set; } = null!;
        public string? Abbreviation { get; set; }
        public string? Value { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
