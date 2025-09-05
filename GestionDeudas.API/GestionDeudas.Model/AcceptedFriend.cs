using System;
using System.Collections.Generic;

namespace GestionDeudas.Model;

public partial class AcceptedFriend
{
    public Guid? UserId { get; set; }

    public Guid? FriendId { get; set; }

    public string? UserEmail { get; set; }

    public string? UserFirstName { get; set; }

    public string? UserLastName { get; set; }

    public string? FriendEmail { get; set; }

    public string? FriendFirstName { get; set; }

    public string? FriendLastName { get; set; }

    public DateTime? FriendsSince { get; set; }
}
