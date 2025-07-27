using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SO.Application.Repositories.ProposalModule;
using SO.Domain.Entities.ProposalModule;
using SO.Persistence.Contexts;

namespace SO.Persistence.Repositories.ProposalModule
{
    public class ProposalReadRepository : ReadRepository<Proposal>, IProposalReadRepository
    {
        public ProposalReadRepository(SODbContext context) : base(context)
        {
        }
    }
}
