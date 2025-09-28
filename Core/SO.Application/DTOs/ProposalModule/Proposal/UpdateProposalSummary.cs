using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SO.Application.DTOs.ProposalModule.BusinessObjective;
using System.Collections.Generic;

namespace SO.Application.DTOs.ProposalModule.Proposal
{
    // Bu DTO, sadece Project Summary adımından gelen verileri taşımak için kullanılır.
    public class UpdateProposalSummary
    {
        public string ProposalId { get; set; }
        public string? ProjectDescription { get; set; }
        public int? OfferDurationDays { get; set; }
        public int? DeliveryDurationDays { get; set; }
        public string? OfferOwner { get; set; }
        public decimal? QuantityValue { get; set; }
        public string? QuantityUnit { get; set; }
        public string? GeneralNote { get; set; }
        public string? AddressId { get; set; }
        // Ticari
        public decimal? TargetPrice { get; set; }
        public string? PaymentMethod { get; set; }
        public string? PaymentTerm { get; set; }
        public string? CommercialNote { get; set; }
        public DateTime? ValidUntilDate { get; set; }
    }
}