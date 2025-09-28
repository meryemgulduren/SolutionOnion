using MediatR;
using Microsoft.AspNetCore.Identity;
using SO.Domain.Entities.Identity;

namespace SO.Application.Features.Commands.AdminModule.User.DeleteUser
{
    public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommandRequest, DeleteUserCommandResponse>
    {
        private readonly UserManager<AppUser> _userManager;

        public DeleteUserCommandHandler(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<DeleteUserCommandResponse> Handle(DeleteUserCommandRequest request, CancellationToken cancellationToken)
        {
            try
            {
                // Kullanıcıyı bul
                var user = await _userManager.FindByIdAsync(request.UserId);
                if (user == null)
                {
                    return new DeleteUserCommandResponse
                    {
                        Succeeded = false,
                        Message = "Kullanıcı bulunamadı"
                    };
                }

                // Kendi kendini silmeye çalışıyorsa engelle
                if (request.CurrentUserId == request.UserId)
                {
                    return new DeleteUserCommandResponse
                    {
                        Succeeded = false,
                        Message = "Kendi hesabınızı silemezsiniz"
                    };
                }

                // SuperAdmin rolündeki kullanıcıları silmeye çalışıyorsa kontrol et
                var userRoles = await _userManager.GetRolesAsync(user);
                if (userRoles.Contains("SuperAdmin"))
                {
                    // En az bir SuperAdmin kalması gerekiyor
                    var superAdminCount = (await _userManager.GetUsersInRoleAsync("SuperAdmin")).Count;
                    if (superAdminCount <= 1)
                    {
                        return new DeleteUserCommandResponse
                        {
                            Succeeded = false,
                            Message = "Sistemde en az bir SuperAdmin bulunmalıdır"
                        };
                    }
                }

                // Kullanıcıyı sil
                var result = await _userManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    return new DeleteUserCommandResponse
                    {
                        Succeeded = true,
                        Message = "Kullanıcı başarıyla silindi"
                    };
                }
                else
                {
                    return new DeleteUserCommandResponse
                    {
                        Succeeded = false,
                        Message = "Kullanıcı silinemedi",
                        Errors = result.Errors.Select(e => e.Description).ToList()
                    };
                }
            }
            catch (Exception ex)
            {
                return new DeleteUserCommandResponse
                {
                    Succeeded = false,
                    Message = "Hata: " + ex.Message
                };
            }
        }
    }
}
