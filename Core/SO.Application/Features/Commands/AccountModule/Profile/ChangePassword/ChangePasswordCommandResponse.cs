namespace SO.Application.Features.Commands.AccountModule.Profile.ChangePassword
{
    public class ChangePasswordCommandResponse
    {
        public bool Succeeded { get; set; }
        public string? ErrorMessage { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
    }
}
