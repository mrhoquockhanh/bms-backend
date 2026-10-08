namespace payment_service.Models
{
    public class UpdatePaymentStatusRequest
    {
        public string Status { get; set; }

        public string? TransactionCode { get; set; }
        public string? TxnRef { get; set; }
    }
}
