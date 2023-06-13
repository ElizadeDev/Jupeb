namespace JupebPortal.Models
{
    public class RemitaStatusResponse
    {
        public string paymentReference { get; set; }
        public decimal amount { get; set; }
        public string paymentState { get; set; }
        public DateTime paymentDate { get; set; }
        public string processorId { get; set; }
        public string transactionId { get; set; }
        public bool tokenized { get; set; }
        public string paymentToken { get; set; }
        public string cardType { get; set; }
        public decimal debitedAmount { get; set; }
        public string message { get; set; }
        public string paymentChannel { get; set; }
        public string customerId { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string phoneNumber { get; set; }
        public string email { get; set; }
        public string narration { get; set; }
    }

}
