using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SO.Application.Repositories;
using SO.Persistence.Repositories;
using SO.Domain.Entities.Identity;
using SO.Persistence.Contexts;
using SO.Application.Abstractions.Services.AccountModule;
 

using SO.Persistence.Services.AccountModule;
using SO.Application.Abstractions.Services.ProposalModule;
using SO.Persistence.Services.ProposalModule;

using System;

namespace SO.Persistence
{
    public static class ServiceRegistration
    {
        public static void AddPersistenceServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<SODbContext>(options => options.UseMySQL(Configuration.ConnectionString));

            // Identity servisleri Program.cs'te ekleniyor, burada eklemeye gerek yok

            // --- Genel (Generic) Repository Kayıtları ---
            services.AddScoped(typeof(IReadRepository<>), typeof(ReadRepository<>));
            services.AddScoped(typeof(IWriteRepository<>), typeof(WriteRepository<>));

            // --- Address Modülü Servis Kayıtları ---
            services.AddScoped<IAddressService, AddressService>();

            // --- Account Modülü Servis Kayıtları ---
            services.AddScoped<IAccountService, AccountService>();

            // --- Proposal Modülü Servis Kayıtları ---
            services.AddScoped<IProposalService, ProposalService>();
            services.AddScoped<IBusinessPartnerService, BusinessPartnerService>();
            services.AddScoped<ICompetitionCompanyService, CompetitionCompanyService>();

            // --- Database Seeder ---
            services.AddScoped<SO.Persistence.Seed.DatabaseSeeder>();

        }
    }
}