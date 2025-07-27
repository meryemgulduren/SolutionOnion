using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MediatR;

namespace SO.Application.Features.Commands.AccountModule.Account.CreateAccount
{
    // Bu sınıf, yeni bir müşteri oluşturma isteğini ve bunun için gereken verileri temsil eder.
    public class CreateAccountCommandRequest : IRequest<CreateAccountCommandResponse>
    {
        public string CompanyName { get; set; }
        public string? ContactPerson { get; set; }
        public string Email { get; set; }
        public string? PhoneNumber { get; set; }
    }
}
