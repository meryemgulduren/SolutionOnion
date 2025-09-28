using MediatR;
using Microsoft.AspNetCore.Identity;
using SO.Domain.Entities.Identity;
using SO.Application.Abstractions.Services;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SO.Application.Features.Commands.AccountModule.Authentication.RegisterUser
{
    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommandRequest, RegisterUserCommandResponse>
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly IEmailService _emailService;

        public RegisterUserCommandHandler(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            RoleManager<AppRole> roleManager,
            IEmailService emailService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _emailService = emailService;
        }

        public async Task<RegisterUserCommandResponse> Handle(RegisterUserCommandRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var user = new AppUser
                {
                    UserName = request.UserName,
                    Email = request.Email,
                    FullName = request.FullName,
                    CreatedDate = DateTime.UtcNow,
                    IsActive = true
                };

                var result = await _userManager.CreateAsync(user, request.Password);

                if (result.Succeeded)
                {
                    // Varsayılan olarak User rolünü ata
                    await _userManager.AddToRoleAsync(user, "User");

                    // Email doğrulama token'ı oluştur
                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

                    // Email doğrulama linki oluştur
                    var confirmationLink = $"{request.BaseUrl}/Account/ConfirmEmail?userId={user.Id}&token={encodedToken}";

                    // Email gönder
                    var emailSubject = "Email Adresinizi Doğrulayın";
                    var emailBody = $@"
                        <h2>Hoş Geldiniz!</h2>
                        <p>Hesabınızı aktifleştirmek için aşağıdaki linke tıklayın:</p>
                        <p><a href='{confirmationLink}' style='background-color: #1976d2; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px;'>Email Adresimi Doğrula</a></p>
                        <p>Bu link 24 saat geçerlidir.</p>
                        <p>Eğer bu işlemi siz yapmadıysanız, bu emaili görmezden gelebilirsiniz.</p>";

                    await _emailService.SendAsync(user.Email, emailSubject, emailBody);

                    return new RegisterUserCommandResponse
                    {
                        Succeeded = true,
                        RedirectUrl = "/Account/EmailConfirmationSent",
                        UserId = user.Id,
                        UserName = user.UserName,
                        Message = "Kayıt başarılı! Lütfen email adresinizi kontrol edin ve doğrulama linkine tıklayın."
                    };
                }

                // Hataları topla
                var errors = result.Errors.Select(e => e.Description).ToList();

                return new RegisterUserCommandResponse
                {
                    Succeeded = false,
                    ErrorMessage = "Kullanıcı kaydı başarısız.",
                    Errors = errors
                };
            }
            catch (Exception ex)
            {
                return new RegisterUserCommandResponse
                {
                    Succeeded = false,
                    ErrorMessage = $"Kayıt sırasında hata oluştu: {ex.Message}"
                };
            }
        }
    }
}
