namespace SO.Application.Features.Commands.AccountModule.Authentication.ForgotPassword
{
    public class ForgotPasswordCommandResponse
    {
        public bool Succeeded { get; set; }
        public string? Message { get; set; }
        public string? ErrorMessage { get; set; }
        public bool UserNotFound { get; set; }
    }
}
