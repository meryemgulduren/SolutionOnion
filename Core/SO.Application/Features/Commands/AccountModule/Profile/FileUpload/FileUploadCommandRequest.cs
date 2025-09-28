using MediatR;
using Microsoft.AspNetCore.Http;

namespace SO.Application.Features.Commands.AccountModule.Profile.FileUpload
{
    public class FileUploadCommandRequest : IRequest<FileUploadCommandResponse>
    {
        public IFormFile? File { get; set; }
        public string ContainerName { get; set; } = string.Empty;
    }
}
