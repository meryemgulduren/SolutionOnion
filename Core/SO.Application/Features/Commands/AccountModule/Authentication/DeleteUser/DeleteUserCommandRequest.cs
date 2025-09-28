using MediatR;

namespace SO.Application.Features.Commands.AccountModule.Authentication.DeleteUser
{
    public class DeleteUserCommandRequest : IRequest<DeleteUserCommandResponse>
    {
        public string UserId { get; set; } = string.Empty;
        public string? CurrentPassword { get; set; }
    }
}


