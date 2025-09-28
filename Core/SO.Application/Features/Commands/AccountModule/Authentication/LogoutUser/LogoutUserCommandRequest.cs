using MediatR;

namespace SO.Application.Features.Commands.AccountModule.Authentication.LogoutUser
{
    public class LogoutUserCommandRequest : IRequest<LogoutUserCommandResponse>
    {
        // Logout için ek parametre gerekmez
    }
}
