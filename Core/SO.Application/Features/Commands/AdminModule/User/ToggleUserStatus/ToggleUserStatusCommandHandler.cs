using MediatR;
using Microsoft.AspNetCore.Identity;
using SO.Domain.Entities.Identity;

namespace SO.Application.Features.Commands.AdminModule.User.ToggleUserStatus
{
    public class ToggleUserStatusCommandHandler : IRequestHandler<ToggleUserStatusCommandRequest, ToggleUserStatusCommandResponse>
    {
        private readonly UserManager<AppUser> _userManager;

        public ToggleUserStatusCommandHandler(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<ToggleUserStatusCommandResponse> Handle(ToggleUserStatusCommandRequest request, CancellationToken cancellationToken)
        {
            try
            {
                // Kullanıcıyı bul
                var user = await _userManager.FindByIdAsync(request.UserId);
                if (user == null)
                {
                    return new ToggleUserStatusCommandResponse
                    {
                        Succeeded = false,
                        Message = "Kullanıcı bulunamadı"
                    };
                }

                // Durumu tersine çevir
                user.IsActive = !user.IsActive;
                var result = await _userManager.UpdateAsync(user);
                
                if (result.Succeeded)
                {
                    return new ToggleUserStatusCommandResponse
                    {
                        Succeeded = true,
                        Message = $"Kullanıcı {(user.IsActive ? "aktif" : "pasif")} edildi",
                        IsActive = user.IsActive
                    };
                }
                else
                {
                    return new ToggleUserStatusCommandResponse
                    {
                        Succeeded = false,
                        Message = "Kullanıcı durumu değiştirilemedi",
                        Errors = result.Errors.Select(e => e.Description).ToList()
                    };
                }
            }
            catch (Exception ex)
            {
                return new ToggleUserStatusCommandResponse
                {
                    Succeeded = false,
                    Message = "Hata: " + ex.Message
                };
            }
        }
    }
}
