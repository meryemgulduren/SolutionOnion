using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using SO.Persistence.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SO.Persistence
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<SODbContext>
    {
        public SODbContext CreateDbContext(string[] args)
        {
            
            DbContextOptionsBuilder<SODbContext> dbContextOptionsBuilder = new ();
            dbContextOptionsBuilder.UseMySQL(Configuration.ConnectionString);
                return new(dbContextOptionsBuilder.Options, null);
        }
    }
}
