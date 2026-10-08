namespace Patient_service.Models.Dto
{
    public class LabTestCreateDto
{
    public string Id { get; set; } = null!;

    public string TestName { get; set; } = null!;

    public string? Description { get; set; }

    public decimal Price { get; set; }

}
}
