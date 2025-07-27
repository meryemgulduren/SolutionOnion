using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MediatR;
using SO.Application.Abstractions.Services.AccountModule;
using System.Threading;
using System.Threading.Tasks;

namespace SO.Application.Features.Commands.AccountModule.Account.DeleteAccount
{
    // Bu sınıf, DeleteAccountCommandRequest isteğini alıp işleyecek olan asıl mantığı içerir.
    public class DeleteAccountCommandHandler : IRequestHandler<DeleteAccountCommandRequest, DeleteAccountCommandResponse>
    {
        private readonly IAccountService _accountService;

        public DeleteAccountCommandHandler(IAccountService accountService)
        {
            _accountService = accountService;
        }

        public async Task<DeleteAccountCommandResponse> Handle(DeleteAccountCommandRequest request, CancellationToken cancellationToken)
        {
            // Servis katmanını çağırarak ID'ye göre müşteriyi siliyoruz.
            await _accountService.DeleteAccountAsync(request.Id);

            // İşlem başarılı olduğunda boş bir response döndürüyoruz.
            return new();
        }
    }
}
