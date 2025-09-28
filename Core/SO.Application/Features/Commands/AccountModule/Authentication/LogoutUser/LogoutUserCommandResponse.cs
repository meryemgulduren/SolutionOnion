namespace SO.Application.Features.Commands.AccountModule.Authentication.LogoutUser
{
    public class LogoutUserCommandResponse
    {
        public bool Succeeded { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
