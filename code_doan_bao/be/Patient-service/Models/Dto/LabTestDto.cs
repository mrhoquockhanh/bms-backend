namespace Patient_service.Models.Dto
{

    public partial class LabTestDto
    {


        public string TestName { get; set; } = null!;

        public string? Description { get; set; }

        public decimal Price { get; set; }

        public DateTime CreatedAt { get; set; }
        public List<string>? CriteriaIds { get; set; }

    }
}
