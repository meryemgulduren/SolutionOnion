using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SO.Domain.Entities.Common;

namespace SO.Domain.Entities.ProposalModule
{
    public class BusinessObjective : BaseEntity
    {
        public string Objective { get; set; } = string.Empty;
        public string Alignment { get; set; } = string.Empty;

        // Bu hedefin hangi teklife ait olduğu
        public Guid ProposalId { get; set; }
        public virtual Proposal Proposal { get; set; }
    }
}
