using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using warehouse_service.Data;
using warehouse_service.Mapping;
using warehouse_service.Models.Entities;
using warehouse_service.Models.DTO;

namespace warehouse_service.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EquipmentController : ControllerBase
    {
        private readonly warehouse_service.Services.Interface.IEquipmentService _service;

        public EquipmentController(warehouse_service.Services.Interface.IEquipmentService service)
        {
            _service = service;
        }

        // GET: Equipment?pageNumber=1&pageSize=10&q=search
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string? q = null)
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 10;

            var (items, total) = await _service.GetPagedAsync(pageNumber, pageSize, q);
            return Ok(new { total, pageNumber, pageSize, items });
        }

        // GET: Equipment/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var dto = await _service.GetByIdAsync(id);
            if (dto == null) return NotFound();
            return Ok(dto);
        }

        // POST: Equipment
        [HttpPost]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] EquipmentCreateDto dto)
        {
            var entity = new Equipment
            {
                EquipmentName = dto.EquipmentName,
                Manufacturer = dto.Manufacturer,
                CodeManufacturer = dto.CodeManufacturer,
                Status = dto.Status,
                LastMaintenanceDate = dto.LastMaintenanceDate,
                Description = dto.Description
            };

            var result = await _service.CreateAsync(entity);

            return CreatedAtAction(nameof(Get), new { id = result.Id }, result);
        }


        // PUT: Equipment/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, [FromBody] EquipmentUpdateDto dto)
        {
            try
            {
                var updated = await _service.UpdateStatusAsync(id, dto);
                return Ok(updated);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        // DELETE: Equipment/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                await _service.DeleteAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

    }

}