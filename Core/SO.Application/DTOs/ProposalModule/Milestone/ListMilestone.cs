using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System;

namespace SO.Application.DTOs.ProposalModule.Milestone
{
    public class ListMilestone
    {
        public string Name { get; set; }
        public string? Description { get; set; }

        public DateTime PlannedCompletionDate { get; set; }
        public bool IsCompleted { get; set; }
    }
}