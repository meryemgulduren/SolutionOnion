using MediatR;

namespace SO.Application.Features.Commands.AdminModule.User.ToggleUserStatus
{
    public class ToggleUserStatusCommandRequest : IRequest<ToggleUserStatusCommandResponse>
    {
        public string UserId { get; set; } = string.Empty;
    }
}
