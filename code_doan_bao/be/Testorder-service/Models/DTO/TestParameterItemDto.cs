namespace Testorder_service.Models.DTO
{
    public class TestParameterItemDto
    {
        public string CriteriaId { get; set; } = null!;
        public string ParamName { get; set; } = null!;
        public string? Abbreviation { get; set; }
        public string? Value { get; set; }
    }
}
