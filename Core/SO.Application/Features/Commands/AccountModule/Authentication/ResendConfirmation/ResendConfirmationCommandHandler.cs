using MediatR;
using Microsoft.AspNetCore.Identity;
using SO.Domain.Entities.Identity;
using SO.Application.Abstractions.Services;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SO.Application.Features.Commands.AccountModule.Authentication.ResendConfirmation
{
    public class ResendConfirmationCommandHandler : IRequestHandler<ResendConfirmationCommandRequest, ResendConfirmationCommandResponse>
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IEmailService _emailService;

        public ResendConfirmationCommandHandler(UserManager<AppUser> userManager, IEmailService emailService)
        {
            _userManager = userManager;
            _emailService = emailService;
        }

        public async Task<ResendConfirmationCommandResponse> Handle(ResendConfirmationCommandRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(request.Email);
                if (user == null)
                {
                    // Güvenlik için kullanıcıyı ifşa etmiyoruz
                    return new ResendConfirmationCommandResponse
                    {
                        Succeeded = true,
                        Message = "Eğer kayıtlı iseniz doğrulama e-postası gönderildi.",
                        UserNotFound = true
                    };
                }

                if (await _userManager.IsEmailConfirmedAsync(user))
                {
                    return new ResendConfirmationCommandResponse
                    {
                        Succeeded = true,
                        Message = "E-posta zaten doğrulanmış.",
                        EmailAlreadyConfirmed = true
                    };
                }

                // Email doğrulama token'ı oluştur
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var tokenBytes = Encoding.UTF8.GetBytes(token);
                var tokenEncoded = WebEncoders.Base64UrlEncode(tokenBytes);
                
                // Callback URL oluştur
                var callbackUrl = $"{request.BaseUrl}/Account/ConfirmEmail?userId={user.Id}&token={tokenEncoded}";
                
                // Email gönder
                await _emailService.SendAsync(
                    user.Email!, 
                    "E-posta Doğrulama", 
                    $"Lütfen e-postanızı doğrulamak için <a href='{callbackUrl}'>buraya tıklayın</a>."
                );

                return new ResendConfirmationCommandResponse
                {
                    Succeeded = true,
                    Message = "Doğrulama e-postası gönderildi."
                };
            }
            catch (Exception ex)
            {
                return new ResendConfirmationCommandResponse
                {
                    Succeeded = false,
                    ErrorMessage = $"E-posta gönderilirken hata oluştu: {ex.Message}"
                };
            }
        }
    }
}

