using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SO.Domain.Entities.AccountModule;
using SO.Domain.Entities.Common;
using SO.Domain.Entities.Identity; // Bunu ekleyeceğiz
using SO.Domain.Entities.ProposalModule;

namespace SO.Persistence.Contexts
{
    // IdentityDbContext'i AppUser ve AppRole ile birlikte kullanacağımızı belirtiyoruz.
    public class SODbContext : IdentityDbContext<AppUser, AppRole, string>
    {
       
        public SODbContext(DbContextOptions options) : base(options)
        {
        }

        // Domain katmanında oluşturduğumuz tüm entity'ler için DbSet'ler
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Proposal> Proposals { get; set; }
        public DbSet<ProposalItem> ProposalItems { get; set; }
        public DbSet<Milestone> Milestones { get; set; }
        public DbSet<SuccessCriterion> SuccessCriteria { get; set; }
        public DbSet<CriticalSuccessFactor> CriticalSuccessFactors { get; set; }
        public DbSet<BusinessObjective> BusinessObjectives { get; set; }
        public DbSet<CustomerBeneficiary> CustomerBeneficiaries { get; set; }
        public DbSet<ProjectStakeholder> ProjectStakeholders { get; set; }
        public DbSet<ResourceRequirement> ResourceRequirements { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
       

        {
            // Bu satır Identity tablolarının (AspNetUsers vb.) kurulması için önemlidir.
            base.OnModelCreating(builder);

            // Fluent API ile tüm ilişkileri burada tanımlamak en iyi pratiktir.
            builder.Entity<Account>()
                .HasMany(a => a.Addresses)
                .WithOne(ad => ad.Account)
                .HasForeignKey(ad => ad.AccountId);

            builder.Entity<Account>()
                .HasMany(a => a.Proposals)
                .WithOne(p => p.Account)
                .HasForeignKey(p => p.AccountId);

            builder.Entity<Proposal>()
                .HasMany(p => p.ProposalItems)
                .WithOne(pi => pi.Proposal)
                .HasForeignKey(pi => pi.ProposalId);

            builder.Entity<Proposal>()
                .HasMany(p => p.Milestones)
                .WithOne(m => m.Proposal)
                .HasForeignKey(m => m.ProposalId);

            builder.Entity<Proposal>()
                .HasMany(p => p.SuccessCriteria)
                .WithOne(sc => sc.Proposal)
                .HasForeignKey(sc => sc.ProposalId);

            builder.Entity<Proposal>()
                .HasMany(p => p.CriticalSuccessFactors)
                .WithOne(csf => csf.Proposal)
                .HasForeignKey(csf => csf.ProposalId);
            builder.Entity<Proposal>()
               .HasMany(p => p.BusinessObjectives)
               .WithOne(bo => bo.Proposal)
               .HasForeignKey(bo => bo.ProposalId);
            builder.Entity<Proposal>()
               .HasMany(p => p.ProjectStakeholders)
               .WithOne(ps => ps.Proposal)
               .HasForeignKey(ps => ps.ProposalId);
            builder.Entity<Proposal>()
                .HasMany(p => p.CustomerBeneficiaries)
                .WithOne(cb => cb.Proposal)
                .HasForeignKey(cb => cb.ProposalId);
            builder.Entity<Proposal>()
              .HasMany(p => p.ResourceRequirements)
              .WithOne(rr => rr.Proposal)
              .HasForeignKey(rr => rr.ProposalId);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var datas = ChangeTracker.Entries<BaseEntity>();

            foreach (var data in datas)
            {
                // _ = data.State switch ... C# 8.0 ve üzeri için daha modern bir yazımdır.
                switch (data.State)
                {
                    case EntityState.Added:
                        data.Entity.CreatedDate = DateTime.UtcNow;
                        // data.Entity.CreatedBy = ...; // Login olan kullanıcı bilgisi buraya gelecek
                        break;
                    case EntityState.Modified:
                        data.Entity.ModifiedDate = DateTime.UtcNow;
                        // data.Entity.ModifiedBy = ...; // Login olan kullanıcı bilgisi buraya gelecek
                        break;
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
