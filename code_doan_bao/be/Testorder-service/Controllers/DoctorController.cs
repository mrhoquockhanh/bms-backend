using Microsoft.AspNetCore.Mvc;
using TestOrder.Models.DTO;
using TestOrder.Services.Interface;
using System.Linq;

namespace TestOrder.Controllers
{
    [ApiController]
    [Route("TestOrder/[controller]")]
    public class DoctorController : ControllerBase
    {
        private readonly IDoctorService _service;
        public DoctorController(IDoctorService service)
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
                return NotFound(new { message = "Không có bác sĩ nào được tìm thấy." });

            return Ok(new
            {
                message = "Lấy danh sách bác sĩ thành công.",
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
        public async Task<IActionResult> Create([FromBody] DoctorCreateDto dto)
        {
            if (dto == null)
                return BadRequest("Doctor data is required.");

            var result = await _service.CreateAsync(dto);

            if (result == null)
                return NotFound("Failed to create doctor. Please check related data.");

            return CreatedAtAction(nameof(GetById), new { id = result.DoctorId }, result);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] DoctorUpdateDto dto)
        {
            dto.DoctorId = id;
            var result = await _service.UpdateAsync(dto);
            return result == null ? NotFound() : Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var success = await _service.DeleteAsync(id);
            return success ? NoContent() : NotFound();
        }

        [HttpGet("email/{email}")]
        public async Task<IActionResult> GetByEmail(string email)
        {
            var result = await _service.GetByEmailAsync(email);
            if (result == null || !result.Any())
                return NotFound(new { message = "Không tìm thấy bác sĩ nào với email này." });

            return Ok(new
            {
                message = "Lấy danh sách bác sĩ theo email thành công.",
                total = result.Count(),
                data = result
            });
        }
    }
}
