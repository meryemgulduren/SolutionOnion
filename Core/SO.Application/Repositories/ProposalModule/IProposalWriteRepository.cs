using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SO.Domain.Entities.ProposalModule;

namespace SO.Application.Repositories.ProposalModule
{
    public interface IProposalWriteRepository : IWriteRepository<Proposal>
    {
        // Proposal'a özel yazma metotları gerekirse ileride buraya eklenecek.
    }
}
