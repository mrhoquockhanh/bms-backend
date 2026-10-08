using Microsoft.AspNetCore.Mvc;
using TestOrder.Models.DTO;
using TestOrder.Services.Interface;

namespace TestOrder.Controllers
{
    [ApiController]
    [Route("TestOrder/[controller]")]
    public class DoctorRoomController : ControllerBase
    {
        private readonly IDoctorRoomService _service;

        public DoctorRoomController(IDoctorRoomService service)
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
                return NotFound(new { message = "Không có phòng nào được tìm thấy." });

            return Ok(new
            {
                message = "Lấy danh sách phòng thành công.",
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
            if (result == null)
                return NotFound(new { message = $"Không tìm thấy phòng với ID: {id}" });

            return Ok(new { message = "Lấy thông tin phòng thành công.", data = result });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] DoctorRoomCreateDto dto)
        {
            if (dto == null)
                return BadRequest(new { message = "Dữ liệu phòng là bắt buộc." });

            var result = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.RoomId }, 
                new { message = "Tạo phòng thành công.", data = result });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] DoctorRoomUpdateDto dto)
        {
            if (dto == null)
                return BadRequest(new { message = "Dữ liệu cập nhật là bắt buộc." });

            var result = await _service.UpdateAsync(id, dto);
            if (result == null)
                return NotFound(new { message = $"Không tìm thấy phòng với ID: {id}" });

            return Ok(new { message = "Cập nhật phòng thành công.", data = result });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _service.DeleteAsync(id);
            if (!result)
                return NotFound(new { message = $"Không tìm thấy phòng với ID: {id}" });

            return Ok(new { message = "Xóa phòng thành công." });
        }
    }
}