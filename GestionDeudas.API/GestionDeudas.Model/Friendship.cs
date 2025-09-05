using System;
using System.Collections.Generic;

namespace GestionDeudas.Model;

public partial class Friendship
{
    public Guid FriendshipId { get; set; }

    public Guid RequesterId { get; set; }

    public Guid AddresseeId { get; set; }

    public string? Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual User Addressee { get; set; } = null!;

    public virtual User Requester { get; set; } = null!;
}
