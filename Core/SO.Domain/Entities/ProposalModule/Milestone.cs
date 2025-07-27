using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SO.Domain.Entities.Common;

namespace SO.Domain.Entities.ProposalModule
{
    public class Milestone : BaseEntity
    {
        public string Name { get; set; } = string.Empty; // DÜZELTME
        public string? Description { get; set; }
        public DateTime PlannedCompletionDate { get; set; }
        public bool IsCompleted { get; set; } = false;
        public Guid ProposalId { get; set; }
        public virtual Proposal Proposal { get; set; } = null!; // DÜZELTME
    }
}
