using Microsoft.EntityFrameworkCore;
using TestOrder.Models.DTO;
using TestOrder.Repositories.Interface;
using TestOrder.Services.Interface;
using Testorder_service.Models.Entities;

namespace TestOrder.Services.Implement
{
    public class TestPackageCriterionService : ITestPackageCriterionService
    {
        private readonly ITestPackageCriterionRepository _repo;

        public TestPackageCriterionService(ITestPackageCriterionRepository repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<TestPackageCriterionDto>> GetAllAsync()
        {
            var list = await _repo.GetAllAsync();
            return list.Select(x => new TestPackageCriterionDto
            {
                TpcId = x.TpcId,
                TestPackageId = x.TestPackageId,
                TestCriteriaId = x.TestCriteriaId
            });
        }

        public async Task<TestPackageCriterionDto?> GetByIdAsync(string id)
        {
            var x = await _repo.GetByIdAsync(id);
            if (x == null) return null;

            return new TestPackageCriterionDto
            {
                TpcId = x.TpcId,
                TestPackageId = x.TestPackageId,
                TestCriteriaId = x.TestCriteriaId
            };
        }

        public async Task CreateAsync(CreateTestPackageCriterionDto dto)
        {
            var random = new Random();
            var entity = new TestPackageCriterion
            {
                TpcId = $"TPC{random.Next(0, 999999):D6}",
                TestPackageId = dto.TestPackageId,
                TestCriteriaId = dto.TestCriteriaId
            };

            await _repo.AddAsync(entity);
            await _repo.SaveChangesAsync();
        }

        public async Task UpdateAsync(string id, UpdateTestPackageCriterionDto dto)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null)
                throw new KeyNotFoundException("TestPackageCriterion not found.");

            entity.TestPackageId = dto.TestPackageId ?? entity.TestPackageId;
            entity.TestCriteriaId = dto.TestCriteriaId ?? entity.TestCriteriaId;

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
