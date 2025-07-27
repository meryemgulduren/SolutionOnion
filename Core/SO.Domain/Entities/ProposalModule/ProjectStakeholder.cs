using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SO.Domain.Entities.Common;

namespace SO.Domain.Entities.ProposalModule
{
    // Bu sınıf, Projeyi Başlatan, Sponsor gibi paydaşları temsil eder.
    public class ProjectStakeholder : BaseEntity
    {
        public string Role { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Responsibilities { get; set; }

        // Bu paydaşın hangi teklife ait olduğu
        public Guid ProposalId { get; set; }
        public virtual Proposal Proposal { get; set; } = null!;
    }
}
