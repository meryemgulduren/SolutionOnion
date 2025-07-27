using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SO.Domain.Entities.Common;

namespace SO.Domain.Entities.ProposalModule
{
    public class SuccessCriterion : BaseEntity
    {
       
        public string Description { get; set; } 

        public Guid ProposalId { get; set; } 
        public virtual Proposal Proposal { get; set; }
    }
}
