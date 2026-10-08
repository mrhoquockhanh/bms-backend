using Microsoft.AspNetCore.Mvc;
using TestOrder.Models.DTO;
using TestOrder.Services.Interface;
using System.Linq;

namespace TestOrder.Controllers
{
    [ApiController]
    [Route("TestOrder/[controller]")]
    public class DoctorShiftController : ControllerBase
    {
        private readonly IDoctorShiftService _service;
        public DoctorShiftController(IDoctorShiftService service)
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
                return NotFound(new { message = "Không có ca trực nào được tìm thấy." });

            return Ok(new
            {
                message = "Lấy danh sách ca trực thành công.",
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

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] DoctorShiftCreateDto dto)
        {
            if (dto == null)
                return BadRequest("Doctor shift data is required.");

            var result = await _service.CreateAsync(dto);

            if (result == null)
                return NotFound("Failed to create doctor shift. Please check related data.");

            return CreatedAtAction(nameof(GetById), new { id = result.ShiftId }, result);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] DoctorShiftUpdateDto dto)
        {
            dto.ShiftId = id;
            var result = await _service.UpdateAsync(dto);
            return result == null ? NotFound() : Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var success = await _service.DeleteAsync(id);
            return success ? NoContent() : NotFound();
        }

        [HttpGet("room/{roomId}")]
        public async Task<IActionResult> GetByRoomId(string roomId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var data = await _service.GetByRoomIdAsync(roomId);
            var total = data.Count();
            var items = data.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            if (!items.Any())
                return NotFound(new { message = $"Không có ca trực nào được tìm thấy cho phòng {roomId}." });

            return Ok(new
            {
                message = "Lấy danh sách ca trực theo phòng thành công.",
                page,
                pageSize,
                total,
                data = items
            });
        }
        [HttpPost("bulk")]
        public async Task<IActionResult> CreateBulk([FromBody] IEnumerable<DoctorShiftCreateDto> dtos)
        {
            if (dtos == null || !dtos.Any())
                return BadRequest(new { message = "Danh sách ca làm việc trống." });

            var result = await _service.CreateBulkAsync(dtos);
            return Ok(new { message = "Tạo lịch làm việc hàng loạt thành công.", data = result });
        }

    }
}
