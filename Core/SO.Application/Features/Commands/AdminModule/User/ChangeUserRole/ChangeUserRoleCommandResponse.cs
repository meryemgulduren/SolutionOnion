namespace SO.Application.Features.Commands.AdminModule.User.ChangeUserRole
{
    public class ChangeUserRoleCommandResponse
    {
        public bool Succeeded { get; set; }
        public string? Message { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
    }
}
