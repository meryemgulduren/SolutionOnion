using MediatR;

namespace SO.Application.Features.Commands.AdminModule.User.ChangeUserRole
{
    public class ChangeUserRoleCommandRequest : IRequest<ChangeUserRoleCommandResponse>
    {
        public string UserId { get; set; } = string.Empty;
        public string NewRole { get; set; } = string.Empty;
    }
}
