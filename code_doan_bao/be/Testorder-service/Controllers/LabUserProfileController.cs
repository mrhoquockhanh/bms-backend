using Microsoft.AspNetCore.Mvc;
using TestOrder.Models.DTO;
using TestOrder.Services.Interface;
using Testorder_service.Models.DTO;
using Testorder_service.Services.Interface;

namespace TestOrder.Controllers
{
    [ApiController]
    [Route("TestOrder/[controller]")]
    public class LabUserProfileController : ControllerBase
    {
        private readonly ILabUserProfileService _service;

        public LabUserProfileController(ILabUserProfileService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var data = await _service.GetAllAsync();
            var total = data.Count();

            if (!data.Any())
                return NotFound(new { message = "Không có nhân viên nào được tìm thấy." });

            var paged = data.Skip((page - 1) * pageSize).Take(pageSize);

            return Ok(new
            {
                message = "Lấy danh sách nhân viên thành công.",
                page,
                pageSize,
                total,
                data = paged
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var result = await _service.GetByIdAsync(id);
            if (result == null)
                return NotFound(new { message = $"Không tìm thấy nhân viên với ID: {id}" });

            return Ok(new
            {
                message = "Lấy thông tin nhân viên thành công.",
                data = result
            });
        }
        [HttpGet("email/{email}")]
        public async Task<IActionResult> GetByEmail(string email)
        {
            var result = await _service.GetByEmailAsync(email);
            if (result == null)
                return NotFound(new { message = $"Không tìm thấy nhân viên với ID: {email}" });

            return Ok(new
            {
                message = "Lấy thông tin nhân viên thành công.",
                data = result
            });
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] LabUserProfileCreateDto dto)
        {
            if (dto == null)
                return BadRequest(new { message = "Dữ liệu không hợp lệ." });

            try
            {
                var result = await _service.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = result.UserId }, new
                {
                    message = "Tạo nhân viên thành công.",
                    data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Lỗi khi tạo nhân viên: {ex.Message}" });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] LabUserProfileUpdateDto dto)
        {
            if (dto == null)
                return BadRequest(new { message = "Dữ liệu cập nhật là bắt buộc." });

            try
            {
                await _service.UpdateAsync(id, dto);
                return Ok(new { message = "Cập nhật nhân viên thành công." });
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                await _service.DeleteAsync(id);
                return Ok(new { message = "Xóa nhân viên thành công." });
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}
