using System;
using System.Collections.Generic;

namespace GestionDeudas.Model;

public partial class Payment
{
    public Guid PaymentId { get; set; }

    public Guid DebtId { get; set; }

    public decimal Amount { get; set; }

    public string? PaymentMethod { get; set; }

    public string? Notes { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Debt Debt { get; set; } = null!;
}
