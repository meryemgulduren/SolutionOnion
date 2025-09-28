using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SO.Application.DTOs.AccountModule.Address
{
    public class ListAddress
    {
        public string Id { get; set; }
        public string AccountId { get; set; }
        public bool isDefault { get; set; } = false;
        public string? AddressType { get; set; }
        public string? AddressName { get; set; }
        public string? AddressLine1 { get; set; }
        public string? AddressLine2 { get; set; }
        public string? AddressLine { get; set; } // JavaScript için birleştirilmiş adres
        public string? Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string? Zip { get; set; }
        public string? PostalCode { get; set; } // JavaScript için alias
        public string Country { get; set; }
        public string? Description { get; set; }
        public string? Phone { get; set; }
        public string? Fax { get; set; }
        public string? Mail { get; set; }
        public bool Active { get; set; } = true;
        public string? CreatedById { get; set; } // Kullanıcı ID'si
        public string? CreatedBy { get; set; } // Kullanıcı adı
        public DateTime CreatedDate { get; set; }
        public string? CompanyName { get; set; } // Şirket adı
    }
}
