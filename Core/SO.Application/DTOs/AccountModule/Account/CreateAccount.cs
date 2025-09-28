using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SO.Application.DTOs.AccountModule.Account
{
    public class CreateAccount
    {
        public string CompanyName { get; set; }
        public string? ContactPerson { get; set; }
        public string Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? TaxOffice { get; set; }
        public string? TaxNumber { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
