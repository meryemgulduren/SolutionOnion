namespace SO.Application.DTOs.ProposalModule.CompetitionCompany
{
    public class CompetitionCompanyDto
    {
        public Guid Id { get; set; }
        public Guid ProposalId { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public decimal? CompetedPrice { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? CreatedById { get; set; }
        public string? CreatedBy { get; set; }
    }
}
