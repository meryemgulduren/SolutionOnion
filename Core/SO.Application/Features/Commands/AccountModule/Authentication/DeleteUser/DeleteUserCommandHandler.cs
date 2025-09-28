using MediatR;
using Microsoft.AspNetCore.Identity;
using SO.Domain.Entities.Identity;
using System.Threading;
using System.Threading.Tasks;

namespace SO.Application.Features.Commands.AccountModule.Authentication.DeleteUser
{
    public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommandRequest, DeleteUserCommandResponse>
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public DeleteUserCommandHandler(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<DeleteUserCommandResponse> Handle(DeleteUserCommandRequest request, CancellationToken cancellationToken)
        {
            // Debug için log ekleyelim
            System.Console.WriteLine($"DeleteUserCommandHandler - Request UserId: {request.UserId}");
            
            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user == null)
            {
                // ID ile bulunamazsa, kullanıcı adı ile deneyelim
                System.Console.WriteLine($"DeleteUserCommandHandler - User not found by ID, trying by username: {request.UserId}");
                user = await _userManager.FindByNameAsync(request.UserId);
                
                if (user == null)
                {
                    System.Console.WriteLine($"DeleteUserCommandHandler - User not found by username either: {request.UserId}");
                    return new DeleteUserCommandResponse { Succeeded = false, ErrorMessage = "Kullanıcı bulunamadı." };
                }
            }
            
            System.Console.WriteLine($"DeleteUserCommandHandler - User found: {user.UserName} ({user.Email})");

            // İsteğe bağlı: mevcut şifre doğrulaması
            if (!string.IsNullOrEmpty(request.CurrentPassword))
            {
                var passwordValid = await _userManager.CheckPasswordAsync(user, request.CurrentPassword);
                if (!passwordValid)
                {
                    return new DeleteUserCommandResponse { Succeeded = false, ErrorMessage = "Şifre yanlış." };
                }
            }

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                return new DeleteUserCommandResponse { Succeeded = false, ErrorMessage = "Hesap silme başarısız." };
            }

            await _signInManager.SignOutAsync();
            return new DeleteUserCommandResponse { Succeeded = true, RedirectUrl = "/Home/Index" };
        }
    }
}


