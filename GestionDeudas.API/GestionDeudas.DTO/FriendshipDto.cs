using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionDeudas.DTO
{
    public class FriendshipDto
    {
        public Guid FriendshipId { get; set; }
        public Guid RequesterId { get; set; }
        public Guid AddresseeId { get; set; }
        public string Status { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Navigation properties
        public UserDto? Requester { get; set; }
        public UserDto? Addressee { get; set; }
    }

    public class CreateFriendshipDto
    {
        [Required]
        public Guid AddresseeId { get; set; }
    }

    public class UpdateFriendshipDto
    {
        [Required]
        [RegularExpression("^(pending|accepted|blocked)$")]
        public string Status { get; set; } = null!;
    }

    public class FriendDto
    {
        public Guid UserId { get; set; }
        public Guid FriendId { get; set; }
        public string UserEmail { get; set; } = null!;
        public string UserFirstName { get; set; } = null!;
        public string UserLastName { get; set; } = null!;
        public string FriendEmail { get; set; } = null!;
        public string FriendFirstName { get; set; } = null!;
        public string FriendLastName { get; set; } = null!;
        public DateTime FriendsSince { get; set; }
        public string FriendFullName => $"{FriendFirstName} {FriendLastName}";
    }

    public class FriendshipRequestDto
    {
        public Guid FriendshipId { get; set; }
        public Guid RequesterId { get; set; }
        public string RequesterName { get; set; } = null!;
        public string RequesterEmail { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
    }
}
