using SO.Application.Repositories.AccountModule;
using SO.Domain.Entities.AccountModule;
using SO.Persistence.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SO.Persistence.Repositories.AccountModule
{
    public class AddressWriteRepository : WriteRepository<Address>, IAddressWriteRepository
    {
        public AddressWriteRepository(SODbContext context) : base(context)
        {
        }
    }
}
