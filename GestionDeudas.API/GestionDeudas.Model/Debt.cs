using System;
using System.Collections.Generic;

namespace GestionDeudas.Model;

public partial class Debt
{
    public Guid DebtId { get; set; }

    public Guid CreditorId { get; set; }

    public Guid DebtorId { get; set; }

    public decimal Amount { get; set; }

    public string? Description { get; set; }

    public string? Status { get; set; }

    public DateOnly? DueDate { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual User Creditor { get; set; } = null!;

    public virtual User Debtor { get; set; } = null!;

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
