using SO.Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace SO.Domain.Entities.AccountModule
{
    public class Address : BaseEntity
    {
        public Guid AccountId { get; set; }
        public bool isDefault { get; set; } = false;
        public string? AddressType { get; set; }
        public string? AddressName { get; set; }
        public string? AddressLine1 { get; set; }
        public string? AddressLine2 { get; set; }
        public string? Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string? Zip { get; set; }
        public string Country { get; set; }
        public string? Description { get; set; }
        public string? Phone { get; set; }
        public string? Fax { get; set; }
        public string? Mail { get; set; }
        public bool Active { get; set; } = true;

        // Navigation properties
        public Account Account { get; set; }
    }
}
