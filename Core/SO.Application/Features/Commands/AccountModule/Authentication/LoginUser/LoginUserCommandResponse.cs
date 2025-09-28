namespace SO.Application.Features.Commands.AccountModule.Authentication.LoginUser
{
    public class LoginUserCommandResponse
    {
        public bool Succeeded { get; set; }
        public string? RedirectUrl { get; set; }
        public string? ErrorMessage { get; set; }
        public bool IsLockedOut { get; set; }
        public string? UserId { get; set; }
        public string? UserName { get; set; }
    }
}
