using GestionDeudas.BLL.Servicios.Contrato;
using GestionDeudas.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GestionDeudas.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IUserService _userService;

        public AuthController(IAuthService authService, IUserService userService)
        {
            _authService = authService;
            _userService = userService;
        }

        private Guid GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            return userIdClaim != null ? Guid.Parse(userIdClaim.Value) : Guid.Empty;
        }

        /// <summary>
        /// Registra un nuevo usuario
        /// </summary>
        [HttpPost("register")]
        public async Task<ActionResult<LoginResponseDto>> Register([FromBody] RegisterDto registerDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var createUserDto = new CreateUserDto
                {
                    Email = registerDto.Email,
                    FirstName = registerDto.FirstName,
                    LastName = registerDto.LastName,
                    PasswordHash = _authService.HashPassword(registerDto.Password)
                };

                var user = await _userService.CreateUserAsync(createUserDto);

                // Generar token de verificación de email
                //var verificationToken = await _authService.GenerateEmailVerificationTokenAsync(user.UserId);

                // Aquí enviarías el email de verificación
                // EmailService.SendVerificationEmail(user.Email, verificationToken);

                // Realizar login automático
                var loginDto = new UserLoginDto
                {
                    Email = registerDto.Email,
                    Password = registerDto.Password
                };

                var loginResponse = await _authService.LoginAsync(loginDto);
                return Ok(loginResponse);
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
        /// Inicia sesión
        /// </summary>
        [HttpPost("login")]
        public async Task<ActionResult<LoginResponseDto>> Login([FromBody] UserLoginDto loginDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var loginResponse = await _authService.LoginAsync(loginDto);
                if (loginResponse == null)
                    return Unauthorized(new { message = "Credenciales inválidas" });

                return Ok(loginResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", details = ex.Message });
            }
        }

        /// <summary>
        /// Renueva el token de acceso usando el refresh token
        /// </summary>
        [HttpPost("refresh-token")]
        public async Task<ActionResult<LoginResponseDto>> RefreshToken([FromBody] RefreshTokenDto refreshTokenDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var loginResponse = await _authService.RefreshTokenAsync(refreshTokenDto);
                if (loginResponse == null)
                    return Unauthorized(new { message = "Refresh token inválido o expirado" });

                return Ok(loginResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", details = ex.Message });
            }
        }

        /// <summary>
        /// Cierra sesión
        /// </summary>
        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout([FromBody] LogoutDto logoutDto)
        {
            try
            {
                var userId = GetCurrentUserId();
                var success = await _authService.LogoutAsync(userId, logoutDto.RefreshToken);

                if (!success)
                    return BadRequest(new { message = "No se pudo cerrar la sesión" });

                return Ok(new { message = "Sesión cerrada exitosamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", details = ex.Message });
            }
        }

        /// <summary>
        /// Cambia la contraseña del usuario actual
        /// </summary>
        [HttpPost("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var userId = GetCurrentUserId();
                var success = await _authService.ChangePasswordAsync(userId, changePasswordDto);

                if (!success)
                    return BadRequest(new { message = "Contraseña actual incorrecta" });

                return Ok(new { message = "Contraseña cambiada exitosamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", details = ex.Message });
            }
        }

        /// <summary>
        /// Solicita el restablecimiento de contraseña
        /// </summary>
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto forgotPasswordDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                await _authService.ForgotPasswordAsync(forgotPasswordDto);
                return Ok(new { message = "Si el email existe, se ha enviado un enlace de restablecimiento" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", details = ex.Message });
            }
        }

        /// <summary>
        /// Restablece la contraseña usando el token
        /// </summary>
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto resetPasswordDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var success = await _authService.ResetPasswordAsync(resetPasswordDto);
                if (!success)
                    return BadRequest(new { message = "Token inválido o expirado" });

                return Ok(new { message = "Contraseña restablecida exitosamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", details = ex.Message });
            }
        }

        /// <summary>
        /// Verifica el email del usuario
        /// </summary>
        [HttpPost("verify-email")]
        public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailDto verifyEmailDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var success = await _authService.VerifyEmailAsync(verifyEmailDto);
                if (!success)
                    return BadRequest(new { message = "Token de verificación inválido o expirado" });

                return Ok(new { message = "Email verificado exitosamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", details = ex.Message });
            }
        }

        /// <summary>
        /// Reenvía el email de verificación
        /// </summary>
        [HttpPost("resend-verification")]
        [Authorize]
        public async Task<IActionResult> ResendVerificationEmail()
        {
            try
            {
                var userId = GetCurrentUserId();
                var verificationToken = await _authService.GenerateEmailVerificationTokenAsync(userId);

                // Aquí enviarías el email de verificación
                // var user = await _userService.GetUserByIdAsync(userId);
                // EmailService.SendVerificationEmail(user.Email, verificationToken);

                return Ok(new { message = "Email de verificación enviado" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", details = ex.Message });
            }
        }

        /// <summary>
        /// Valida un token JWT
        /// </summary>
        [HttpPost("validate-token")]
        public async Task<ActionResult<bool>> ValidateToken([FromBody] ValidateTokenDto validateTokenDto)
        {
            try
            {
                var isValid = await _authService.ValidateTokenAsync(validateTokenDto.Token);
                return Ok(new { isValid });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", details = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene información del usuario actual
        /// </summary>
        [HttpGet("me")]
        [Authorize]
        public async Task<ActionResult<UserDto>> GetCurrentUser()
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
    }

    // DTOs adicionales para Auth
    public class RegisterDto
    {
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class LogoutDto
    {
        public string RefreshToken { get; set; } = string.Empty;
    }

    public class ValidateTokenDto
    {
        public string Token { get; set; } = string.Empty;
    }
}
