using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using SO.Application.Abstractions.Services.AccountModule;
using System.Threading;



namespace SO.Application.Features.Queries.AccountModule.Account.GetAllAccount
{
    public class GetAllAccountQueryHandler : IRequestHandler<GetAllAccountQueryRequest, GetAllAccountQueryResponse>
    {
        private readonly IAccountService _accountService;

        public GetAllAccountQueryHandler(IAccountService accountService)
        {
            _accountService = accountService;
        }

        public async Task<GetAllAccountQueryResponse> Handle(GetAllAccountQueryRequest request, CancellationToken cancellationToken)
        {
            // Servis katmanını çağırarak tüm müşterileri getiriyoruz.
            var accounts = await _accountService.GetAllAccountsAsync();

            // Dönen sonucu Response nesnesine atayarak geri döndürüyoruz.
            return new()
            {
                Result = accounts
            };
        }
    }
}