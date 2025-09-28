namespace SO.Application.Features.Commands.AccountModule.Profile.EditProfile
{
    public class EditProfileCommandResponse
    {
        public bool Succeeded { get; set; }
        public string? ErrorMessage { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
    }
}
