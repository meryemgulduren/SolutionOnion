namespace SO.Application.Features.Commands.AdminModule.Export.ExportAllProposalsWord
{
    public class ExportAllProposalsWordCommandResponse
    {
        public bool Succeeded { get; set; }
        public byte[]? FileBytes { get; set; }
        public string? FileName { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
