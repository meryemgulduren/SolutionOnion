using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MediatR;
using SO.Application.Abstractions.Services.AccountModule;
using SO.Application.DTOs.AccountModule.Account;
using System.Threading;
using System.Threading.Tasks;

namespace SO.Application.Features.Commands.AccountModule.Account.UpdateAccount
{
    // Bu sınıf, UpdateAccountCommandRequest isteğini alıp işleyecek olan asıl mantığı içerir.
    public class UpdateAccountCommandHandler : IRequestHandler<UpdateAccountCommandRequest, UpdateAccountCommandResponse>
    {
        private readonly IAccountService _accountService;

        public UpdateAccountCommandHandler(IAccountService accountService)
        {
            _accountService = accountService;
        }

        public async Task<UpdateAccountCommandResponse> Handle(UpdateAccountCommandRequest request, CancellationToken cancellationToken)
        {
            // DÜZELTME: DTO'nun tam yolunu belirtiyoruz.
            await _accountService.UpdateAccountAsync(new SO.Application.DTOs.AccountModule.Account.UpdateAccount
            {
                Id = request.Id,
                CompanyName = request.CompanyName,
                ContactPerson = request.ContactPerson,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                TaxOffice = request.TaxOffice,
                TaxNumber = request.TaxNumber,
                IsActive = request.IsActive
            });
            return new();
        }
    }
}

