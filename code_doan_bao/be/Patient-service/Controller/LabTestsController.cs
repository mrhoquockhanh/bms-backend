using Microsoft.AspNetCore.Mvc;
using Service.Interface;
using Patient_service.Models;
using Patient_service.Models.Dto;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Linq;
using System.Net.WebSockets;

namespace Patient_service.Controllers
{
    [ApiController]
    [Route("Patients/lab-tests")]
    public class LabTestsController : ControllerBase
    {
        private readonly ILabTestService _labTestService;

        public LabTestsController(ILabTestService labTestService)
        {
            _labTestService = labTestService;
        }

        [HttpGet]
        public async Task<IActionResult> ApiGetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var tests = await _labTestService.GetAllAsync();
            var total = tests.Count();
            var items = tests.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            if (!items.Any())
                return NotFound(new { message = "Không có xét nghiệm nào." });

            return Ok(new
            {
                message = "Lấy danh sách xét nghiệm thành công.",
                page,
                pageSize,
                total,
                data = items
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var test = await _labTestService.GetByIdAsync(id);
            if (test == null)
                return NotFound(new { message = $"Không tìm thấy xét nghiệm với ID: {id}" });
            return Ok(new { message = "Lấy xét nghiệm thành công.", data = test });
        }
        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string key, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            if (string.IsNullOrWhiteSpace(key))
                return BadRequest(new { message = "Vui lòng nhập từ khóa tìm kiếm." });

            var results = await _labTestService.SearchAsync(key);
            if (results == null || !results.Any())
                return NotFound(new { message = "Không tìm thấy xét nghiệm phù hợp." });

            var total = results.Count();
            var items = results.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            return Ok(new
            {
                message = "Tìm kiếm xét nghiệm thành công.",
                key,
                page,
                pageSize,
                total,
                data = items
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] LabTestDto test)
        {
          var check = await _labTestService.AddAsync(test);
           if(check == null)
            {
                return NotFound(new { message = "tạo xét nghiệm thất bại" });
            }
            return Ok(new { message = "Tạo xét nghiệm thành công.", data = test });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] LabTestDto test)
        {
            var existing = await _labTestService.GetByIdAsync(id);
            if (existing == null)
                return NotFound(new { message = $"Không tìm thấy xét nghiệm với ID: {id}" });

            await _labTestService.UpdateAsync(test, id);
            return Ok(new { message = "Cập nhật xét nghiệm thành công." });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var existing = await _labTestService.GetByIdAsync(id);
            if (existing == null)
                return NotFound(new { message = $"Không tìm thấy xét nghiệm với ID: {id}" });

            await _labTestService.DeleteAsync(id);
            return Ok(new { message = "Xóa xét nghiệm thành công." });
        }
    }
}
