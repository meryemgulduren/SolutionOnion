using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MediatR;

namespace SO.Application.Features.Commands.AccountModule.Account.UpdateAccount
{
    // Bu sınıf, bir müşteriyi güncelleme isteğini ve güncellenecek verileri temsil eder.
    public class UpdateAccountCommandRequest : IRequest<UpdateAccountCommandResponse>
    {
        public string Id { get; set; } // Hangi müşterinin güncelleneceğini belirtir
        public string CompanyName { get; set; }
        public string? ContactPerson { get; set; }
        public string Email { get; set; }
        public string? PhoneNumber { get; set; }
        public bool IsActive { get; set; }
    }
}