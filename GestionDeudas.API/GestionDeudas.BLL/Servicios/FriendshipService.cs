using AutoMapper;
using GestionDeudas.BLL.Servicios.Contrato;
using GestionDeudas.DAL.Repositorios.Contrato;
using GestionDeudas.DTO;
using GestionDeudas.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionDeudas.BLL.Servicios
{
    public class FriendshipService : IFriendshipService
    {
        private readonly IGenericRepository<Friendship> _friendshipRepository;
        private readonly IGenericRepository<User> _userRepository;
        private readonly IMapper _mapper;
        private readonly DbContext _context;

        public FriendshipService(IGenericRepository<Friendship> friendshipRepository, IGenericRepository<User> userRepository, IMapper mapper, DbContext context)
        {
            _friendshipRepository = friendshipRepository;
            _userRepository = userRepository;
            _mapper = mapper;
            _context = context;
        }

        public async Task<IEnumerable<FriendDto>> GetUserFriendsAsync(Guid userId)
        {
            var sql = @"
                SELECT 
                    user_id as UserId,
                    friend_id as FriendId,
                    user_email as UserEmail,
                    user_first_name as UserFirstName,
                    user_last_name as UserLastName,
                    friend_email as FriendEmail,
                    friend_first_name as FriendFirstName,
                    friend_last_name as FriendLastName,
                    friends_since as FriendsSince
                FROM accepted_friends 
                WHERE user_id = {0}
                ORDER BY friend_first_name, friend_last_name";

            var friends = await _context.Database.SqlQueryRaw<FriendDto>(sql, userId).ToListAsync();
            return friends;
        }

        public async Task<IEnumerable<FriendshipRequestDto>> GetPendingFriendshipRequestsAsync(Guid userId)
        {
            var friendships = await (await _friendshipRepository.Consultar(f => f.AddresseeId == userId && f.Status == "pending"))
                .Include(f => f.Requester)
                .ToListAsync();

            return _mapper.Map<IEnumerable<FriendshipRequestDto>>(friendships);
        }

        public async Task<IEnumerable<FriendshipRequestDto>> GetSentFriendshipRequestsAsync(Guid userId)
        {
            var friendships = await (await _friendshipRepository.Consultar(f => f.RequesterId == userId && f.Status == "pending"))
                .Include(f => f.Addressee)
                .ToListAsync();

            return friendships.Select(f => new FriendshipRequestDto
            {
                FriendshipId = f.FriendshipId,
                RequesterId = f.AddresseeId, // Invertir para mostrar a quien se envió
                RequesterName = f.Addressee != null ? $"{f.Addressee.FirstName} {f.Addressee.LastName}" : "",
                RequesterEmail = f.Addressee?.Email ?? "",
                CreatedAt = f.CreatedAt
            });
        }

        public async Task<FriendshipDto> SendFriendshipRequestAsync(Guid requesterId, CreateFriendshipDto createFriendshipDto)
        {
            if (requesterId == createFriendshipDto.AddresseeId)
                throw new ArgumentException("No puedes enviarte una solicitud de amistad a ti mismo");

            var addresseeExists = await _userRepository.Obtener(u => u.UserId == createFriendshipDto.AddresseeId && u.IsActive == true) != null;
            if (!addresseeExists)
                throw new ArgumentException("El usuario destinatario no existe o está inactivo");

            var existingFriendship = await _friendshipRepository.Obtener(f =>
                (f.RequesterId == requesterId && f.AddresseeId == createFriendshipDto.AddresseeId) ||
                (f.RequesterId == createFriendshipDto.AddresseeId && f.AddresseeId == requesterId));

            if (existingFriendship != null)
            {
                if (existingFriendship.Status == "accepted")
                    throw new ArgumentException("Ya son amigos");
                if (existingFriendship.Status == "pending")
                    throw new ArgumentException("Ya existe una solicitud de amistad pendiente");
                if (existingFriendship.Status == "blocked")
                    throw new ArgumentException("No se puede enviar solicitud a este usuario");
            }

            var friendship = _mapper.Map<Friendship>(createFriendshipDto);
            friendship.FriendshipId = Guid.NewGuid();
            friendship.RequesterId = requesterId;
            friendship.CreatedAt = DateTime.UtcNow;
            friendship.UpdatedAt = DateTime.UtcNow;

            var createdFriendship = await _friendshipRepository.Crear(friendship);

            var result = await (await _friendshipRepository.Consultar(f => f.FriendshipId == createdFriendship.FriendshipId))
                .Include(f => f.Requester)
                .Include(f => f.Addressee)
                .FirstOrDefaultAsync();

            return _mapper.Map<FriendshipDto>(result);
        }

        public async Task<bool> AcceptFriendshipRequestAsync(Guid userId, Guid friendshipId)
        {
            var friendship = await _friendshipRepository.Obtener(f => f.FriendshipId == friendshipId);
            if (friendship == null || friendship.AddresseeId != userId || friendship.Status != "pending")
                return false;

            friendship.Status = "accepted";
            friendship.UpdatedAt = DateTime.UtcNow;

            return await _friendshipRepository.Editar(friendship);
        }

        public async Task<bool> RejectFriendshipRequestAsync(Guid userId, Guid friendshipId)
        {
            var friendship = await _friendshipRepository.Obtener(f => f.FriendshipId == friendshipId);
            if (friendship == null || friendship.AddresseeId != userId || friendship.Status != "pending")
                return false;

            return await _friendshipRepository.Eliminar(friendship);
        }

        public async Task<bool> BlockUserAsync(Guid userId, Guid friendshipId)
        {
            var friendship = await _friendshipRepository.Obtener(f => f.FriendshipId == friendshipId);
            if (friendship == null || friendship.AddresseeId != userId)
                return false;

            friendship.Status = "blocked";
            friendship.UpdatedAt = DateTime.UtcNow;

            return await _friendshipRepository.Editar(friendship);
        }

        public async Task<bool> RemoveFriendAsync(Guid userId, Guid friendId)
        {
            var friendship = await _friendshipRepository.Obtener(f =>
                ((f.RequesterId == userId && f.AddresseeId == friendId) ||
                 (f.RequesterId == friendId && f.AddresseeId == userId)) &&
                f.Status == "accepted");

            if (friendship == null) return false;

            return await _friendshipRepository.Eliminar(friendship);
        }

        public async Task<bool> AreFriendsAsync(Guid user1Id, Guid user2Id)
        {
            var friendship = await _friendshipRepository.Obtener(f =>
                ((f.RequesterId == user1Id && f.AddresseeId == user2Id) ||
                 (f.RequesterId == user2Id && f.AddresseeId == user1Id)) &&
                f.Status == "accepted");

            return friendship != null;
        }

        public async Task<bool> HasPendingRequestAsync(Guid requesterId, Guid addresseeId)
        {
            var friendship = await _friendshipRepository.Obtener(f =>
                f.RequesterId == requesterId && f.AddresseeId == addresseeId && f.Status == "pending");

            return friendship != null;
        }

        public async Task<FriendshipDto?> GetFriendshipAsync(Guid user1Id, Guid user2Id)
        {
            var friendship = await (await _friendshipRepository.Consultar(f =>
                (f.RequesterId == user1Id && f.AddresseeId == user2Id) ||
                (f.RequesterId == user2Id && f.AddresseeId == user1Id)))
                .Include(f => f.Requester)
                .Include(f => f.Addressee)
                .FirstOrDefaultAsync();

            return friendship != null ? _mapper.Map<FriendshipDto>(friendship) : null;
        }
    }
}
