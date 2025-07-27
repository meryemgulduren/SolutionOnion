using SO.Domain.Entities.Common;
using SO.Domain.Entities.ProposalModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SO.Domain.Entities.AccountModule
{
    public class Account : BaseEntity
    {
        public string CompanyName { get; set; } = string.Empty;
        public string? ContactPerson { get; set; }
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string? TaxOffice { get; set; }
        public string? TaxNumber { get; set; }
        public bool IsActive { get; set; } = true;
        public virtual ICollection<Address> Addresses { get; set; } = new List<Address>();
        public virtual ICollection<Proposal> Proposals { get; set; } = new List<Proposal>();
    }
}
