using System.ComponentModel.DataAnnotations;

namespace SO.Application.DTOs.ProposalModule.BusinessPartner
{
    public class CreateBusinessPartner
    {
        [Required]
        public Guid ProposalId { get; set; }
        
        [Required]
        [StringLength(200)]
        public string PartnerName { get; set; } = string.Empty;
        
        [Required]
        [StringLength(100)]
        public string Role { get; set; } = string.Empty;
        
        [StringLength(200)]
        public string? ContactInfo { get; set; }
        
        [StringLength(500)]
        public string? Notes { get; set; }
    }
}
