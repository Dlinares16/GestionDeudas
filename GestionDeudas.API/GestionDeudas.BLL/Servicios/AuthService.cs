using AutoMapper;
using GestionDeudas.BLL.Servicios.Contrato;
using GestionDeudas.DAL.Repositorios.Contrato;
using GestionDeudas.DTO;
using GestionDeudas.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Crypto.Generators;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace GestionDeudas.BLL.Servicios
{
    public class AuthService : IAuthService
    {
        private readonly IGenericRepository<User> _userRepository;
        private readonly IGenericRepository<UserSession> _sessionRepository;
        private readonly IGenericRepository<EmailVerification> _verificationRepository;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public AuthService(
            IGenericRepository<User> userRepository,
            IGenericRepository<UserSession> sessionRepository,
            IGenericRepository<EmailVerification> verificationRepository,
            IMapper mapper,
            IConfiguration configuration)
        {
            _userRepository = userRepository;
            _sessionRepository = sessionRepository;
            _verificationRepository = verificationRepository;
            _mapper = mapper;
            _configuration = configuration;
        }

        public async Task<LoginResponseDto?> LoginAsync(UserLoginDto loginDto)
        {
            var user = await _userRepository.Obtener(u => u.Email == loginDto.Email && u.IsActive == true);
            if (user == null || !VerifyPassword(loginDto.Password, user.PasswordHash))
                return null;

            var accessToken = GenerateAccessToken(user);
            var refreshToken = GenerateRefreshToken();
            var expiresAt = DateTime.UtcNow.AddDays(7);

            var session = new UserSession
            {
                SessionId = Guid.NewGuid(),
                UserId = user.UserId,
                RefreshToken = refreshToken,
                ExpiresAt = expiresAt,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            await _sessionRepository.Crear(session);

            return new LoginResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresAt = expiresAt,
                User = _mapper.Map<UserDto>(user)
            };
        }

        public async Task<LoginResponseDto?> RefreshTokenAsync(RefreshTokenDto refreshTokenDto)
        {
            var session = await _sessionRepository.Obtener(s =>
                s.RefreshToken == refreshTokenDto.RefreshToken &&
                s.IsActive == true &&
                s.ExpiresAt > DateTime.UtcNow);

            if (session == null) return null;

            var user = await _userRepository.Obtener(u => u.UserId == session.UserId && u.IsActive == true);
            if (user == null) return null;

            // Invalidar la sesión anterior
            session.IsActive = false;
            await _sessionRepository.Editar(session);

            // Crear nueva sesión
            var newAccessToken = GenerateAccessToken(user);
            var newRefreshToken = GenerateRefreshToken();
            var newExpiresAt = DateTime.UtcNow.AddDays(7);

            var newSession = new UserSession
            {
                SessionId = Guid.NewGuid(),
                UserId = user.UserId,
                RefreshToken = newRefreshToken,
                ExpiresAt = newExpiresAt,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            await _sessionRepository.Crear(newSession);

            return new LoginResponseDto
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
                ExpiresAt = newExpiresAt,
                User = _mapper.Map<UserDto>(user)
            };
        }

        public async Task<bool> LogoutAsync(Guid userId, string refreshToken)
        {
            var session = await _sessionRepository.Obtener(s =>
                s.UserId == userId &&
                s.RefreshToken == refreshToken &&
                s.IsActive == true);

            if (session == null) return false;

            session.IsActive = false;
            return await _sessionRepository.Editar(session);
        }

        public async Task<bool> ChangePasswordAsync(Guid userId, ChangePasswordDto changePasswordDto)
        {
            var user = await _userRepository.Obtener(u => u.UserId == userId && u.IsActive == true);
            if (user == null || !VerifyPassword(changePasswordDto.CurrentPassword, user.PasswordHash))
                return false;

            user.PasswordHash = HashPassword(changePasswordDto.NewPassword);
            user.UpdatedAt = DateTime.UtcNow;

            // Invalidar todas las sesiones del usuario
            var sessions = await (await _sessionRepository.Consultar(s => s.UserId == userId && s.IsActive == true))
                .ToListAsync();

            foreach (var session in sessions)
            {
                session.IsActive = false;
                await _sessionRepository.Editar(session);
            }

            return await _userRepository.Editar(user);
        }

        public async Task<bool> ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto)
        {
            var user = await _userRepository.Obtener(u => u.Email == forgotPasswordDto.Email && u.IsActive == true);
            if (user == null) return true; // Por seguridad, siempre devolver true

            var token = GeneratePasswordResetToken();
            var verification = new EmailVerification
            {
                VerificationId = Guid.NewGuid(),
                UserId = user.UserId,
                VerificationToken = token,
                ExpiresAt = DateTime.UtcNow.AddHours(1),
                IsUsed = false,
                CreatedAt = DateTime.UtcNow
            };

            await _verificationRepository.Crear(verification);

            // Aquí enviarías el email con el token
            // EmailService.SendPasswordResetEmail(user.Email, token);

            return true;
        }

        public async Task<bool> ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
        {
            var verification = await _verificationRepository.Obtener(v =>
                v.VerificationToken == resetPasswordDto.Token &&
                !v.IsUsed == true &&
                v.ExpiresAt > DateTime.UtcNow);

            if (verification == null) return false;

            var user = await _userRepository.Obtener(u =>
                u.UserId == verification.UserId &&
                u.Email == resetPasswordDto.Email &&
                u.IsActive == true);

            if (user == null) return false;

            user.PasswordHash = HashPassword(resetPasswordDto.NewPassword);
            user.UpdatedAt = DateTime.UtcNow;

            verification.IsUsed = true;

            // Invalidar todas las sesiones del usuario
            var sessions = await (await _sessionRepository.Consultar(s => s.UserId == user.UserId && s.IsActive == true))
                .ToListAsync();

            foreach (var session in sessions)
            {
                session.IsActive = false;
                await _sessionRepository.Editar(session);
            }

            await _userRepository.Editar(user);
            await _verificationRepository.Editar(verification);

            return true;
        }

        public async Task<string> GenerateEmailVerificationTokenAsync(Guid userId)
        {
            var token = GenerateVerificationToken();
            var verification = new EmailVerification
            {
                VerificationId = Guid.NewGuid(),
                UserId = userId,
                VerificationToken = token,
                ExpiresAt = DateTime.UtcNow.AddDays(1),
                IsUsed = false,
                CreatedAt = DateTime.UtcNow
            };

            await _verificationRepository.Crear(verification);
            return token;
        }

        public async Task<bool> VerifyEmailAsync(VerifyEmailDto verifyEmailDto)
        {
            var verification = await _verificationRepository.Obtener(v =>
                v.VerificationToken == verifyEmailDto.Token &&
                v.UserId == verifyEmailDto.UserId &&
                !v.IsUsed == true &&
                v.ExpiresAt > DateTime.UtcNow);

            if (verification == null) return false;

            var user = await _userRepository.Obtener(u => u.UserId == verifyEmailDto.UserId);
            if (user == null) return false;

            user.EmailVerified = true;
            user.UpdatedAt = DateTime.UtcNow;

            verification.IsUsed = true;

            await _userRepository.Editar(user);
            await _verificationRepository.Editar(verification);

            return true;
        }

        public async Task<bool> ValidateTokenAsync(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);

                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _configuration["Jwt:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = _configuration["Jwt:Audience"],
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password, BCrypt.Net.BCrypt.GenerateSalt(12));
        }

        public bool VerifyPassword(string password, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }

        private string GenerateAccessToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new(ClaimTypes.Email, user.Email),
                new(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
                new("email_verified", user.EmailVerified.ToString())
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(int.Parse(_configuration["Jwt:AccessTokenExpirationMinutes"])),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        private string GeneratePasswordResetToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber).Replace("/", "_").Replace("+", "-");
        }

        private string GenerateVerificationToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber).Replace("/", "_").Replace("+", "-");
        }
    }
}
