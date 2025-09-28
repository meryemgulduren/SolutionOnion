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
            var allAccounts = await _accountService.GetAllAccountsAsync();

            // Admin ise tüm account'ları göster
            if (request.IsAdmin)
            {
                return new() { Result = allAccounts };
            }

            // User ise sadece kendi oluşturduğu account'ları göster
            if (!string.IsNullOrEmpty(request.CurrentUserId))
            {
                var filteredAccounts = allAccounts?.Where(a => 
                    a.CreatedById == request.CurrentUserId).ToList();
                return new() { Result = filteredAccounts ?? new List<SO.Application.DTOs.AccountModule.Account.ListAccount>() };
            }
            
            // Kullanıcı ID yoksa boş liste döndür
            return new() { Result = new List<SO.Application.DTOs.AccountModule.Account.ListAccount>() };
        }
    }
}