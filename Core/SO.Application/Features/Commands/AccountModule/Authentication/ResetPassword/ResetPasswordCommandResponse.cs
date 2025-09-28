namespace SO.Application.Features.Commands.AccountModule.Authentication.ResetPassword
{
    public class ResetPasswordCommandResponse
    {
        public bool Succeeded { get; set; }
        public string? Message { get; set; }
        public string? ErrorMessage { get; set; }
        public bool UserNotFound { get; set; }
        public bool InvalidToken { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
    }
}
