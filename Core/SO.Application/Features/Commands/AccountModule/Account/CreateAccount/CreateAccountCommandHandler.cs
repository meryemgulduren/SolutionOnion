using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using SO.Application.Abstractions.Services.AccountModule;
using SO.Application.DTOs.AccountModule.Account;
using System.Threading;


namespace SO.Application.Features.Commands.AccountModule.Account.CreateAccount
{
    // Bu sınıf, CreateAccountCommandRequest isteğini alıp işleyecek olan asıl mantığı içerir.
    public class CreateAccountCommandHandler : IRequestHandler<CreateAccountCommandRequest, CreateAccountCommandResponse>
    {
        private readonly IAccountService _accountService;

        public CreateAccountCommandHandler(IAccountService accountService)
        {
            _accountService = accountService;
        }

        public async Task<CreateAccountCommandResponse> Handle(CreateAccountCommandRequest request, CancellationToken cancellationToken)
        {
            // DÜZELTME: DTO'nun tam yolunu belirtiyoruz.
            await _accountService.CreateAccountAsync(new SO.Application.DTOs.AccountModule.Account.CreateAccount
            {
                CompanyName = request.CompanyName,
                ContactPerson = request.ContactPerson,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber
            });
            return new();
        }
    }
}
