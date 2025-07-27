using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SO.Application.DTOs.AccountModule.Account
{
    public class ListAccount
    {
        public Guid Id { get; set; }
        public string CompanyName { get; set; }
        public string? ContactPerson { get; set; }
        public string Email { get; set; }
    }
}
