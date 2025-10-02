using Microsoft.EntityFrameworkCore;
using SO.Application.Abstractions.Services.ProposalModule;
using SO.Application.Abstractions.Services.AccountModule;
using SO.Application.DTOs.ProposalModule.Proposal;
using SO.Application.Repositories;
using SO.Domain.Entities.ProposalModule;
using SO.Domain.Entities.AccountModule;
using SO.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SO.Persistence.Services.ProposalModule
{
	public class ProposalService : IProposalService
	{
		private readonly IWriteRepository<Proposal> _proposalWriteRepository;
		private readonly IReadRepository<Proposal> _proposalReadRepository;
		private readonly IReadRepository<Account> _accountReadRepository;
		private readonly IAddressService _addressService;

		public ProposalService(
			IWriteRepository<Proposal> proposalWriteRepository, 
			IReadRepository<Proposal> proposalReadRepository, 
			IReadRepository<Account> accountReadRepository,
			IAddressService addressService)
		{
			_proposalWriteRepository = proposalWriteRepository;
			_proposalReadRepository = proposalReadRepository;
			_accountReadRepository = accountReadRepository;
			_addressService = addressService;
		}
		public async Task UpdateProposalSummaryAsync(UpdateProposalSummary summary)
		{
					// TÜM İŞLEM write-repo context üzerinden
		var proposal = await _proposalWriteRepository.Table
			.FirstOrDefaultAsync(p => p.Id.ToString() == summary.ProposalId);

			if (proposal == null) return;

					// Step güncelleme - bir sonraki adıma geçiş
		if (summary.NextStep > proposal.CurrentStep)
		{
			proposal.CurrentStep = summary.NextStep;
		}

		// ⭐ PARTIAL UPDATE: Sadece dolu (NULL olmayan) alanları güncelle
		// Bu sayede her kaydet butonu sadece kendi adımındaki alanları kaydeder
		// Önceki adımlarda kaydedilen veriler korunur
		
		// Genel Tanım Alanları - Sadece dolu ise güncelle
		if (!string.IsNullOrWhiteSpace(summary.ProjectDescription))
			proposal.ProjectDescription = summary.ProjectDescription;
		
		if (summary.OfferDurationDays.HasValue)
			proposal.OfferDurationDays = summary.OfferDurationDays;
		
		if (summary.DeliveryDurationDays.HasValue)
			proposal.DeliveryDurationDays = summary.DeliveryDurationDays;
		
		if (!string.IsNullOrWhiteSpace(summary.OfferOwner))
			proposal.OfferOwner = summary.OfferOwner;
		
		if (summary.QuantityValue.HasValue)
			proposal.QuantityValue = summary.QuantityValue;
		
		if (!string.IsNullOrWhiteSpace(summary.QuantityUnit))
			proposal.QuantityUnit = summary.QuantityUnit;
		
		if (!string.IsNullOrWhiteSpace(summary.GeneralNote))
			proposal.GeneralNote = summary.GeneralNote;

		if (!string.IsNullOrWhiteSpace(summary.AddressId) && Guid.TryParse(summary.AddressId, out var addrId))
			proposal.AddressId = addrId;

		// Ticari Alanlar - Sadece dolu ise güncelle
		if (summary.TargetPrice.HasValue)
			proposal.TargetPrice = summary.TargetPrice;
		
		if (!string.IsNullOrWhiteSpace(summary.PaymentMethod))
			proposal.PaymentMethod = summary.PaymentMethod;
		
		if (!string.IsNullOrWhiteSpace(summary.PaymentTerm))
			proposal.PaymentTerm = summary.PaymentTerm;
		
		if (!string.IsNullOrWhiteSpace(summary.CommercialNote))
			proposal.CommercialNote = summary.CommercialNote;
		
					if (summary.ValidUntilDate.HasValue)
				proposal.ValidUntilDate = summary.ValidUntilDate;

		// (İsteğe bağlı) explicit update
		_proposalWriteRepository.Update(proposal);

		await _proposalWriteRepository.SaveAsync();
		}

		public async Task<string> CreateProposalAsync(CreateProposal createProposal)
		{
			try
			{
				// Önce Account bilgisini al (firma adı için)
				var account = await _accountReadRepository.GetByIdAsync(createProposal.AccountId);
				string companyName = account?.CompanyName ?? "DEFAULT";
				
				// Proje kodunu oluştur
				string proposalCode = await GenerateProposalCodeAsync(companyName, DateTime.UtcNow);
				
				var newProposal = new Proposal
				{
					AccountId = Guid.Parse(createProposal.AccountId),
					ProposalCode = proposalCode,
					ProposalName = createProposal.ProposalName,
					PreparedBy = createProposal.PreparedBy,
					ProjectDescription = createProposal.ProjectDescription,
					ProposalDate = DateTime.UtcNow,
					Status = ProposalStatus.Draft,
					Currency = "TRY"
				};

				newProposal.TotalAmount = 0;

				await _proposalWriteRepository.AddAsync(newProposal);
				await _proposalWriteRepository.SaveAsync();

				return newProposal.Id.ToString();
			}
			catch (Exception ex)
			{
				throw new Exception($"Proposal creation failed: {ex.Message}", ex);
			}
		}
		
		// Otomatik proje kodu oluşturma
		private async Task<string> GenerateProposalCodeAsync(string companyName, DateTime date)
		{
			// 1. Firma kodunu oluştur: İlk harf + sonraki 2-3 sessiz harf
			string companyCode = GenerateCompanyCode(companyName);
			
			// 2. Tarih formatı: YYMMDD
			string dateCode = date.ToString("yyMMdd");
			
			// 3. Aynı gün ve aynı firma kodu için sıra numarası bul
			string baseCode = $"PR.{companyCode}.{dateCode}";
			
			var existingCodes = await _proposalReadRepository.Table
				.Where(p => p.ProposalCode.StartsWith(baseCode))
				.Select(p => p.ProposalCode)
				.ToListAsync();
			
			int sequenceNumber = 1;
			string proposalCode;
			
			do
			{
				proposalCode = $"{baseCode}.{sequenceNumber:D2}";
				sequenceNumber++;
			}
			while (existingCodes.Contains(proposalCode));
			
			return proposalCode;
		}
		
		// Firma adından kod oluştur
		private string GenerateCompanyCode(string companyName)
		{
			if (string.IsNullOrWhiteSpace(companyName))
				return "XXX";
			
			// Türkçe karakterleri normalize et
			companyName = companyName.ToUpperInvariant()
				.Replace('İ', 'I')
				.Replace('Ş', 'S')
				.Replace('Ğ', 'G')
				.Replace('Ü', 'U')
				.Replace('Ö', 'O')
				.Replace('Ç', 'C');
			
			// Sessiz harfler (consonants)
			var consonants = "BCDFGHJKLMNPQRSTVWXYZ";
			
			// İlk harfi al
			char firstLetter = companyName.FirstOrDefault(c => char.IsLetter(c));
			if (firstLetter == default(char))
				return "XXX";
			
			// Sonraki sessiz harfleri bul
			var remainingLetters = companyName.Skip(1).Where(c => char.IsLetter(c) && consonants.Contains(c)).ToList();
			
			// Kod oluştur: İlk harf + en az 2 sessiz harf
			var code = new System.Text.StringBuilder();
			code.Append(firstLetter);
			
			// En az 2, en fazla 3 sessiz harf ekle
			int consonantCount = Math.Min(remainingLetters.Count, 3);
			for (int i = 0; i < consonantCount && i < remainingLetters.Count; i++)
			{
				code.Append(remainingLetters[i]);
			}
			
			// Eğer yeterli sessiz harf yoksa 'X' ekle
			while (code.Length < 3)
			{
				code.Append('X');
			}
			
			return code.ToString().Substring(0, Math.Min(4, code.Length));
		}

		public async Task DeleteProposalAsync(string id)
		{
			try
			{
				// Önce proposal'ın var olup olmadığını kontrol et
				var proposal = await _proposalReadRepository.GetByIdAsync(id);
				if (proposal == null)
				{
					throw new Exception($"Proposal with ID {id} not found");
				}

				await _proposalWriteRepository.RemoveAsync(id);
				await _proposalWriteRepository.SaveAsync();
			}
			catch (Exception ex)
			{
				throw new Exception($"Failed to delete proposal: {ex.Message}", ex);
			}
		}

		public async Task<List<ListProposal>> GetAllProposalsAsync(string? currentUserId = null, bool isAdmin = false)
		{
			// Admin değilse ve kullanıcı ID varsa, sadece kendi proposal'larını getir
			if (!isAdmin && !string.IsNullOrEmpty(currentUserId))
			{
							return await _proposalReadRepository.GetAll(false)
				.Include(p => p.Account)
				.Where(p => p.CreatedById == currentUserId)
					.Select(p => new ListProposal
					{
						Id = p.Id,
						ProposalCode = p.ProposalCode,
						ProposalName = p.ProposalName,
						CompanyName = p.Account.CompanyName,
						ProposalDate = p.ProposalDate,
						TotalAmount = p.TotalAmount,
						Status = p.Status.ToString(),
						CreatedDate = p.CreatedDate,
						UpdatedDate = p.ModifiedDate,
						PreparedBy = p.PreparedBy,
						CreatedById = p.CreatedById,
						Currency = p.Currency
					}).ToListAsync();
			}
			else
			{
							// Admin ise tüm proposal'ları getir
			return await _proposalReadRepository.GetAll(false)
				.Include(p => p.Account)
				.Select(p => new ListProposal
					{
						Id = p.Id,
						ProposalCode = p.ProposalCode,
						ProposalName = p.ProposalName,
						CompanyName = p.Account.CompanyName,
						ProposalDate = p.ProposalDate,
						TotalAmount = p.TotalAmount,
						Status = p.Status.ToString(),
						CreatedDate = p.CreatedDate,
						UpdatedDate = p.ModifiedDate,
						PreparedBy = p.PreparedBy,
						CreatedById = p.CreatedById,
						Currency = p.Currency
					}).ToListAsync();
			}
		}

		public async Task<SingleProposal> GetProposalByIdAsync(string id)
		{
			System.Diagnostics.Debug.WriteLine($"ProposalService.GetProposalByIdAsync called with id: '{id}'");
			
			// Önce tüm proposal'ları listele
			var allProposals = await _proposalReadRepository.GetAll(false).ToListAsync();
			System.Diagnostics.Debug.WriteLine($"Total proposals in database: {allProposals.Count}");
			
			foreach (var p in allProposals)
			{
				System.Diagnostics.Debug.WriteLine($"Proposal ID: {p.Id}, ToString: '{p.Id.ToString()}', Name: {p.ProposalName}");
			}
			
			// Guid.TryParse ile ID'yi kontrol et
			if (!Guid.TryParse(id, out Guid proposalGuid))
			{
				System.Diagnostics.Debug.WriteLine($"Invalid GUID format: '{id}'");
				return null;
			}
			
					var proposal = await _proposalReadRepository.GetWhere(p => p.Id == proposalGuid)
			.Include(p => p.Account)
			.FirstOrDefaultAsync();
				
			System.Diagnostics.Debug.WriteLine($"Found proposal: {proposal != null}");

			// Address bilgilerini ayrıca çek
			SO.Application.DTOs.AccountModule.Address.SingleAddress? address = null;
			if (proposal?.AddressId.HasValue == true)
			{
				try
				{
					System.Diagnostics.Debug.WriteLine($"ProposalService: AddressId = {proposal.AddressId.Value}");
					address = await _addressService.GetAddressByIdAsync(proposal.AddressId.Value.ToString());
					System.Diagnostics.Debug.WriteLine($"ProposalService: Address found - Fax: {address?.Fax}, AddressLine1: {address?.AddressLine1}");
				}
				catch (ArgumentException ex)
				{
					System.Diagnostics.Debug.WriteLine($"ProposalService: Address not found - {ex.Message}");
					address = null;
				}
			}
			else
			{
				System.Diagnostics.Debug.WriteLine("ProposalService: No AddressId found, trying to get default address for Account");
				// AddressId yoksa Account'ın default adresini çek
				if (proposal?.AccountId != null)
				{
					try
					{
						var allAddresses = await _addressService.GetAllAddressesAsync();
						var accountAddresses = allAddresses.Where(a => a.AccountId == proposal.AccountId.ToString()).ToList();
						
						// Önce default adresi bul
						var defaultAddress = accountAddresses.FirstOrDefault(a => a.isDefault);
						
						// Default yoksa ilk adresi al
						if (defaultAddress == null && accountAddresses.Any())
						{
							defaultAddress = accountAddresses.First();
						}
						
						// Seçilen adresi SingleAddress olarak çek
						if (defaultAddress != null)
						{
							address = await _addressService.GetAddressByIdAsync(defaultAddress.Id);
						}
						
						System.Diagnostics.Debug.WriteLine($"ProposalService: Default address found - Fax: {address?.Fax}, AddressLine1: {address?.AddressLine1}");
					}
					catch (Exception ex)
					{
						System.Diagnostics.Debug.WriteLine($"ProposalService: Error getting default address - {ex.Message}");
						address = null;
					}
				}
			}

			if (proposal == null)
				return null;

			return new SingleProposal
			{
				Id = proposal.Id,
				ProposalCode = proposal.ProposalCode,
				ProposalName = proposal.ProposalName,
				AccountId = proposal.AccountId,
				CompanyName = proposal.Account.CompanyName,
				Status = proposal.Status.ToString(),
				ProposalDate = proposal.ProposalDate,
				CurrentStep = proposal.CurrentStep,
				ProjectDescription = proposal.ProjectDescription,
				OfferDurationDays = proposal.OfferDurationDays,
				DeliveryDurationDays = proposal.DeliveryDurationDays,
				OfferOwner = proposal.OfferOwner,
				QuantityValue = proposal.QuantityValue,
				QuantityUnit = proposal.QuantityUnit,
				GeneralNote = proposal.GeneralNote,
				AddressId = proposal.AddressId,
				// Müşteri Bilgileri (Account'tan)
				CustomerName = proposal.Account?.ContactPerson,
				CustomerPhone = proposal.Account?.PhoneNumber,
				CustomerEmail = proposal.Account?.Email,
				// Müşteri Bilgileri (Address'ten)
				CustomerFax = address?.Fax,
				CustomerAddress = address?.AddressLine1,
				// Ticari
				TargetPrice = proposal.TargetPrice,
				PaymentMethod = proposal.PaymentMethod,
				PaymentTerm = proposal.PaymentTerm,
				CommercialNote = proposal.CommercialNote,
				ValidUntilDate = proposal.ValidUntilDate,
				TotalAmount = proposal.TotalAmount,
				Currency = proposal.Currency,
				CreatedById = proposal.CreatedById,
				CreatedDate = proposal.CreatedDate,
				UpdatedDate = proposal.ModifiedDate,
				PreparedBy = proposal.PreparedBy
			};
		}

		public async Task UpdateProposalAsync(UpdateProposal updateProposal)
		{
			var proposal = await _proposalReadRepository.GetByIdAsync(updateProposal.Id);
			if (proposal != null)
			{
				proposal.ProposalName = updateProposal.ProposalName;
				if (Enum.TryParse<ProposalStatus>(updateProposal.Status, true, out var status))
				{
					proposal.Status = status;
				}

				_proposalWriteRepository.Update(proposal);
				await _proposalWriteRepository.SaveAsync();
			}
		}
		// Eski adım bazlı güncelleme metotları kaldırıldı
	}
}



