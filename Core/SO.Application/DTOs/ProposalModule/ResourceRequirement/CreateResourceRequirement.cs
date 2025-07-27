using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace SO.Application.DTOs.ProposalModule.ResourceRequirement
{
    public class CreateResourceRequirement
    {
        public string Resource { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}
