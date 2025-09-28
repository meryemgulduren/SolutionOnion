using MediatR;
using Microsoft.AspNetCore.Identity;
using SO.Domain.Entities.Identity;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SO.Application.Features.Commands.AccountModule.Authentication.LoginUser
{
    public class LoginUserCommandHandler : IRequestHandler<LoginUserCommandRequest, LoginUserCommandResponse>
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;

        public LoginUserCommandHandler(
            SignInManager<AppUser> signInManager,
            UserManager<AppUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        public async Task<LoginUserCommandResponse> Handle(LoginUserCommandRequest request, CancellationToken cancellationToken)
        {
            try
            {
                // Email ile kullanıcıyı bulup kullanıcı adı üzerinden giriş yap
                var userForLogin = await _userManager.FindByEmailAsync(request.Email);
                if (userForLogin == null)
                {
                    return new LoginUserCommandResponse
                    {
                        Succeeded = false,
                        ErrorMessage = "Invalid email or password."
                    };
                }

                // Email doğrulama kontrolü
                if (!await _userManager.IsEmailConfirmedAsync(userForLogin))
                {
                    return new LoginUserCommandResponse
                    {
                        Succeeded = false,
                        ErrorMessage = "Lütfen önce email adresinizi doğrulayın. Email'inizi kontrol edin veya yeni doğrulama linki isteyin."
                    };
                }

                var result = await _signInManager.PasswordSignInAsync(
                    userForLogin.UserName,
                    request.Password,
                    request.RememberMe,
                    lockoutOnFailure: true);

                if (result.Succeeded)
                {
                    // Son giriş tarihini güncelle
                    var user = userForLogin;
                    if (user != null)
                    {
                        user.LastLoginDate = DateTime.UtcNow;
                        await _userManager.UpdateAsync(user);
                    }

                    // Redirect URL belirle
                    string redirectUrl = "/Home/Index";
                    if (!string.IsNullOrEmpty(request.ReturnUrl))
                    {
                        redirectUrl = request.ReturnUrl;
                    }

                    return new LoginUserCommandResponse
                    {
                        Succeeded = true,
                        RedirectUrl = !string.IsNullOrEmpty(redirectUrl) ? redirectUrl : "/Home/Dashboard",
                        UserId = user?.Id,
                        UserName = user?.UserName
                    };
                }

                if (result.IsLockedOut)
                {
                    return new LoginUserCommandResponse
                    {
                        Succeeded = false,
                        IsLockedOut = true,
                        ErrorMessage = "Your account is locked. Please try again later."
                    };
                }

                return new LoginUserCommandResponse
                {
                    Succeeded = false,
                    ErrorMessage = "Invalid login attempt."
                };
            }
            catch (Exception ex)
            {
                return new LoginUserCommandResponse
                {
                    Succeeded = false,
                    ErrorMessage = $"An error occurred during login: {ex.Message}"
                };
            }
        }
    }
}
