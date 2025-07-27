using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MediatR;
using SO.Application.Abstractions.Services.AccountModule;
using System.Threading;


namespace SO.Application.Features.Queries.AccountModule.Account.GetByIdAccount
{
    public class GetByIdAccountQueryHandler : IRequestHandler<GetByIdAccountQueryRequest, GetByIdAccountQueryResponse>
    {
        private readonly IAccountService _accountService;

        public GetByIdAccountQueryHandler(IAccountService accountService)
        {
            _accountService = accountService;
        }

        public async Task<GetByIdAccountQueryResponse> Handle(GetByIdAccountQueryRequest request, CancellationToken cancellationToken)
        {
            // Servis katmanını çağırarak ID'ye göre müşteriyi getiriyoruz.
            var account = await _accountService.GetAccountByIdAsync(request.Id);

            // Dönen sonucu Response nesnesine atayarak geri döndürüyoruz.
            return new()
            {
                Result = account
            };
        }
    }
}

