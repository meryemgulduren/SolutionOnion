namespace SO.Application.Features.Commands.AccountModule.Authentication.RegisterUser
{
    public class RegisterUserCommandResponse
    {
        public bool Succeeded { get; set; }
        public string? RedirectUrl { get; set; }
        public string? ErrorMessage { get; set; }
        public string? UserId { get; set; }
        public string? UserName { get; set; }
        public string? Message { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
    }
}
