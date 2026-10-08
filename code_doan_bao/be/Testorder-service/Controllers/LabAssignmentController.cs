using Microsoft.AspNetCore.Mvc;
using TestOrder.Models.DTO;
using TestOrder.Services.Interface;
using Testorder_service.Models.DTO;
using Testorder_service.Services.Interface;

namespace TestOrder.Controllers
{
    [ApiController]
    [Route("TestOrder/[controller]")]
    public class LabAssignmentController : ControllerBase
    {
        private readonly ILabAssignmentService _service;

        public LabAssignmentController(ILabAssignmentService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var data = await _service.GetAllAsync();
            if (!data.Any())
                return NotFound(new { message = "Không có dữ liệu phân công nào." });

            var total = data.Count();
            var paged = data.Skip((page - 1) * pageSize).Take(pageSize);
            return Ok(new { total, page, pageSize, data = paged });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var result = await _service.GetByIdAsync(id);
            if (result == null)
                return NotFound(new { message = $"Không tìm thấy phân công có ID {id}" });

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] LabAssignmentCreateDto dto)
        {
            try
            {
                var created = await _service.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = created.AssignmentId }, created);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Lỗi khi tạo phân công: {ex.Message}" });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] LabAssignmentUpdateDto dto)
        {
            try
            {
                await _service.UpdateAsync(id, dto);
                return Ok(new { message = "Cập nhật phân công thành công." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Lỗi khi cập nhật: {ex.Message}" });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                await _service.DeleteAsync(id);
                return Ok(new { message = "Xóa phân công thành công." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Lỗi khi xóa: {ex.Message}" });
            }
        }
        [HttpPost("bulk")]
        public async Task<IActionResult> CreateBulk([FromBody] IEnumerable<LabAssignmentCreateDto> dtos)
        {
            if (dtos == null || !dtos.Any())
                return BadRequest(new { message = "Danh sách phân công trống." });

            var result = await _service.CreateBulkAsync(dtos);
            return Ok(new { message = "Tạo phân công hàng loạt thành công.", data = result });
        }

    }
}
