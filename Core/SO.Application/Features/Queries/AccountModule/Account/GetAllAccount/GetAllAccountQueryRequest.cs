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
        // Tüm müşterileri listeleyeceğimiz için bu isteğin şimdilik bir parametreye ihtiyacı yok.
    }
}
