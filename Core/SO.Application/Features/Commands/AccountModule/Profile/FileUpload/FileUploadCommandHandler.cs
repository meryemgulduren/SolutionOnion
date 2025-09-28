using MediatR;
using SO.Application.Abstractions.Storage;
using Microsoft.AspNetCore.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SO.Application.Features.Commands.AccountModule.Profile.FileUpload
{
    public class FileUploadCommandHandler : IRequestHandler<FileUploadCommandRequest, FileUploadCommandResponse>
    {
        private readonly IStorageService _storageService;

        public FileUploadCommandHandler(IStorageService storageService)
        {
            _storageService = storageService;
        }

        public async Task<FileUploadCommandResponse> Handle(FileUploadCommandRequest request, CancellationToken cancellationToken)
        {
            if (request.File == null)
            {
                return new FileUploadCommandResponse
                {
                    Succeeded = false,
                    ErrorMessage = "Dosya bulunamadı."
                };
            }

            try
            {
                var fileCollection = new FormFileCollection { request.File };
                var uploadResult = await _storageService.UploadAsync(request.ContainerName, fileCollection);
                
                if (uploadResult.Any())
                {
                    return new FileUploadCommandResponse
                    {
                        Succeeded = true,
                        FilePath = uploadResult.First().pathOrContainerName
                    };
                }
                else
                {
                    return new FileUploadCommandResponse
                    {
                        Succeeded = false,
                        ErrorMessage = "Dosya yüklenirken hata oluştu."
                    };
                }
            }
            catch (Exception ex)
            {
                return new FileUploadCommandResponse
                {
                    Succeeded = false,
                    ErrorMessage = $"Dosya yükleme hatası: {ex.Message}"
                };
            }
        }
    }
}
