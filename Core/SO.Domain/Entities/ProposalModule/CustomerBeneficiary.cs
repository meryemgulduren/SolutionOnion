using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SO.Domain.Entities.Common;

namespace SO.Domain.Entities.ProposalModule
{
    // Bu sınıf, projeden faydalanacak olanları (Müşteri, Son Kullanıcı vb.) temsil eder.
    public class CustomerBeneficiary : BaseEntity
    {
        public string Beneficiary { get; set; } = string.Empty;
        public string? NeedsAndConcern { get; set; }

        // Bu kaydın hangi teklife ait olduğu
        public Guid ProposalId { get; set; }
        public virtual Proposal Proposal { get; set; } = null!;
    }
}
