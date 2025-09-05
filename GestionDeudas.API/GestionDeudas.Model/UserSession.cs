using System;
using System.Collections.Generic;

namespace GestionDeudas.Model;

public partial class UserSession
{
    public Guid SessionId { get; set; }

    public Guid UserId { get; set; }

    public string RefreshToken { get; set; } = null!;

    public DateTime ExpiresAt { get; set; }

    public DateTime? CreatedAt { get; set; }

    public bool? IsActive { get; set; }

    public virtual User User { get; set; } = null!;
}
