using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SO.Application.DTOs.ProposalModule.Proposal
{
    public class SingleProposal
    {
        public Guid Id { get; set; }
        public Guid AccountId { get; set; }
        public string ProposalName { get; set; }
        public string CompanyName { get; set; }
        public string PreparedBy { get; set; }
        public string Status { get; set; }
        public DateTime ProposalDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Currency { get; set; }
        public string? Description { get; set; }
        public string? CreatedById { get; set; }
       
        public string? ProjectDescription { get; set; }
        public int? OfferDurationDays { get; set; }
        public int? DeliveryDurationDays { get; set; }
        public string? OfferOwner { get; set; }
        public decimal? QuantityValue { get; set; }
        public string? QuantityUnit { get; set; }
        public string? GeneralNote { get; set; }
        public Guid? AddressId { get; set; }
        // Müşteri Bilgileri (Account'tan)
        public string? CustomerName { get; set; }
        public string? CustomerPhone { get; set; }
        public string? CustomerEmail { get; set; }
        // Müşteri Bilgileri (Address'ten)
        public string? CustomerFax { get; set; }
        public string? CustomerAddress { get; set; }
        // Ticari
        public decimal? TargetPrice { get; set; }
        public string? PaymentMethod { get; set; }
        public string? PaymentTerm { get; set; }
        public string? CommercialNote { get; set; }
        public DateTime? ValidUntilDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public List<ListCommercialCompetitor> Competitors { get; set; }
        public List<ListCommercialPartner> Partners { get; set; }

    }

    public class ListCommercialCompetitor
    {
        public string CompanyName { get; set; }
        public decimal? OfferedPrice { get; set; }
        public string? Note { get; set; }
    }

    public class ListCommercialPartner
    {
        public string CompanyName { get; set; }
        public string? Role { get; set; }
        public string? Note { get; set; }
    }
}
