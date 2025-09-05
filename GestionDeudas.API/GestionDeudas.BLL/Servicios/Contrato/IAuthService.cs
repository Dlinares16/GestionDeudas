using GestionDeudas.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionDeudas.BLL.Servicios.Contrato
{
    public interface IAuthService
    {
        Task<LoginResponseDto?> LoginAsync(UserLoginDto loginDto);
        Task<LoginResponseDto?> RefreshTokenAsync(RefreshTokenDto refreshTokenDto);
        Task<bool> LogoutAsync(Guid userId, string refreshToken);
        Task<bool> ChangePasswordAsync(Guid userId, ChangePasswordDto changePasswordDto);
        Task<bool> ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto);
        Task<bool> ResetPasswordAsync(ResetPasswordDto resetPasswordDto);
        Task<string> GenerateEmailVerificationTokenAsync(Guid userId);
        Task<bool> VerifyEmailAsync(VerifyEmailDto verifyEmailDto);
        Task<bool> ValidateTokenAsync(string token);
        string HashPassword(string password);
        bool VerifyPassword(string password, string hashedPassword);
    }
}
