using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TestOrder.Models.DTO;
using TestOrder.Services.Interface;
using Testorder_service.Models.DTO;
using Testorder_service.Services.Interface;

namespace TestOrder.Controllers
{
    [Route("TestOrder/[controller]")]
    [ApiController]
    public class LabRoomController : ControllerBase
    {
        private readonly ILabRoomService _service;

        public LabRoomController(ILabRoomService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            if (page <= 0 || pageSize <= 0)
                return BadRequest(new { message = "Giá trị phân trang không hợp lệ." });

            var allRooms = await _service.GetAllAsync();
            var totalCount = allRooms.Count();

            var pagedRooms = allRooms
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            if (!pagedRooms.Any())
                return NotFound(new { message = "Không có phòng nào được tìm thấy." });

            return Ok(new
            {
                currentPage = page,
                pageSize,
                totalCount,
                totalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
                data = pagedRooms
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var room = await _service.GetByIdAsync(id);
            if (room == null)
                return NotFound(new { message = "Không tìm thấy phòng." });

            return Ok(room);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateLabRoomDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _service.CreateAsync(dto);
                return Ok(new { message = "Tạo phòng thành công." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi tạo phòng.", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateLabRoomDto dto)
        {
            try
            {
                await _service.UpdateAsync(id, dto);
                return Ok(new { message = "Cập nhật phòng thành công." });
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "Không tìm thấy phòng để cập nhật." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi cập nhật phòng.", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                await _service.DeleteAsync(id);
                return Ok(new { message = "Xóa phòng thành công." });
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "Không tìm thấy phòng để xóa." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi xóa phòng.", error = ex.Message });
            }
        }
    }
}
