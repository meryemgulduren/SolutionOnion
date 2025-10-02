using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SO.Domain.Entities.AccountModule;
using SO.Domain.Entities.Common;
using SO.Domain.Enums;

namespace SO.Domain.Entities.ProposalModule
{
    public class Proposal : BaseEntity
    {
        // Genel tanım alanları
        public string ProposalCode { get; set; } = string.Empty; // Otomatik oluşturulan proje kodu (örn: PR.HDR.170922.01)
        public string ProposalName { get; set; } = string.Empty;
        public string PreparedBy { get; set; } = string.Empty;
        public ProposalStatus Status { get; set; } = ProposalStatus.Draft;
        public DateTime ProposalDate { get; set; }
        public string? ProjectDescription { get; set; }
        
        // Wizard step tracking
        public int CurrentStep { get; set; } = 1; // 1: Genel Tanım, 2: Ticari, 3: Riskler
        
        // Proje detayları
        public int? OfferDurationDays { get; set; }
        public int? DeliveryDurationDays { get; set; }
        public string? OfferOwner { get; set; }
        public decimal? QuantityValue { get; set; }
        public string? QuantityUnit { get; set; }
        public string? GeneralNote { get; set; }
        
        // Ticari alanlar
        public decimal? TargetPrice { get; set; }
        public string? PaymentMethod { get; set; }
        public string? PaymentTerm { get; set; }
        public string? CommercialNote { get; set; }
        public DateTime? ValidUntilDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Currency { get; set; } = "TRY";
        
        // İlişkiler
        public Guid AccountId { get; set; }
        public Guid? AddressId { get; set; }
        public virtual Account Account { get; set; } = null!;
        
        // Yeni tablo ilişkileri
        public virtual ICollection<CompetitionCompany> CompetitionCompanies { get; set; } = new List<CompetitionCompany>();
        public virtual ICollection<BusinessPartner> BusinessPartners { get; set; } = new List<BusinessPartner>();
        public virtual ICollection<ProposalRisk> ProposalRisks { get; set; } = new List<ProposalRisk>();
    }
}
