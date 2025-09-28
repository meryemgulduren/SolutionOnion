namespace SO.Application.Features.Commands.AdminModule.Export.ExportProposalWord
{
    public class ExportProposalWordCommandResponse
    {
        public bool Succeeded { get; set; }
        public byte[]? FileBytes { get; set; }
        public string? FileName { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
