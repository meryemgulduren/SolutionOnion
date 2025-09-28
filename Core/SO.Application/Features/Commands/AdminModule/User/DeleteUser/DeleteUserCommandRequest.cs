using MediatR;

namespace SO.Application.Features.Commands.AdminModule.User.DeleteUser
{
    public class DeleteUserCommandRequest : IRequest<DeleteUserCommandResponse>
    {
        public string UserId { get; set; } = string.Empty;
        public string CurrentUserId { get; set; } = string.Empty;
    }
}
