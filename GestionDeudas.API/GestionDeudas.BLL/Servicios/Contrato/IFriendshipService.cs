using GestionDeudas.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionDeudas.BLL.Servicios.Contrato
{
    public interface IFriendshipService
    {
        Task<IEnumerable<FriendDto>> GetUserFriendsAsync(Guid userId);
        Task<IEnumerable<FriendshipRequestDto>> GetPendingFriendshipRequestsAsync(Guid userId);
        Task<IEnumerable<FriendshipRequestDto>> GetSentFriendshipRequestsAsync(Guid userId);
        Task<FriendshipDto> SendFriendshipRequestAsync(Guid requesterId, CreateFriendshipDto createFriendshipDto);
        Task<bool> AcceptFriendshipRequestAsync(Guid userId, Guid friendshipId);
        Task<bool> RejectFriendshipRequestAsync(Guid userId, Guid friendshipId);
        Task<bool> BlockUserAsync(Guid userId, Guid friendshipId);
        Task<bool> RemoveFriendAsync(Guid userId, Guid friendId);
        Task<bool> AreFriendsAsync(Guid user1Id, Guid user2Id);
        Task<bool> HasPendingRequestAsync(Guid requesterId, Guid addresseeId);
        Task<FriendshipDto?> GetFriendshipAsync(Guid user1Id, Guid user2Id);
    }
}
