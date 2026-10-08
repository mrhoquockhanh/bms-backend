namespace TestOrder.Models.DTO
{
    public class CreateTestCriterionDto
    {
        public string CriteriaName { get; set; } = null!;
        public string? Description { get; set; }
        public string? Unit { get; set; }
        public decimal? ReferenceMin { get; set; }
        public decimal? ReferenceMax { get; set; }
    }
}
