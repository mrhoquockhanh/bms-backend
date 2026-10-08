using Microsoft.AspNetCore.Mvc;
using TestOrder.Models.DTO;
using TestOrder.Services.Interface;
using System.Linq;

namespace TestOrder.Controllers
{
    [Route("TestOrder/[controller]")]
    [ApiController]

    public class BloodSampleController : ControllerBase
    {
        private readonly IBloodSampleService _service;

        public BloodSampleController(IBloodSampleService service)
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
                return NotFound(new { message = "Không có mẫu máu nào được tìm thấy." });

            return Ok(new
            {
                message = "Lấy danh sách mẫu máu thành công.",
                page,
                pageSize,
                total,
                data = items
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var data = await _service.GetByIdAsync(id);
            if (data == null) return NotFound();
            return Ok(data);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] BloodSampleCreateDto dto)
        {
            if (dto == null)
                return BadRequest("Invalid blood sample data.");

            var created = await _service.CreateAsync(dto);

            if (created == null)
                return NotFound("Failed to create blood sample.");

            return CreatedAtAction(nameof(GetById), new { id = created.SampleId }, created);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] BloodSampleUpdateDto dto)
        {
            var success = await _service.UpdateAsync(id, dto);
            if (!success) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var success = await _service.DeleteAsync(id);
            if (!success) return NotFound();
            return NoContent();
        }

        [HttpGet("by-appointment/{appointmentId}")]
        public async Task<IActionResult> GetByAppointmentId(string appointmentId)
        {
            if (string.IsNullOrWhiteSpace(appointmentId))
                return BadRequest("Appointment ID is required.");

            var samples = await _service.GetBloodSampleByAppointmentIdAsync(appointmentId);

            if (samples == null || !samples.Any())
                return NotFound($"No blood samples found for appointment ID: {appointmentId}");

            return Ok(samples);
        }


    }
}
