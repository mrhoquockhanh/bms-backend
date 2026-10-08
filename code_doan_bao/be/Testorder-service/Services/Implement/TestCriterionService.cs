using TestOrder.Models.DTO;
using TestOrder.Repositories.Interface;
using TestOrder.Services.Interface;
using Testorder_service.Models.Entities;

namespace TestOrder.Services.Implement
{
    public class TestCriterionService : ITestCriterionService
    {
        private readonly ITestCriterionRepository _repo;

        public TestCriterionService(ITestCriterionRepository repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<TestCriterionDto>> GetAllAsync()
        {
            var data = await _repo.GetAllAsync();
            return data.Select(x => new TestCriterionDto
            {
                CriteriaId = x.CriteriaId,
                CriteriaName = x.CriteriaName,
                Unit = x.Unit,
                Description = x.Description,
                ReferenceMin = x.ReferenceMin,
                ReferenceMax = x.ReferenceMax,
                ActiveFlag = x.ActiveFlag
            });
        }

        public async Task<TestCriterionDto?> GetByIdAsync(string id)
        {
            var x = await _repo.GetByIdAsync(id);
            if (x == null) return null;
            return new TestCriterionDto
            {
                CriteriaId = x.CriteriaId,
                CriteriaName = x.CriteriaName,
                Unit = x.Unit,
                Description = x.Description,
                ReferenceMin = x.ReferenceMin,
                ReferenceMax = x.ReferenceMax,
                ActiveFlag = x.ActiveFlag
            };
        }

        public async Task CreateAsync(CreateTestCriterionDto dto)
        {
            var random = new Random();
            var entity = new TestCriterion
            {
                CriteriaId = $"CRI{random.Next(0, 999999):D6}",
                CriteriaName = dto.CriteriaName,
                Description = dto.Description,
                Unit = dto.Unit,
                ReferenceMin = dto.ReferenceMin,
                ReferenceMax = dto.ReferenceMax,
                CreatedAt = DateTime.Now,
                ActiveFlag = true
            };
            await _repo.AddAsync(entity);
            await _repo.SaveChangesAsync();
        }

        public async Task UpdateAsync(string id, UpdateTestCriterionDto dto)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) throw new KeyNotFoundException("Criterion not found.");

            entity.Description = dto.Description ?? entity.Description;
            entity.Unit = dto.Unit ?? entity.Unit;
            entity.ReferenceMin = dto.ReferenceMin ?? entity.ReferenceMin;
            entity.ReferenceMax = dto.ReferenceMax ?? entity.ReferenceMax;
            entity.ActiveFlag = dto.ActiveFlag ?? entity.ActiveFlag;
            entity.UpdatedAt = DateTime.Now;

            await _repo.UpdateAsync(entity);
            await _repo.SaveChangesAsync();
        }

        public async Task DeleteAsync(string id)
        {
            await _repo.DeleteAsync(id);
            await _repo.SaveChangesAsync();
        }

    }
}
