using GestionDeudas.BLL.Servicios.Contrato;
using GestionDeudas.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GestionDeudas.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class FriendshipsController : ControllerBase
    {
        private readonly IFriendshipService _friendshipService;

        public FriendshipsController(IFriendshipService friendshipService)
        {
            _friendshipService = friendshipService;
        }

        private Guid GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            return userIdClaim != null ? Guid.Parse(userIdClaim.Value) : Guid.Empty;
        }

        /// <summary>
        /// Obtiene la lista de amigos del usuario actual
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FriendDto>>> GetMyFriends()
        {
            try
            {
                var userId = GetCurrentUserId();
                var friends = await _friendshipService.GetUserFriendsAsync(userId);
                return Ok(friends);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", details = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene las solicitudes de amistad pendientes recibidas
        /// </summary>
        [HttpGet("requests/pending")]
        public async Task<ActionResult<IEnumerable<FriendshipRequestDto>>> GetPendingRequests()
        {
            try
            {
                var userId = GetCurrentUserId();
                var requests = await _friendshipService.GetPendingFriendshipRequestsAsync(userId);
                return Ok(requests);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", details = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene las solicitudes de amistad enviadas
        /// </summary>
        [HttpGet("requests/sent")]
        public async Task<ActionResult<IEnumerable<FriendshipRequestDto>>> GetSentRequests()
        {
            try
            {
                var userId = GetCurrentUserId();
                var requests = await _friendshipService.GetSentFriendshipRequestsAsync(userId);
                return Ok(requests);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", details = ex.Message });
            }
        }

        /// <summary>
        /// Envía una solicitud de amistad
        /// </summary>
        [HttpPost("requests")]
        public async Task<ActionResult<FriendshipDto>> SendFriendshipRequest([FromBody] CreateFriendshipDto createFriendshipDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var requesterId = GetCurrentUserId();
                var friendship = await _friendshipService.SendFriendshipRequestAsync(requesterId, createFriendshipDto);

                return CreatedAtAction(nameof(GetFriendship), new { userId = createFriendshipDto.AddresseeId }, friendship);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", details = ex.Message });
            }
        }

        /// <summary>
        /// Acepta una solicitud de amistad
        /// </summary>
        [HttpPatch("requests/{friendshipId}/accept")]
        public async Task<IActionResult> AcceptFriendshipRequest(Guid friendshipId)
        {
            try
            {
                var userId = GetCurrentUserId();
                var success = await _friendshipService.AcceptFriendshipRequestAsync(userId, friendshipId);

                if (!success)
                    return BadRequest(new { message = "No se pudo aceptar la solicitud" });

                return Ok(new { message = "Solicitud de amistad aceptada" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", details = ex.Message });
            }
        }

        /// <summary>
        /// Rechaza una solicitud de amistad
        /// </summary>
        [HttpPatch("requests/{friendshipId}/reject")]
        public async Task<IActionResult> RejectFriendshipRequest(Guid friendshipId)
        {
            try
            {
                var userId = GetCurrentUserId();
                var success = await _friendshipService.RejectFriendshipRequestAsync(userId, friendshipId);

                if (!success)
                    return BadRequest(new { message = "No se pudo rechazar la solicitud" });

                return Ok(new { message = "Solicitud de amistad rechazada" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", details = ex.Message });
            }
        }

        /// <summary>
        /// Bloquea un usuario
        /// </summary>
        [HttpPatch("requests/{friendshipId}/block")]
        public async Task<IActionResult> BlockUser(Guid friendshipId)
        {
            try
            {
                var userId = GetCurrentUserId();
                var success = await _friendshipService.BlockUserAsync(userId, friendshipId);

                if (!success)
                    return BadRequest(new { message = "No se pudo bloquear al usuario" });

                return Ok(new { message = "Usuario bloqueado exitosamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", details = ex.Message });
            }
        }

        /// <summary>
        /// Elimina un amigo
        /// </summary>
        [HttpDelete("{friendId}")]
        public async Task<IActionResult> RemoveFriend(Guid friendId)
        {
            try
            {
                var userId = GetCurrentUserId();
                var success = await _friendshipService.RemoveFriendAsync(userId, friendId);

                if (!success)
                    return NotFound(new { message = "Amistad no encontrada" });

                return Ok(new { message = "Amigo eliminado exitosamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", details = ex.Message });
            }
        }

        /// <summary>
        /// Verifica si dos usuarios son amigos
        /// </summary>
        [HttpGet("check/{userId}")]
        public async Task<ActionResult<bool>> AreFriends(Guid userId)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                var areFriends = await _friendshipService.AreFriendsAsync(currentUserId, userId);
                return Ok(new { areFriends });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", details = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene la información de amistad entre dos usuarios
        /// </summary>
        [HttpGet("friendship/{userId}")]
        public async Task<ActionResult<FriendshipDto>> GetFriendship(Guid userId)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                var friendship = await _friendshipService.GetFriendshipAsync(currentUserId, userId);

                if (friendship == null)
                    return NotFound(new { message = "No existe relación de amistad" });

                return Ok(friendship);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", details = ex.Message });
            }
        }

        /// <summary>
        /// Verifica si existe una solicitud pendiente
        /// </summary>
        [HttpGet("check-pending/{userId}")]
        public async Task<ActionResult<bool>> HasPendingRequest(Guid userId)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                var hasPending = await _friendshipService.HasPendingRequestAsync(currentUserId, userId);
                return Ok(new { hasPendingRequest = hasPending });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", details = ex.Message });
            }
        }
    }
}
