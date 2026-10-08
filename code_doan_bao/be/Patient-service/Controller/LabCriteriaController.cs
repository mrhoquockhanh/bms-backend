using Microsoft.AspNetCore.Mvc;
using Patient_service.Models;
using Patient_service.Models.Dto;
using Service.Implement;
using Service.Interface;
using System.Linq;

namespace Patient_service.Controllers
{
    [ApiController]
    [Route("Patients/lab-criteria")]
    public class LabCriteriaController : ControllerBase
    {
        private readonly ILabCriterionService _service;

        public LabCriteriaController(ILabCriterionService service)
        {
            _service = service;
        }

        
        // GET: api/lab-criteria
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var criteria = await _service.GetAllAsync();
            var total = criteria.Count();
            var items = criteria.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            if (!items.Any())
                return NotFound(new { message = "Không có tiêu chí xét nghiệm nào." });

            return Ok(new
            {
                message = "Lấy danh sách tiêu chí xét nghiệm thành công.",
                page,
                pageSize,
                total,
                data = items
            });
        }


        // GET: api/lab-criteria/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var criterion = await _service.GetByIdAsync(id);
            if (criterion == null)
                return NotFound(new { message = $"Không tìm thấy tiêu chí xét nghiệm với ID: {id}" });
            return Ok(new { message = "Lấy tiêu chí xét nghiệm thành công.", data = criterion });
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string key, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            if (string.IsNullOrWhiteSpace(key))
                return BadRequest(new { message = "Vui lòng nhập từ khóa tìm kiếm." });

            var results = await _service.SearchAsync(key);
            if (results == null || !results.Any())
                return NotFound(new { message = "Không tìm thấy tiêu chí xét nghiệm phù hợp." });

            var total = results.Count();
            var items = results.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            return Ok(new
            {
                message = "Tìm kiếm tiêu chí xét nghiệm thành công.",
                key,
                page,
                pageSize,
                total,
                data = items
            });
        }
        // POST: api/lab-criteria
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] LabCriterionDto criterion)
        {
            if (criterion == null)
                return BadRequest(new { message = "Dữ liệu gửi lên không hợp lệ." });

            await _service.AddAsync(criterion);
            return Ok(new { message = "Tạo tiêu chí xét nghiệm thành công.", data = criterion });
        }

        // PUT: api/lab-criteria/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] LabCriterionDto criterion)
        {
            var existing = await _service.GetByIdAsync(id);
            if (existing == null)
                return NotFound(new { message = $"Không tìm thấy tiêu chí xét nghiệm với ID: {id}" });

            await _service.UpdateAsync(criterion, id);
            return Ok(new { message = "Cập nhật tiêu chí xét nghiệm thành công." });
        }

        // DELETE: api/lab-criteria/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var existing = await _service.GetByIdAsync(id);
            if (existing == null)
                return NotFound(new { message = $"Không tìm thấy tiêu chí xét nghiệm với ID: {id}" });

            await _service.DeleteAsync(id);
            return Ok(new { message = "Xóa tiêu chí xét nghiệm thành công." });
        }
    }
}
