using TestOrder.Models.DTO;
using TestOrder.Repositories.Interface;
using TestOrder.Services.Interface;
using Testorder_service.Models.Entities;

namespace TestOrder.Services.Implement
{
    public class TestParameterService : ITestParameterService
    {
        private readonly ITestParameterRepository _repo;

        public TestParameterService(ITestParameterRepository repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<TestParameterDto>> GetAllAsync()
        {
            var list = await _repo.GetAllAsync();
            return list.Select(x => new TestParameterDto
            {
                ParamId = x.ParamId,
                TestResultId = x.TestResultId,
                CriteriaId = x.CriteriaId,
                ParamName = x.ParamName,
                Abbreviation = x.Abbreviation,
                Value = x.Value,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt
            });
        }

        public async Task<TestParameterDto?> GetByIdAsync(string id)
        {
            var x = await _repo.GetByIdAsync(id);
            if (x == null) return null;

            return new TestParameterDto
            {
                ParamId = x.ParamId,
                TestResultId = x.TestResultId,
                CriteriaId = x.CriteriaId,
                ParamName = x.ParamName,
                Abbreviation = x.Abbreviation,
                Value = x.Value,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt
            };
        }

        public async Task CreateAsync(CreateTestParameterDto dto)
        {
            var entity = new TestParameter
            {
                ParamId = Guid.NewGuid().ToString(),
                TestResultId = dto.TestResultId,
                CriteriaId = dto.CriteriaId,
                ParamName = dto.ParamName,
                Abbreviation = dto.Abbreviation,
                Value = dto.Value,
                CreatedAt = DateTime.Now
            };

            await _repo.AddAsync(entity);
            await _repo.SaveChangesAsync();
        }

        public async Task UpdateAsync(string id, UpdateTestParameterDto dto)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null)
                throw new KeyNotFoundException("TestParameter not found.");

            entity.CriteriaId = dto.CriteriaId ?? entity.CriteriaId;
            entity.ParamName = dto.ParamName ?? entity.ParamName;
            entity.Abbreviation = dto.Abbreviation ?? entity.Abbreviation;
            entity.Value = dto.Value ?? entity.Value;
            entity.UpdatedAt = DateTime.Now;

            await _repo.UpdateAsync(entity);
            await _repo.SaveChangesAsync();
        }

        public async Task DeleteAsync(string id)
        {
            await _repo.DeleteAsync(id);
            await _repo.SaveChangesAsync();
        }

        public Task<List<TestParameter>> GetByTestResultIdAsync(string resultId)
        {
            return _repo.GetByTestResultIdAsync(resultId);
        }
    }
}
