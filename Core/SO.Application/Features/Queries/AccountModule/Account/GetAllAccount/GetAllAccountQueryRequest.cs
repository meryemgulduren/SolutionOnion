using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;



namespace SO.Application.Features.Queries.AccountModule.Account.GetAllAccount
{
    public class GetAllAccountQueryRequest : IRequest<GetAllAccountQueryResponse>
    {
        public string? CurrentUserId { get; set; }
        public bool IsAdmin { get; set; } = false;
    }
}
