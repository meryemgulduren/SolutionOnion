namespace SO.Application.Features.Commands.AccountModule.Authentication.ResendConfirmation
{
    public class ResendConfirmationCommandResponse
    {
        public bool Succeeded { get; set; }
        public string? Message { get; set; }
        public string? ErrorMessage { get; set; }
        public bool EmailAlreadyConfirmed { get; set; }
        public bool UserNotFound { get; set; }
    }
}

