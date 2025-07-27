using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MediatR;

namespace SO.Application.Features.Commands.AccountModule.Account.DeleteAccount
{
    // Bu sınıf, bir müşteriyi silme isteğini temsil eder.
    // Hangi müşterinin silineceğini belirtmek için sadece Id bilgisi yeterlidir.
    public class DeleteAccountCommandRequest : IRequest<DeleteAccountCommandResponse>
    {
        public string Id { get; set; }
    }
}
