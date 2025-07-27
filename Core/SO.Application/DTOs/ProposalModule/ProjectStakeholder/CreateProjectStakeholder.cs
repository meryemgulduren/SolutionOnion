using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SO.Application.DTOs.ProposalModule.ProjectStakeholder
{
    public class CreateProjectStakeholder
    {
        public string Role { get; set; }
        public string Name { get; set; }
        public string? Responsibilities { get; set; }
    }
}
