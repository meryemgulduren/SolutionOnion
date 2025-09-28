using MediatR;
using Microsoft.AspNetCore.Identity;
using SO.Domain.Entities.Identity;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SO.Application.Features.Commands.AccountModule.Authentication.ConfirmEmail
{
    public class ConfirmEmailCommandHandler : IRequestHandler<ConfirmEmailCommandRequest, ConfirmEmailCommandResponse>
    {
        private readonly UserManager<AppUser> _userManager;

        public ConfirmEmailCommandHandler(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<ConfirmEmailCommandResponse> Handle(ConfirmEmailCommandRequest request, CancellationToken cancellationToken)
        {
            try
            {
                // User ID ve Token kontrolü
                if (string.IsNullOrEmpty(request.UserId) || string.IsNullOrEmpty(request.Token))
                {
                    return new ConfirmEmailCommandResponse
                    {
                        Succeeded = false,
                        ErrorMessage = "User ID and token are required."
                    };
                }

                // Kullanıcıyı bul
                var user = await _userManager.FindByIdAsync(request.UserId);
                if (user == null)
                {
                    return new ConfirmEmailCommandResponse
                    {
                        Succeeded = false,
                        ErrorMessage = "User not found.",
                        UserNotFound = true
                    };
                }

                // Token'ı decode et
                var decodedBytes = WebEncoders.Base64UrlDecode(request.Token);
                var decodedToken = Encoding.UTF8.GetString(decodedBytes);

                // Email'i doğrula
                var result = await _userManager.ConfirmEmailAsync(user, decodedToken);
                
                if (result.Succeeded)
                {
                    return new ConfirmEmailCommandResponse
                    {
                        Succeeded = true,
                        Message = "Email confirmed successfully."
                    };
                }
                else
                {
                    return new ConfirmEmailCommandResponse
                    {
                        Succeeded = false,
                        ErrorMessage = "Invalid or expired token.",
                        InvalidToken = true
                    };
                }
            }
            catch (Exception ex)
            {
                return new ConfirmEmailCommandResponse
                {
                    Succeeded = false,
                    ErrorMessage = $"Email confirmation failed: {ex.Message}"
                };
            }
        }
    }
}
