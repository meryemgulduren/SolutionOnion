using MediatR;
using Microsoft.AspNetCore.Identity;
using SO.Domain.Entities.Identity;

namespace SO.Application.Features.Commands.AdminModule.User.ChangeUserRole
{
    public class ChangeUserRoleCommandHandler : IRequestHandler<ChangeUserRoleCommandRequest, ChangeUserRoleCommandResponse>
    {
        private readonly UserManager<AppUser> _userManager;

        public ChangeUserRoleCommandHandler(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<ChangeUserRoleCommandResponse> Handle(ChangeUserRoleCommandRequest request, CancellationToken cancellationToken)
        {
            try
            {
                // Kullanıcıyı bul
                var user = await _userManager.FindByIdAsync(request.UserId);
                if (user == null)
                {
                    return new ChangeUserRoleCommandResponse
                    {
                        Succeeded = false,
                        Message = "Kullanıcı bulunamadı"
                    };
                }

                // Mevcut rolleri kaldır
                var currentRoles = await _userManager.GetRolesAsync(user);
                await _userManager.RemoveFromRolesAsync(user, currentRoles);

                // Yeni rolü ata
                var result = await _userManager.AddToRoleAsync(user, request.NewRole);
                
                if (result.Succeeded)
                {
                    return new ChangeUserRoleCommandResponse
                    {
                        Succeeded = true,
                        Message = $"Kullanıcı rolü başarıyla '{request.NewRole}' olarak değiştirildi"
                    };
                }
                else
                {
                    return new ChangeUserRoleCommandResponse
                    {
                        Succeeded = false,
                        Message = "Rol değiştirilemedi",
                        Errors = result.Errors.Select(e => e.Description).ToList()
                    };
                }
            }
            catch (Exception ex)
            {
                return new ChangeUserRoleCommandResponse
                {
                    Succeeded = false,
                    Message = "Hata: " + ex.Message
                };
            }
        }
    }
}
