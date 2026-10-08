using Microsoft.AspNetCore.Mvc;
using TestOrder.Models.DTO;
using TestOrder.Services.Interface;
using System.Linq;

namespace TestOrder.Controllers
{
   
    [Route("TestOrder/[controller]")]
    [ApiController]
    public class TestOrderCommentController : ControllerBase
    {
        private readonly ITestOrderCommentService _service;

        public TestOrderCommentController(ITestOrderCommentService service)
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
                return NotFound(new { message = "Không có bình luận nào được tìm thấy." });

            return Ok(new
            {
                message = "Lấy danh sách bình luận thành công.",
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
            return result == null ? NotFound() : Ok(result);
        }

        [HttpGet("appointment/{appointmentId}")]
        public async Task<IActionResult> GetByAppointmentId(string appointmentId)
        {
            var data = await _service.GetByAppointmentIdAsync(appointmentId);
            if (data == null || !data.Any())
                return NotFound(new { message = "Không tìm thấy bình luận nào cho cuộc hẹn này." });

            return Ok(new
            {
                message = "Lấy danh sách bình luận theo cuộc hẹn thành công.",
                data
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateTestOrderCommentDto dto)
        {
            try
            {
                await _service.CreateAsync(dto);
                return Ok(new { message = "Tạo bình luận thành công." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Tạo bình luận thất bại.", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateTestOrderCommentDto dto)
        {
            try
            {
                await _service.UpdateAsync(id, dto);
                return Ok(new { message = "Cập nhật bình luận thành công." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Cập nhật bình luận thất bại.", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                await _service.DeleteAsync(id);
                return Ok(new { message = "Xóa bình luận thành công." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Xóa bình luận thất bại.", error = ex.Message });
            }
        }

    }
}
