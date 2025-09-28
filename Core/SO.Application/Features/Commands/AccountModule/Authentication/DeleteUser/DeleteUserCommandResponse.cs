namespace SO.Application.Features.Commands.AccountModule.Authentication.DeleteUser
{
    public class DeleteUserCommandResponse
    {
        public bool Succeeded { get; set; }
        public string? ErrorMessage { get; set; }
        public string? RedirectUrl { get; set; }
    }
}


