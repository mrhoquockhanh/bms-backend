using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Patient_service.Models;
using Patient_service.Models.Dto;
using Patient_service.Repositories.Interface;

namespace Patient_service.Repositories
{
    public class MedicalRecordRepository : IMedicalRecordRepository
    {
        private readonly PatientServiceContext _context;
        private readonly IMapper _mapper;

        public MedicalRecordRepository(PatientServiceContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // 🧠 Helper method: Truy vấn MedicalRecord đầy đủ (giảm trùng lặp)
        private IQueryable<MedicalRecord> IncludeAllRelations()
        {
            return _context.MedicalRecords;
                
        }

        // 📋 Lấy toàn bộ Medical Records (kèm Patient + LabTests)
        // Lưu ý: Đã sửa lại để sử dụng IncludeAllRelations để lấy đầy đủ dữ liệu
        public async Task<IEnumerable<MedicalRecord>> GetAllAsync()
        {
            return await _context.MedicalRecords.ToListAsync();
        }

        // 🔍 Lấy theo ID
        public async Task<MedicalRecord?> GetByIdAsync(string id)
        {
            return await IncludeAllRelations()
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        // 🔎 Lấy theo Patient ID
        public async Task<IEnumerable<MedicalRecord>> GetByPatientIdAsync(string patientId)
        {
            return await IncludeAllRelations()
                .Where(r => r.PatientId == patientId)
                .ToListAsync();
        }

        // ➕ Thêm mới (Thêm kiểm tra PatientId)
        public async Task<MedicalRecord> AddAsync(MedicalRecordDto createDto)
        {
            // **[CHECK 1]** Kiểm tra PatientId có tồn tại hay không
            var patientExists = await _context.Patients.AnyAsync(p => p.Id == createDto.PatientId);
            if (!patientExists)
            {
                throw new KeyNotFoundException($"Patient with ID {createDto.PatientId} not found.");
            }

            var record = new MedicalRecord
            {
                Id = "MR-" + Guid.NewGuid().ToString("N").Substring(0, 10),
                PatientId = createDto.PatientId,
                RecordType = createDto.RecordType,
                Title = createDto.Title,
                Description = createDto.Description,
                RecordData = createDto.RecordData,
                Status = createDto.Status,
                OnsetDate = createDto.OnsetDate,
                EndDate = createDto.EndDate,
                CreatedBy = createDto.CreatedBy,
                VerifiedBy = createDto.VerifiedBy,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            // 🧩 Liên kết LabTests thật từ DB (Thêm kiểm tra LabTestIds)
            if (createDto.LabTestIds?.Any() == true)
            {
                var validLabTests = await _context.LabTests
                    .Where(t => createDto.LabTestIds.Contains(t.Id))
                    .ToListAsync();

                // **[CHECK 2]** Kiểm tra tất cả LabTestIds đều hợp lệ
                if (validLabTests.Count != createDto.LabTestIds.Count)
                {
                    var missingIds = createDto.LabTestIds.Except(validLabTests.Select(t => t.Id));
                    throw new InvalidOperationException($"LabTests not found: {string.Join(", ", missingIds)}");
                }

                record.LabTests = validLabTests;
            }

            _context.MedicalRecords.Add(record);
            await _context.SaveChangesAsync();

            // Load lại để trả về đầy đủ quan hệ
            return await IncludeAllRelations()
                .FirstAsync(r => r.Id == record.Id);
        }

        // ✏️ Cập nhật (Thêm kiểm tra PatientId và LabTestIds)
        public async Task UpdateAsync(MedicalRecordDto recordDto, string id)
        {
            var record = await _context.MedicalRecords
                .Include(r => r.LabTests)
                .FirstOrDefaultAsync(r => r.Id == id);

            // **[CHECK 1]** Kiểm tra MedicalRecord có tồn tại không
            if (record == null)
                throw new KeyNotFoundException($"MedicalRecord with ID {id} not found.");

            // **[CHECK 2]** Kiểm tra PatientId mới có tồn tại không (nếu thay đổi)
            if (record.PatientId != recordDto.PatientId)
            {
                var patientExists = await _context.Patients.AnyAsync(p => p.Id == recordDto.PatientId);
                if (!patientExists)
                {
                    throw new KeyNotFoundException($"Patient with ID {recordDto.PatientId} not found.");
                }
            }


            // Cập nhật thông tin cơ bản
            record.PatientId = recordDto.PatientId;
            record.RecordType = recordDto.RecordType;
            record.Title = recordDto.Title;
            record.Description = recordDto.Description;
            record.RecordData = recordDto.RecordData;
            record.Status = recordDto.Status;
            record.OnsetDate = recordDto.OnsetDate;
            record.EndDate = recordDto.EndDate;
            record.CreatedBy = recordDto.CreatedBy;
            record.VerifiedBy = recordDto.VerifiedBy;
            record.UpdatedAt = DateTime.UtcNow;

            // Cập nhật danh sách LabTests (nếu có)
            if (recordDto.LabTestIds != null)
            {
                // Lấy lab tests mới
                var validLabTests = await _context.LabTests
                    .Where(t => recordDto.LabTestIds.Contains(t.Id))
                    .ToListAsync();

                // **[CHECK 3]** Kiểm tra tất cả LabTestIds đều hợp lệ
                if (validLabTests.Count != recordDto.LabTestIds.Count)
                {
                    var missingIds = recordDto.LabTestIds.Except(validLabTests.Select(t => t.Id));
                    throw new InvalidOperationException($"LabTests not found: {string.Join(", ", missingIds)}");
                }

                // Xóa liên kết cũ và gán mới
                record.LabTests.Clear();
                foreach (var test in validLabTests)
                {
                    record.LabTests.Add(test);
                }
            }

            // Đánh dấu bản ghi là Modified để EF Core biết rằng nó cần được cập nhật
            _context.MedicalRecords.Update(record);
            await _context.SaveChangesAsync();
        }

        // ❌ Xóa (Giữ nguyên kiểm tra)
        public async Task DeleteAsync(string id)
        {
            var record = await _context.MedicalRecords
                .FirstOrDefaultAsync(r => r.Id == id);

            if (record == null)
                throw new KeyNotFoundException($"MedicalRecord with ID {id} not found.");

            // EF Core sẽ tự xóa bản ghi trung gian của quan hệ N-N
            _context.MedicalRecords.Remove(record);
            await _context.SaveChangesAsync();
        }

        public async Task<List<MedicalRecord>> SearchAsync(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                return new List<MedicalRecord>();

            key = key.Trim().ToLower();

            var query = _context.MedicalRecords
                .AsQueryable();

            query = query.Where(m =>
                (!string.IsNullOrEmpty(m.RecordType) && m.RecordType.ToLower().Contains(key)) ||
                (!string.IsNullOrEmpty(m.Title) && m.Title.ToLower().Contains(key)) ||
                (!string.IsNullOrEmpty(m.Description) && m.Description.ToLower().Contains(key)) ||
                (!string.IsNullOrEmpty(m.RecordData) && m.RecordData.ToLower().Contains(key)) ||
                (!string.IsNullOrEmpty(m.Status) && m.Status.ToLower().Contains(key)) ||
                (!string.IsNullOrEmpty(m.CreatedBy) && m.CreatedBy.ToLower().Contains(key)) ||
                (!string.IsNullOrEmpty(m.VerifiedBy) && m.VerifiedBy.ToLower().Contains(key)) ||
                (m.Patient != null && !string.IsNullOrEmpty(m.Patient.Fullname) && m.Patient.Fullname.ToLower().Contains(key))
                
            );

            // Tìm theo ngày nếu người dùng nhập dạng yyyy-MM-dd
            if (DateOnly.TryParse(key, out var date))
            {
                query = query.Union(_context.MedicalRecords
                    //.Include(m => m.Patient)
                    .Where(m => m.OnsetDate == date || m.EndDate == date));
            }

            return await query
                .OrderByDescending(m => m.UpdatedAt)
                .ThenByDescending(m => m.CreatedAt)
                .ToListAsync();
        }
    }
}