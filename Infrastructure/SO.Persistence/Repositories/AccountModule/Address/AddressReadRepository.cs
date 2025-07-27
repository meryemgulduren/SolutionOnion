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
    public class AddressReadRepository : ReadRepository<Address>, IAddressReadRepository
    {
        public AddressReadRepository(SODbContext context) : base(context)
        {
        }
    }
}
