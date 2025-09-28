namespace SO.Application.Features.Commands.AccountModule.Authentication.ConfirmEmail
{
    public class ConfirmEmailCommandResponse
    {
        public bool Succeeded { get; set; }
        public string? Message { get; set; }
        public string? ErrorMessage { get; set; }
        public bool UserNotFound { get; set; }
        public bool InvalidToken { get; set; }
    }
}
