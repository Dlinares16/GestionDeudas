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
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        private Guid GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            return userIdClaim != null ? Guid.Parse(userIdClaim.Value) : Guid.Empty;
        }

        /// <summary>
        /// Obtiene todos los usuarios activos
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetAllUsers()
        {
            try
            {
                var users = await _userService.GetAllUsersAsync();
                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", details = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene un usuario por su ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetUserById(Guid id)
        {
            try
            {
                var user = await _userService.GetUserByIdAsync(id);
                if (user == null)
                    return NotFound(new { message = "Usuario no encontrado" });

                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", details = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene el perfil del usuario actual
        /// </summary>
        [HttpGet("profile")]
        public async Task<ActionResult<UserDto>> GetMyProfile()
        {
            try
            {
                var userId = GetCurrentUserId();
                var user = await _userService.GetUserByIdAsync(userId);

                if (user == null)
                    return NotFound(new { message = "Usuario no encontrado" });

                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", details = ex.Message });
            }
        }

        /// <summary>
        /// Crea un nuevo usuario
        /// </summary>
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<UserDto>> CreateUser([FromBody] CreateUserDto createUserDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var user = await _userService.CreateUserAsync(createUserDto);
                return CreatedAtAction(nameof(GetUserById), new { id = user.UserId }, user);
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
        /// Actualiza el perfil del usuario actual
        /// </summary>
        [HttpPut("profile")]
        public async Task<ActionResult<UserDto>> UpdateMyProfile([FromBody] UpdateUserDto updateUserDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var userId = GetCurrentUserId();
                var updatedUser = await _userService.UpdateUserAsync(userId, updateUserDto);

                if (updatedUser == null)
                    return NotFound(new { message = "Usuario no encontrado" });

                return Ok(updatedUser);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", details = ex.Message });
            }
        }

        /// <summary>
        /// Desactiva la cuenta del usuario actual
        /// </summary>
        [HttpPatch("profile/deactivate")]
        public async Task<IActionResult> DeactivateMyAccount()
        {
            try
            {
                var userId = GetCurrentUserId();
                var success = await _userService.DeactivateUserAsync(userId);

                if (!success)
                    return NotFound(new { message = "Usuario no encontrado" });

                return Ok(new { message = "Cuenta desactivada exitosamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", details = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene el balance del usuario actual
        /// </summary>
        [HttpGet("balance")]
        public async Task<ActionResult<UserBalanceDto>> GetMyBalance()
        {
            try
            {
                var userId = GetCurrentUserId();
                var balance = await _userService.GetUserBalanceAsync(userId);

                if (balance == null)
                    return NotFound(new { message = "Balance no encontrado" });

                return Ok(balance);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", details = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene todos los balances de usuarios (solo para administradores)
        /// </summary>
        [HttpGet("balances")]
        public async Task<ActionResult<IEnumerable<UserBalanceDto>>> GetAllBalances()
        {
            try
            {
                var balances = await _userService.GetAllUserBalancesAsync();
                return Ok(balances);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", details = ex.Message });
            }
        }

        /// <summary>
        /// Verifica el email del usuario actual
        /// </summary>
        [HttpPatch("verify-email")]
        public async Task<IActionResult> VerifyEmail()
        {
            try
            {
                var userId = GetCurrentUserId();
                var success = await _userService.VerifyEmailAsync(userId);

                if (!success)
                    return NotFound(new { message = "Usuario no encontrado" });

                return Ok(new { message = "Email verificado exitosamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", details = ex.Message });
            }
        }
    }
}
