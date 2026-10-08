using System.ComponentModel.DataAnnotations;

namespace payment_service.Models
{
    public class CreatePaymentRequest
    {
        [Required]
        public Guid PatientId { get; set; }

        [Required]
        public decimal Amount { get; set; }

        public string? Notes { get; set; }
        public string? TransactionCode { get; set; }
        public string? TxnRef { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? PaidAt { get; set; }
    }
}
