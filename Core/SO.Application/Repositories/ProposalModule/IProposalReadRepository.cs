using SO.Domain.Entities.ProposalModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SO.Application.Repositories.ProposalModule
{
    public interface IProposalReadRepository : IReadRepository<Proposal>
    {
        // Proposal'a özel okuma metotları gerekirse ileride buraya eklenecek.
    }
}
