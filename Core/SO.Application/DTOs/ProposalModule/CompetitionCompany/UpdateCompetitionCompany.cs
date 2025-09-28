using System.ComponentModel.DataAnnotations;

namespace SO.Application.DTOs.ProposalModule.CompetitionCompany
{
    public class UpdateCompetitionCompany
    {
        [Required]
        public Guid Id { get; set; }
        
        [Required]
        public Guid ProposalId { get; set; }
        
        [Required]
        [StringLength(200)]
        public string CompanyName { get; set; } = string.Empty;
        
        public decimal? CompetedPrice { get; set; }
        
        [StringLength(500)]
        public string? Notes { get; set; }
    }
}
