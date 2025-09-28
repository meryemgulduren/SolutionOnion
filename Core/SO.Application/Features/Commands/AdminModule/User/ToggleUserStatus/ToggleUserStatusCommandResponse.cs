namespace SO.Application.Features.Commands.AdminModule.User.ToggleUserStatus
{
    public class ToggleUserStatusCommandResponse
    {
        public bool Succeeded { get; set; }
        public string? Message { get; set; }
        public bool IsActive { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
    }
}
