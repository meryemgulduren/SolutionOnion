using MediatR;
using Microsoft.AspNetCore.Identity;
using SO.Domain.Entities.Identity;
using SO.Application.Abstractions.Services;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SO.Application.Features.Commands.AccountModule.Authentication.ForgotPassword
{
    public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommandRequest, ForgotPasswordCommandResponse>
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IEmailService _emailService;

        public ForgotPasswordCommandHandler(UserManager<AppUser> userManager, IEmailService emailService)
        {
            _userManager = userManager;
            _emailService = emailService;
        }

        public async Task<ForgotPasswordCommandResponse> Handle(ForgotPasswordCommandRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(request.Email);
                if (user == null)
                {
                    // Güvenlik için kullanıcıyı ifşa etmiyoruz
                    return new ForgotPasswordCommandResponse
                    {
                        Succeeded = true,
                        Message = "If you have an account with us, you will receive a password reset email.",
                        UserNotFound = true
                    };
                }

                // Password reset token oluştur
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var tokenBytes = Encoding.UTF8.GetBytes(token);
                var tokenEncoded = WebEncoders.Base64UrlEncode(tokenBytes);
                
                // Callback URL oluştur
                var callbackUrl = $"{request.BaseUrl}/Account/ResetPassword?email={user.Email}&token={tokenEncoded}";
                
                // Email gönder
                await _emailService.SendAsync(
                    user.Email!, 
                    "Şifre Sıfırlama", 
                    $"Şifrenizi sıfırlamak için <a href='{callbackUrl}'>buraya tıklayın</a>."
                );

                return new ForgotPasswordCommandResponse
                {
                    Succeeded = true,
                    Message = "Password reset email has been sent."
                };
            }
            catch (Exception ex)
            {
                return new ForgotPasswordCommandResponse
                {
                    Succeeded = false,
                    ErrorMessage = $"Password reset email failed: {ex.Message}"
                };
            }
        }
    }
}
