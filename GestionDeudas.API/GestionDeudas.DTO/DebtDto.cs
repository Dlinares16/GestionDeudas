using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionDeudas.DTO
{
    public class DebtDto
    {
        public Guid DebtId { get; set; }
        public Guid CreditorId { get; set; }
        public Guid DebtorId { get; set; }
        public decimal Amount { get; set; }
        public string? Description { get; set; }
        public string Status { get; set; } = null!;
        public DateOnly? DueDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Navigation properties
        public UserDto? Creditor { get; set; }
        public UserDto? Debtor { get; set; }
        public List<PaymentDto>? Payments { get; set; }

        // Calculated properties
        public decimal TotalPaid { get; set; }
        public decimal RemainingAmount { get; set; }
        public bool IsOverdue => DueDate.HasValue && DueDate < DateOnly.FromDateTime(DateTime.Now) && Status == "pending";
    }

    public class CreateDebtDto
    {
        [Required]
        public Guid DebtorId { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "El monto debe ser mayor que 0")]
        public decimal Amount { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        public DateOnly? DueDate { get; set; }
    }

    public class UpdateDebtDto
    {
        [Range(0.01, double.MaxValue, ErrorMessage = "El monto debe ser mayor que 0")]
        public decimal? Amount { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        public DateOnly? DueDate { get; set; }

        [RegularExpression("^(pending|paid|cancelled)$")]
        public string? Status { get; set; }
    }

    public class DebtSummaryDto
    {
        public Guid DebtId { get; set; }
        public string CreditorName { get; set; } = null!;
        public string DebtorName { get; set; } = null!;
        public decimal Amount { get; set; }
        public string? Description { get; set; }
        public string Status { get; set; } = null!;
        public DateOnly? DueDate { get; set; }
        public decimal TotalPaid { get; set; }
        public decimal RemainingAmount { get; set; }
        public bool IsOverdue { get; set; }
    }
}
