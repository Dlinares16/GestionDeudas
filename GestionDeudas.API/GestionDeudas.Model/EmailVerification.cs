using System;
using System.Collections.Generic;

namespace GestionDeudas.Model;

public partial class EmailVerification
{
    public Guid VerificationId { get; set; }

    public Guid UserId { get; set; }

    public string VerificationToken { get; set; } = null!;

    public DateTime ExpiresAt { get; set; }

    public bool? IsUsed { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual User User { get; set; } = null!;
}
