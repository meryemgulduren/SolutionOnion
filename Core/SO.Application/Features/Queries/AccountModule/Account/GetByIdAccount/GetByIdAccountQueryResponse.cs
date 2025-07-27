using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SO.Application.Features.Queries.AccountModule.Account.GetByIdAccount
{
    public class GetByIdAccountQueryResponse
    {
        // Handler'dan gelen sonucu (tek bir müşteri DTO'su) bu özellik tutacak.
        public object Result { get; set; }
    }
}
