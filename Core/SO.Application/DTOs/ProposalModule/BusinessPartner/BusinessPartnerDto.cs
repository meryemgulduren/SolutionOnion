namespace SO.Application.DTOs.ProposalModule.BusinessPartner
{
    public class BusinessPartnerDto
    {
        public Guid Id { get; set; }
        public Guid ProposalId { get; set; }
        public string PartnerName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string? ContactInfo { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? CreatedById { get; set; }
        public string? CreatedBy { get; set; }
    }
}
