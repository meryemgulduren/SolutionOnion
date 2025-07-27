using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SO.Application.Repositories;
using SO.Persistence.Repositories;
using SO.Domain.Entities.Identity;
using SO.Persistence.Contexts;
using SO.Application.Abstractions.Services.AccountModule;
using SO.Application.Repositories.AccountModule; 
using SO.Persistence.Repositories.AccountModule;
using SO.Persistence.Services.AccountModule;
using SO.Application.Abstractions.Services.ProposalModule;
using SO.Application.Repositories.ProposalModule;
using SO.Persistence.Services.ProposalModule;
using SO.Persistence.Repositories.ProposalModule;
using System;

namespace SO.Persistence
{
    public static class ServiceRegistration
    {
        public static void AddPersistenceServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<SODbContext>(options => options.UseMySQL(Configuration.ConnectionString));

            services.AddIdentity<AppUser, AppRole>()
                .AddEntityFrameworkStores<SODbContext>();

            // --- Genel (Generic) Repository Kayıtları ---
            services.AddScoped(typeof(IReadRepository<>), typeof(ReadRepository<>));
            services.AddScoped(typeof(IWriteRepository<>), typeof(WriteRepository<>));

            // --- Address Modülü Servis Kayıtları ---
            services.AddScoped<IAddressService, AddressService>();
            services.AddScoped<IAddressReadRepository, AddressReadRepository>();
            services.AddScoped<IAddressWriteRepository, AddressWriteRepository>();

            // --- Account Modülü Servis Kayıtları ---
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IAccountReadRepository, AccountReadRepository>();
            services.AddScoped<IAccountWriteRepository, AccountWriteRepository>();

            // --- Proposal Modülü Servis Kayıtları ---
            services.AddScoped<IProposalService, ProposalService>();
            services.AddScoped<IProposalReadRepository, ProposalReadRepository>();
            services.AddScoped<IProposalWriteRepository, ProposalWriteRepository>();
        }
    }
}