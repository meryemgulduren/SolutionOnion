using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MediatR;

namespace SO.Application.Features.Queries.AccountModule.Account.GetByIdAccount
{
    public class GetByIdAccountQueryRequest : IRequest<GetByIdAccountQueryResponse>
    {
        // Hangi müşteriyi istediğimizi belirtmek için Id özelliğini ekliyoruz.
        public string Id { get; set; }
    }
}