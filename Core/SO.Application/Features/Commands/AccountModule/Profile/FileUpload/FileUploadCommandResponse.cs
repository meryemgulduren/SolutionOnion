namespace SO.Application.Features.Commands.AccountModule.Profile.FileUpload
{
    public class FileUploadCommandResponse
    {
        public bool Succeeded { get; set; }
        public string? FilePath { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
