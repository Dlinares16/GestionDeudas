using System;
using System.Collections.Generic;

namespace GestionDeudas.Model;

public partial class UserBalance
{
    public Guid? UserId { get; set; }

    public string? Email { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public decimal? TotalOwedToMe { get; set; }

    public decimal? TotalIOwe { get; set; }

    public decimal? NetBalance { get; set; }
}
