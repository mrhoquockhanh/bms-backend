namespace TestOrder.Models.DTO
{
    public class CreateTestParameterDto
    {
        public string TestResultId { get; set; } = null!;
        public string CriteriaId { get; set; } = null!;
        public string ParamName { get; set; } = null!;
        public string? Abbreviation { get; set; }
        public string? Value { get; set; }
    }
}
