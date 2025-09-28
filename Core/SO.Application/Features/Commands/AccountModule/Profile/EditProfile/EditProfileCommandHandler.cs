using MediatR;
using Microsoft.AspNetCore.Identity;
using SO.Domain.Entities.Identity;
using System.Threading;
using System.Threading.Tasks;

namespace SO.Application.Features.Commands.AccountModule.Profile.EditProfile
{
    public class EditProfileCommandHandler : IRequestHandler<EditProfileCommandRequest, EditProfileCommandResponse>
    {
        private readonly UserManager<AppUser> _userManager;

        public EditProfileCommandHandler(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<EditProfileCommandResponse> Handle(EditProfileCommandRequest request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user == null)
            {
                return new EditProfileCommandResponse
                {
                    Succeeded = false,
                    ErrorMessage = "Kullanıcı bulunamadı."
                };
            }

            // E-posta değişikliği kontrolü
            if (user.Email != request.Email)
            {
                var emailExists = await _userManager.FindByEmailAsync(request.Email);
                if (emailExists != null && emailExists.Id != user.Id)
                {
                    return new EditProfileCommandResponse
                    {
                        Succeeded = false,
                        ErrorMessage = "Bu e-posta adresi başka bir kullanıcı tarafından kullanılıyor."
                    };
                }
                user.Email = request.Email;
                user.EmailConfirmed = false; // E-posta değişirse doğrulama gerekebilir
            }

            // Kullanıcı adı değişikliği kontrolü
            if (user.UserName != request.UserName)
            {
                var usernameExists = await _userManager.FindByNameAsync(request.UserName);
                if (usernameExists != null && usernameExists.Id != user.Id)
                {
                    return new EditProfileCommandResponse
                    {
                        Succeeded = false,
                        ErrorMessage = "Bu kullanıcı adı başka bir kullanıcı tarafından kullanılıyor."
                    };
                }
                user.UserName = request.UserName;
            }

            user.FullName = request.FullName;
            user.ProfilePicture = request.ProfilePicture;

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                return new EditProfileCommandResponse { Succeeded = true };
            }

            return new EditProfileCommandResponse
            {
                Succeeded = false,
                ErrorMessage = "Profil güncellenirken hata oluştu.",
                Errors = result.Errors.Select(e => e.Description).ToList()
            };
        }
    }
}
