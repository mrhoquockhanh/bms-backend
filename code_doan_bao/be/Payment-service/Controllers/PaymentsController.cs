using Microsoft.AspNetCore.Mvc;
using payment_service.Models;
using payment_service.Repositories;

namespace payment_service.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentRepository _repo;

        public PaymentsController(IPaymentRepository repo)
        {
            _repo = repo;
        }

        // ✅ Tạo mới thanh toán
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreatePaymentRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var payment = new Payment
            {
                PaymentId = Guid.NewGuid(),
                PatientId = request.PatientId,
                Amount = request.Amount,
                Notes = request.Notes,
                Status = "Pendding",
                PaidAt = request.PaidAt,
                CreatedAt = request.CreatedAt,
                TransactionCode = request.TransactionCode,
                TxnRef = request.TxnRef,
            };

            var created = await _repo.CreateAsync(payment);

            return CreatedAtAction(nameof(GetById), new { id = created.PaymentId }, created);
        }

        // ✅ Lấy thanh toán theo ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            if (page <= 0 || pageSize <= 0)
                return BadRequest("page và pageSize phải lớn hơn 0.");

            var list = await _repo.GetByPatientIdPagedAsync(id, page, pageSize);
            var totalCount = await _repo.CountByPatientIdAsync(id);

            return Ok(new
            {
                data = list,
                page,
                pageSize,
                totalCount,
                totalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            });
        }

        // ✅ Lấy danh sách thanh toán của 1 bệnh nhân (có phân trang)
        [HttpGet("by-patient/{patientId}")]
        public async Task<IActionResult> GetByPatient(Guid patientId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            if (page <= 0 || pageSize <= 0)
                return BadRequest("page và pageSize phải lớn hơn 0.");

            var list = await _repo.GetByPatientIdPagedAsync(patientId, page, pageSize);
            var totalCount = await _repo.CountByPatientIdAsync(patientId);

            return Ok(new
            {
                data = list,
                page,
                pageSize,
                totalCount,
                totalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            });
        }

        // ✅ Lấy tất cả thanh toán (có phân trang)
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            if (page <= 0 || pageSize <= 0)
                return BadRequest("page và pageSize phải lớn hơn 0.");

            var list = await _repo.GetAllPagedAsync(page, pageSize);
            var totalCount = await _repo.CountAllAsync();

            return Ok(new
            {
                data = list,
                page,
                pageSize,
                totalCount,
                totalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            });
        }

        // ✅ Cập nhật trạng thái thanh toán theo PaymentId
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdatePaymentStatusRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Status))
                return BadRequest("Status is required.");

            var payment = await _repo.GetByIdAsync(id);
            if (payment == null)
                return NotFound($"Payment with id {id} not found.");

            // Update status
            payment.Status = request.Status;

            // Update new fields if provided
            if (!string.IsNullOrWhiteSpace(request.TransactionCode))
                payment.TransactionCode = request.TransactionCode;

            if (!string.IsNullOrWhiteSpace(request.TxnRef))
                payment.TxnRef = request.TxnRef;

            await _repo.UpdateAsync(payment);

            return Ok(payment);
        }


        // ✅ Cập nhật trạng thái thanh toán theo TxnRef (nếu muốn cập nhật bằng tham chiếu giao dịch)
        //[HttpPut("txnref/{txnRef}/status")]
        //public async Task<IActionResult> UpdateStatusByTxnRef(string txnRef, [FromBody] UpdatePaymentStatusRequest request)
        //{
        //    if (request == null || string.IsNullOrWhiteSpace(request.Status))
        //        return BadRequest("Status is required.");

        //    var payment = await _repo.GetByTxnRefAsync(txnRef);
        //    if (payment == null)
        //        return NotFound($"Payment with txnRef {txnRef} not found.");

        //    payment.Status = request.Status;
        //    await _repo.UpdateAsync(payment);

        //    return Ok(payment);
        //}
    }
}
