namespace SO.Application.Features.Commands.AdminModule.User.DeleteUser
{
    public class DeleteUserCommandResponse
    {
        public bool Succeeded { get; set; }
        public string? Message { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
    }
}
