using MediatR;
using Microsoft.AspNetCore.Identity;
using SO.Domain.Entities.Identity;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SO.Application.Features.Commands.AccountModule.Authentication.ResetPassword
{
    public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommandRequest, ResetPasswordCommandResponse>
    {
        private readonly UserManager<AppUser> _userManager;

        public ResetPasswordCommandHandler(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<ResetPasswordCommandResponse> Handle(ResetPasswordCommandRequest request, CancellationToken cancellationToken)
        {
            try
            {
                // Email ve Token kontrolü
                if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Token))
                {
                    return new ResetPasswordCommandResponse
                    {
                        Succeeded = false,
                        ErrorMessage = "Email and token are required."
                    };
                }

                // Kullanıcıyı bul
                var user = await _userManager.FindByEmailAsync(request.Email);
                if (user == null)
                {
                    return new ResetPasswordCommandResponse
                    {
                        Succeeded = false,
                        ErrorMessage = "User not found.",
                        UserNotFound = true
                    };
                }

                // Token'ı decode et
                var decodedBytes = WebEncoders.Base64UrlDecode(request.Token);
                var decodedToken = Encoding.UTF8.GetString(decodedBytes);

                // Şifreyi sıfırla
                var result = await _userManager.ResetPasswordAsync(user, decodedToken, request.Password);
                
                if (result.Succeeded)
                {
                    return new ResetPasswordCommandResponse
                    {
                        Succeeded = true,
                        Message = "Password has been reset successfully."
                    };
                }
                else
                {
                    var errors = result.Errors.Select(e => e.Description).ToList();
                    return new ResetPasswordCommandResponse
                    {
                        Succeeded = false,
                        ErrorMessage = "Password reset failed.",
                        InvalidToken = errors.Any(e => e.Contains("token") || e.Contains("invalid")),
                        Errors = errors
                    };
                }
            }
            catch (Exception ex)
            {
                return new ResetPasswordCommandResponse
                {
                    Succeeded = false,
                    ErrorMessage = $"Password reset failed: {ex.Message}"
                };
            }
        }
    }
}
