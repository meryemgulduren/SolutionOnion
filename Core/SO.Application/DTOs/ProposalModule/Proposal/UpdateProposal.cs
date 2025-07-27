using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SO.Application.DTOs.ProposalModule.Proposal
{
    public class UpdateProposal
    {
        public string Id { get; set; }
        public string ProposalName { get; set; }
        public string Status { get; set; }
        // Güncellenmesi istenen diğer alanlar buraya eklenebilir.
    }
}
