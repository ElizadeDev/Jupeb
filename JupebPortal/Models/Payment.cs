using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JupebPortal.Models
{
    public class Payment
    {
        public int Id { get; set; }

        public string UserId { get; set; }
        [Display(Name = "RRR")]
        public string? PaymentRef { get; set; }
        [Display(Name = "Transaction ID")]
        [Required]
        public string TransactionID { get; set; }
        [Required]

        [Column(TypeName = "decimal(18,2)")]
        [Range(0.01, double.MaxValue)]

        public decimal Amount { get; set; }
        public string? Purpose { get; set; }
        [Required]
        [Display(Name = "Success Status")]
        public bool IsSuccess { get; set; } = false;
        [Display(Name = "Payment Date")]
        [Required]
        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
        [Display(Name = "Payment Channel")]
        public string PaymentChannel { get; set; } = "Remita";
    }
}
