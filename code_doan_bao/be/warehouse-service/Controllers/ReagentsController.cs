using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using warehouse_service.Data;
using warehouse_service.Models;
using warehouse_service.Mapping;
using warehouse_service.Models.DTO;
using System.Text.Json;

namespace warehouse_service.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ReagentsController : ControllerBase
    {
        private readonly warehouse_service.Services.Interface.IReagentService _service;

        public ReagentsController(warehouse_service.Services.Interface.IReagentService service)
        {
            _service = service;
        }

        // GET: Reagents?pageNumber=1&pageSize=10&q=search
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string? q = null)
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 10;

            var (items, total) = await _service.GetPagedAsync(pageNumber, pageSize, q);
            return Ok(new { total, pageNumber, pageSize, items });
        }

        // GET: Reagents/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var dto = await _service.GetByIdAsync(id);
            if (dto == null) return NotFound();
            return Ok(dto);
        }

        // POST: Reagents
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ReagentCreateDto dto)
        {
            var entity = dto.ToEntity();

            var created = await _service.CreateAsync(entity);

            return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
        }




        // PUT: Reagents/5
        // Standard PUT: accepts ReagentUpdateDto to update fields (status, quantity, etc.)
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, [FromBody] ReagentUpdateDto dto)
        {
            try
            {
                var updated = await _service.UpdateAsync(id, dto);
                return Ok(updated);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // PATCH: Reagents/5/subtract
        // Accepts a plain integer in the body representing how much to subtract from quantity.
        [HttpPatch("{id}/subtract")]
        public async Task<IActionResult> SubtractQuantity(string id, [FromBody] int amount)
        {
            if (amount < 0) return BadRequest(new { message = "Amount to subtract must be non-negative" });

            try
            {
                var updated = await _service.SubtractQuantityAsync(id, amount);
                return Ok(updated);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                await _service.DeleteAsync(id);
                return NoContent(); // 204 — Xóa thành công, không trả body
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message }); // 404 nếu không tìm thấy
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message }); // 400 nếu lỗi khác
            }
        }

    }
}