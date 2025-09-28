using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace SO.Domain.Entities.Identity
{
    public class AppUser : IdentityUser<string>
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public override string Id { get; set; } = Guid.NewGuid().ToString();
        
        public string? FullName { get; set; }
        public string? ProfilePicture { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? LastLoginDate { get; set; }
        public bool IsActive { get; set; } = true;
        
        // Navigation properties
        public virtual ICollection<ProposalModule.Proposal> CreatedProposals { get; set; } = new List<ProposalModule.Proposal>();
        public virtual ICollection<AccountModule.Account> ManagedAccounts { get; set; } = new List<AccountModule.Account>();
    }
}
