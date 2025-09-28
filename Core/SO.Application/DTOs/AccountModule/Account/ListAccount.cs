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
        public string? PhoneNumber { get; set; }
        public string? Phone { get; set; } // JavaScript için alias
        public string? TaxOffice { get; set; }
        public string? TaxNumber { get; set; }
        public bool IsActive { get; set; }
        public string Status { get; set; } // Active/Inactive string
        public DateTime CreatedDate { get; set; }
        public string? CreatedById { get; set; } // Kullanıcı ID'si
        public string? CreatedBy { get; set; } // Kullanıcı adı
    }
}
