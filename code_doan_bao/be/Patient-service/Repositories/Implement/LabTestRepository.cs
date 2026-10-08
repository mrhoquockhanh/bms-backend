using Microsoft.EntityFrameworkCore;
using Patient_service.Models;
using Patient_service.Models.Dto;
using Patient_service.Repositories.Interface;

namespace Patient_service.Repositories
{
    public class LabTestRepository : ILabTestRepository
    {
        private readonly PatientServiceContext _context;

        public LabTestRepository(PatientServiceContext context)
        {
            _context = context;
        }

        // 🧠 Helper để Include tất cả quan hệ
        private IQueryable<LabTest> IncludeAllRelations()
        {
            return _context.LabTests;
                //.Include(t => t.Criteria); // N–N qua lab_test_criteria
                ////.Include(t => t.MedicalRecords); // N–N qua medical_record_lab_tests
        }

        // 📋 Lấy tất cả Lab Tests
        public async Task<IEnumerable<LabTest>> GetAllAsync()
        {
            return await IncludeAllRelations()
                .ToListAsync();
        }

        // 🔍 Lấy theo ID
        public async Task<LabTest?> GetByIdAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("LabTest ID cannot be null or empty.", nameof(id));

            return await _context.LabTests.FirstOrDefaultAsync(t => t.Id == id) ;
            
        }

        // ➕ Thêm mới (có thể thêm kèm danh sách tiêu chí)
        public async Task<LabTest> AddAsync(LabTestDto labTestDto)
        {
            if (labTestDto == null)
                throw new ArgumentNullException(nameof(labTestDto));

            var labTest = new LabTest
            {
                Id = "LT-" + Guid.NewGuid().ToString("N")[..8].ToUpper(),
                TestName = labTestDto.TestName,
                Description = labTestDto.Description,
                Price = labTestDto.Price,
                CreatedAt = DateTime.UtcNow
            };

            // Thêm danh sách tiêu chí (nếu có)
            if (labTestDto.CriteriaIds != null && labTestDto.CriteriaIds.Any())
            {
                var criteria = await _context.LabCriteria
                    .Where(c => labTestDto.CriteriaIds.Contains(c.Id))
                    .ToListAsync();

                foreach (var c in criteria)
                    labTest.Criteria.Add(c);
            }

            await _context.LabTests.AddAsync(labTest);
            await _context.SaveChangesAsync();

            return await GetByIdAsync(labTest.Id) ?? labTest;
        }

        // ✏️ Cập nhật LabTest và danh sách tiêu chí
        public async Task UpdateAsync(LabTestDto labTestDto, string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("LabTest ID cannot be null or empty.", nameof(id));

            var labTest = await _context.LabTests
                .Include(t => t.Criteria)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (labTest == null)
                throw new KeyNotFoundException($"LabTest with ID {id} not found.");

            // Cập nhật thông tin chính
            labTest.TestName = labTestDto.TestName;
            labTest.Description = labTestDto.Description;
            labTest.Price = labTestDto.Price;

            // 🔁 Cập nhật danh sách tiêu chí (nếu có)
            if (labTestDto.CriteriaIds != null)
            {
                var newCriteria = await _context.LabCriteria
                    .Where(c => labTestDto.CriteriaIds.Contains(c.Id))
                    .ToListAsync();

                labTest.Criteria.Clear(); // clear liên kết cũc
                foreach (var c in newCriteria)
                    labTest.Criteria.Add(c);
            }

            _context.LabTests.Update(labTest);
            await _context.SaveChangesAsync();
        }

        // ❌ Xóa LabTest (và xóa liên kết trung gian)
        public async Task DeleteAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("LabTest ID cannot be null or empty.", nameof(id));

            var entity = await _context.LabTests
                .Include(t => t.MedicalRecords)
                .Include(t => t.Criteria)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (entity == null)
                throw new KeyNotFoundException($"LabTest with ID {id} not found.");

            // Xóa liên kết N–N (EF sẽ tự xử lý, nhưng ta có thể chủ động clear)
            entity.MedicalRecords.Clear();
            entity.Criteria.Clear();

            _context.LabTests.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<List<LabTest>> SearchAsync(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                return new List<LabTest>();

            key = key.Trim().ToLower();

            var query = _context.LabTests.AsQueryable();

            query = query.Where(l =>
                (!string.IsNullOrEmpty(l.TestName) && l.TestName.ToLower().Contains(key)) ||
                (!string.IsNullOrEmpty(l.Description) && l.Description.ToLower().Contains(key))
            );

            // Tìm theo giá nếu người dùng nhập số
            if (decimal.TryParse(key, out var price))
            {
                query = query.Union(_context.LabTests.Where(l => l.Price == price));
            }

            // Tìm theo ngày tạo
            if (DateTime.TryParse(key, out var date))
            {
                query = query.Union(_context.LabTests.Where(l => l.CreatedAt.Date == date.Date));
            }

            return await query
                .OrderByDescending(l => l.CreatedAt)
                .ToListAsync();
        }
    }
}
