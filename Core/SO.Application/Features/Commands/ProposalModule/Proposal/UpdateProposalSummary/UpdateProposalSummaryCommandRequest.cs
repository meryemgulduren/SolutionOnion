using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using SO.Application.DTOs.ProposalModule.BusinessObjective;

namespace SO.Application.Features.Commands.ProposalModule.Proposal.UpdateProposalSummary
{
	public class UpdateProposalSummaryCommandRequest : IRequest<UpdateProposalSummaryCommandResponse>
	{
		public string Id { get; set; }
		
		// Wizard step tracking
		public int CurrentStep { get; set; } = 1;
		public int NextStep { get; set; } = 1;
		
		public string? ProjectDescription { get; set; }
		
		[Required(ErrorMessage = "Teklif Süresi (gün) alanı zorunludur")]
		[Range(1, int.MaxValue, ErrorMessage = "Teklif Süresi 1'den büyük olmalıdır")]
		public int? OfferDurationDays { get; set; }
		
		[Required(ErrorMessage = "Teslim Süresi (gün) alanı zorunludur")]
		[Range(1, int.MaxValue, ErrorMessage = "Teslim Süresi 1'den büyük olmalıdır")]
		public int? DeliveryDurationDays { get; set; }
		
		[Required(ErrorMessage = "Teklif Sorumlusu alanı zorunludur")]
		[StringLength(100, ErrorMessage = "Teklif Sorumlusu en fazla 100 karakter olabilir")]
		public string? OfferOwner { get; set; }
		
		[Required(ErrorMessage = "Miktar alanı zorunludur")]
		[Range(0.01, double.MaxValue, ErrorMessage = "Miktar 0.01'den büyük olmalıdır")]
		public decimal? QuantityValue { get; set; }
		
		[Required(ErrorMessage = "Birim seçimi zorunludur")]
		public string? QuantityUnit { get; set; }
		
		public string? GeneralNote { get; set; }
		public string? AddressId { get; set; }
		// Ticari
		[Required(ErrorMessage = "Hedef Fiyat alanı zorunludur")]
		[Range(0.01, double.MaxValue, ErrorMessage = "Hedef Fiyat 0.01'den büyük olmalıdır")]
		public decimal? TargetPrice { get; set; }
		
		[Required(ErrorMessage = "Ödeme Metodu alanı zorunludur")]
		[StringLength(100, ErrorMessage = "Ödeme Metodu en fazla 100 karakter olabilir")]
		public string? PaymentMethod { get; set; }
		
		[Required(ErrorMessage = "Ödeme Vadesi alanı zorunludur")]
		[StringLength(100, ErrorMessage = "Ödeme Vadesi en fazla 100 karakter olabilir")]
		public string? PaymentTerm { get; set; }
		
		[Required(ErrorMessage = "Geçerlilik Tarihi alanı zorunludur")]
		public DateTime? ValidUntilDate { get; set; }
		
		public decimal? CompetitivePrice { get; set; }
		
		[StringLength(1000, ErrorMessage = "Ticari Not en fazla 1000 karakter olabilir")]
		public string? CommercialNote { get; set; }
	}
}