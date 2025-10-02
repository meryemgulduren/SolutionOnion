using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SO.Domain.Entities.AccountModule;
using SO.Domain.Entities.Common;
using SO.Domain.Entities.Identity;
using SO.Domain.Entities.ProposalModule;
using System;
using Microsoft.AspNetCore.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SO.Persistence.Contexts
{
    // IdentityDbContext'i AppUser ve AppRole ile birlikte kullanacağımızı belirtiyoruz.
    public class SODbContext : IdentityDbContext<AppUser, AppRole, string>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SODbContext(DbContextOptions options, IHttpContextAccessor httpContextAccessor) : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        // Domain katmanında oluşturduğumuz tüm entity'ler için DbSet'ler
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Proposal> Proposals { get; set; }
        public DbSet<CompetitionCompany> CompetitionCompanies { get; set; }
        public DbSet<BusinessPartner> BusinessPartners { get; set; }
        public DbSet<ProposalRisk> ProposalRisks { get; set; }
        
        // Permission system
        public DbSet<AppPermission> Permissions { get; set; }
        public DbSet<PermissionRole> PermissionRoles { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            // Bu satır Identity tablolarının (AspNetUsers vb.) kurulması için önemlidir.
            base.OnModelCreating(builder);

            // Identity User ile Proposal ilişkisi
            builder.Entity<AppUser>()
                .HasMany(u => u.CreatedProposals)
                .WithOne()
                .HasForeignKey("CreatedById")
                .OnDelete(DeleteBehavior.Restrict);

            // Identity User ile Account ilişkisi
            builder.Entity<AppUser>()
                .HasMany(u => u.ManagedAccounts)
                .WithOne()
                .HasForeignKey("ManagedById")
                .OnDelete(DeleteBehavior.Restrict);

            // Account-Address
            builder.Entity<Account>()
                .HasMany(a => a.Addresses)
                .WithOne(ad => ad.Account)
                .HasForeignKey(ad => ad.AccountId);

            // Account-Proposal
            builder.Entity<Account>()
                .HasMany(a => a.Proposals)
                .WithOne(p => p.Account)
                .HasForeignKey(p => p.AccountId);

            // Proposal-CompetitionCompany
            builder.Entity<Proposal>()
                .HasMany(p => p.CompetitionCompanies)
                .WithOne(cc => cc.Proposal)
                .HasForeignKey(cc => cc.ProposalId)
                .OnDelete(DeleteBehavior.Cascade);

            // Proposal-BusinessPartner
            builder.Entity<Proposal>()
                .HasMany(p => p.BusinessPartners)
                .WithOne(bp => bp.Proposal)
                .HasForeignKey(bp => bp.ProposalId)
                .OnDelete(DeleteBehavior.Cascade);

            // Proposal-ProposalRisk
            builder.Entity<Proposal>()
                .HasMany(p => p.ProposalRisks)
                .WithOne(pr => pr.Proposal)
                .HasForeignKey(pr => pr.ProposalId)
                .OnDelete(DeleteBehavior.Cascade);

            // Permission-PermissionRole ilişkisi
            builder.Entity<PermissionRole>()
                .HasOne(pr => pr.Permission)
                .WithMany(p => p.PermissionRoles)
                .HasForeignKey(pr => pr.PermissionId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<PermissionRole>()
                .HasOne(pr => pr.Role)
                .WithMany()
                .HasForeignKey(pr => pr.RoleId)
                .OnDelete(DeleteBehavior.Cascade);

            // Unique constraint: Aynı permission-role kombinasyonu sadece bir kez olabilir
            builder.Entity<PermissionRole>()
                .HasIndex(pr => new { pr.PermissionId, pr.RoleId })
                .IsUnique();

        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var datas = ChangeTracker.Entries<BaseEntity>();
            var currentUserId = GetCurrentUserId();

            foreach (var data in datas)
            {
                switch (data.State)
                {
                    case EntityState.Added:
                        data.Entity.CreatedDate = DateTime.UtcNow;
                        data.Entity.CreatedById = currentUserId;
                        break;
                    case EntityState.Modified:
                        data.Entity.ModifiedDate = DateTime.UtcNow;
                        data.Entity.ModifiedById = currentUserId;
                        break;
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }

        private string? GetCurrentUserId()
        {
            try
            {
                var user = _httpContextAccessor?.HttpContext?.User;
                if (user?.Identity?.IsAuthenticated == true)
                {
                    // NameIdentifier claim'ini kullan (Identity'de bu genellikle user ID'dir)
                    var nameIdentifier = user.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                    if (!string.IsNullOrEmpty(nameIdentifier))
                    {
                        System.Diagnostics.Debug.WriteLine($"SODbContext.GetCurrentUserId: Found user ID: {nameIdentifier}");
                        return nameIdentifier;
                    }
                    
                    // Debug: Tüm claim'leri yazdır
                    System.Diagnostics.Debug.WriteLine("SODbContext.GetCurrentUserId: No NameIdentifier found, checking all claims:");
                    foreach (var claim in user.Claims)
                    {
                        System.Diagnostics.Debug.WriteLine($"Claim: {claim.Type} = {claim.Value}");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"SODbContext.GetCurrentUserId: Error: {ex.Message}");
            }
            return null;
        }
    }
}

