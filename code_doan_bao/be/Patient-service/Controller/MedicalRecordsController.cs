using Microsoft.AspNetCore.Mvc;
using Service.Interface;
using Patient_service.Models;
using Patient_service.Models.Dto;
using System.Linq;

namespace Patient_service.Controllers
{
    [ApiController]
    [Route("Patients/medical-records")]
    public class MedicalRecordsController : ControllerBase
    {
        private readonly IMedicalRecordService _medicalRecordService;

        public MedicalRecordsController(IMedicalRecordService medicalRecordService)
        {
            _medicalRecordService = medicalRecordService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var records = await _medicalRecordService.GetAllAsync();
            var total = records.Count();
            var items = records.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            if (!items.Any())
                return NotFound(new { message = "Không có hồ sơ bệnh án nào." });

            return Ok(new
            {
                message = "Lấy danh sách hồ sơ bệnh án thành công.",
                page,
                pageSize,
                total,
                data = items
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var record = await _medicalRecordService.GetByIdAsync(id);
            if (record == null)
                return NotFound(new { message = $"Không tìm thấy hồ sơ bệnh án với ID: {id}" });
            return Ok(new { message = "Lấy hồ sơ bệnh án thành công.", data = record });
        }

        [HttpGet("by-patient/{patientId}")]
        public async Task<IActionResult> GetByPatientId(string patientId)
        {
            var records = await _medicalRecordService.GetByPatientIdAsync(patientId);
            if (records == null || !records.Any())
                return NotFound(new { message = "Không có hồ sơ bệnh án nào cho bệnh nhân này." });
            return Ok(new { message = "Lấy hồ sơ bệnh án theo bệnh nhân thành công.", data = records });
        }
        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string key, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            if (string.IsNullOrWhiteSpace(key))
                return BadRequest(new { message = "Vui lòng nhập từ khóa tìm kiếm." });

            var results = await _medicalRecordService.SearchAsync(key);

            if (results == null || !results.Any())
                return NotFound(new { message = "Không tìm thấy hồ sơ bệnh án phù hợp." });

            var total = results.Count();
            var items = results.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            return Ok(new
            {
                message = "Tìm kiếm hồ sơ bệnh án thành công.",
                key,
                page,
                pageSize,
                total,
                data = items
            });
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] MedicalRecordDto record)
        {
            var newRecord = await _medicalRecordService.AddAsync(record);
            if (newRecord == null)
                return NotFound(new { message = "Tạo hồ sơ bệnh án thất bại." });
            return Ok(new { message = "Tạo hồ sơ bệnh án thành công.", data = newRecord });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] MedicalRecordDto record)
        {
            var existing = await _medicalRecordService.GetByIdAsync(id);
            if (existing == null)
                return NotFound(new { message = $"Không tìm thấy hồ sơ bệnh án với ID: {id}" });

            await _medicalRecordService.UpdateAsync(record, id);
            return Ok(new { message = "Cập nhật hồ sơ bệnh án thành công." });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var existing = await _medicalRecordService.GetByIdAsync(id);
            if (existing == null)
                return NotFound(new { message = $"Không tìm thấy hồ sơ bệnh án với ID: {id}" });

            await _medicalRecordService.DeleteAsync(id);
            return Ok(new { message = "Xóa hồ sơ bệnh án thành công." });
        }
    }
}
