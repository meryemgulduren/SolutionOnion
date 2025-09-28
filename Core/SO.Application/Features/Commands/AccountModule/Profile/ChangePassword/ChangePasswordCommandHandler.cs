using MediatR;
using Microsoft.AspNetCore.Identity;
using SO.Domain.Entities.Identity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SO.Application.Features.Commands.AccountModule.Profile.ChangePassword
{
    public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommandRequest, ChangePasswordCommandResponse>
    {
        private readonly UserManager<AppUser> _userManager;

        public ChangePasswordCommandHandler(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<ChangePasswordCommandResponse> Handle(ChangePasswordCommandRequest request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user == null)
            {
                return new ChangePasswordCommandResponse
                {
                    Succeeded = false,
                    ErrorMessage = "Kullanıcı bulunamadı."
                };
            }

            var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
            if (result.Succeeded)
            {
                return new ChangePasswordCommandResponse { Succeeded = true };
            }

            return new ChangePasswordCommandResponse
            {
                Succeeded = false,
                ErrorMessage = "Şifre değiştirilirken hata oluştu.",
                Errors = result.Errors.Select(e => e.Description).ToList()
            };
        }
    }
}
