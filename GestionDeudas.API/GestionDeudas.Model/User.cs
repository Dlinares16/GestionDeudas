using System;
using System.Collections.Generic;

namespace GestionDeudas.Model;

public partial class User
{
    public Guid UserId { get; set; }

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string? Phone { get; set; }

    public bool? IsActive { get; set; }

    public bool? EmailVerified { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Debt> DebtCreditors { get; set; } = new List<Debt>();

    public virtual ICollection<Debt> DebtDebtors { get; set; } = new List<Debt>();

    public virtual ICollection<EmailVerification> EmailVerifications { get; set; } = new List<EmailVerification>();

    public virtual ICollection<Friendship> FriendshipAddressees { get; set; } = new List<Friendship>();

    public virtual ICollection<Friendship> FriendshipRequesters { get; set; } = new List<Friendship>();

    public virtual ICollection<UserSession> UserSessions { get; set; } = new List<UserSession>();
}
