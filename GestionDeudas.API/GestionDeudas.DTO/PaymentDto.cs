using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionDeudas.DTO
{
    public class PaymentDto
    {
        public Guid PaymentId { get; set; }
        public Guid DebtId { get; set; }
        public decimal Amount { get; set; }
        public string? PaymentMethod { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation property
        public DebtDto? Debt { get; set; }
    }

    public class CreatePaymentDto
    {
        [Required]
        public Guid DebtId { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "El monto debe ser mayor que 0")]
        public decimal Amount { get; set; }

        [MaxLength(50)]
        public string? PaymentMethod { get; set; }

        [MaxLength(500)]
        public string? Notes { get; set; }
    }

    public class PaymentSummaryDto
    {
        public Guid PaymentId { get; set; }
        public decimal Amount { get; set; }
        public string? PaymentMethod { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
        public string DebtDescription { get; set; } = null!;
        public string CreditorName { get; set; } = null!;
        public string DebtorName { get; set; } = null!;
    }
}
