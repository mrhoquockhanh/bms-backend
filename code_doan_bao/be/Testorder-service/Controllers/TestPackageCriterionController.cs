using Microsoft.AspNetCore.Mvc;
using TestOrder.Models.DTO;
using TestOrder.Services.Interface;
using System.Linq;

namespace TestOrder.Controllers
{
    [Route("TestOrder/[controller]")]
    [ApiController]

    public class TestPackageCriterionController : ControllerBase
    {
        private readonly ITestPackageCriterionService _service;

        public TestPackageCriterionController(ITestPackageCriterionService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var data = await _service.GetAllAsync();
            var total = data.Count();
            var items = data.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            if (!items.Any())
                return NotFound(new { message = "Không có tiêu chí gói xét nghiệm nào được tìm thấy." });

            return Ok(new
            {
                message = "Lấy danh sách tiêu chí gói xét nghiệm thành công.",
                page,
                pageSize,
                total,
                data = items
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var result = await _service.GetByIdAsync(id);
            return result == null
                ? NotFound(new { message = "Không tìm thấy tiêu chí gói xét nghiệm." })
                : Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateTestPackageCriterionDto dto)
        {
            try
            {
                await _service.CreateAsync(dto);
                return Ok(new { message = "Tạo tiêu chí gói xét nghiệm thành công." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Tạo tiêu chí gói xét nghiệm thất bại.", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateTestPackageCriterionDto dto)
        {
            try
            {
                await _service.UpdateAsync(id, dto);
                return Ok(new { message = "Cập nhật tiêu chí gói xét nghiệm thành công." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Cập nhật tiêu chí gói xét nghiệm thất bại.", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                await _service.DeleteAsync(id);
                return Ok(new { message = "Xóa tiêu chí gói xét nghiệm thành công." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Xóa tiêu chí gói xét nghiệm thất bại.", error = ex.Message });
            }
        }

    }
}
