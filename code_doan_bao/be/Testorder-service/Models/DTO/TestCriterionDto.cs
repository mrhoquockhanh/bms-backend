namespace TestOrder.Models.DTO
{
    public class TestCriterionDto
    {
        public string CriteriaId { get; set; } = null!;
        public string CriteriaName { get; set; } = null!;
        public string? Unit { get; set; }
        public string? Description { get; set; }
        public decimal? ReferenceMin { get; set; }
        public decimal? ReferenceMax { get; set; }
        public bool? ActiveFlag { get; set; }
    }
}
