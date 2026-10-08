namespace TestOrder.Models.DTO
{
    public class UpdateTestCriterionDto
    {
        public string? Description { get; set; }
        public string? Unit { get; set; }
        public decimal? ReferenceMin { get; set; }
        public decimal? ReferenceMax { get; set; }
        public bool? ActiveFlag { get; set; }
    }
}
