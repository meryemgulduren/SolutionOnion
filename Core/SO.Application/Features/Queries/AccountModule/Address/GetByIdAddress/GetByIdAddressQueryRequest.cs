using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SO.Application.Features.Queries.AccountModule.Address.GetByIdAddress
{
    public class GetByIdAddressQueryRequest : IRequest<GetByIdAddressQueryResponse>
    {
        public string Id { get; set; }
    }
}
