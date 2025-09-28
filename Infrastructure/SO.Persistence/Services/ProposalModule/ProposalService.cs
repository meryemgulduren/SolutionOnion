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
        private readonly IAddressService _addressService;

        public ProposalService(IWriteRepository<Proposal> proposalWriteRepository, IReadRepository<Proposal> proposalReadRepository, IAddressService addressService)
        {
            _proposalWriteRepository = proposalWriteRepository;
            _proposalReadRepository = proposalReadRepository;
            _addressService = addressService;
        }
        public async Task UpdateProposalSummaryAsync(UpdateProposalSummary summary)
        {
            // TÜM İŞLEM write-repo context üzerinden
            var proposal = await _proposalWriteRepository.Table
                .FirstOrDefaultAsync(p => p.Id.ToString() == summary.ProposalId);

            if (proposal == null) return;

            // Ana alanlar
            proposal.ProjectDescription = summary.ProjectDescription;
            proposal.OfferDurationDays = summary.OfferDurationDays;
            proposal.DeliveryDurationDays = summary.DeliveryDurationDays;
            proposal.OfferOwner = summary.OfferOwner;
            proposal.QuantityValue = summary.QuantityValue;
            proposal.QuantityUnit = summary.QuantityUnit;
            proposal.GeneralNote = summary.GeneralNote;

            if (!string.IsNullOrWhiteSpace(summary.AddressId) && Guid.TryParse(summary.AddressId, out var addrId))
                proposal.AddressId = addrId;
            else
                proposal.AddressId = null;

            // Ticari alanlar
            proposal.TargetPrice = summary.TargetPrice;
            proposal.PaymentMethod = summary.PaymentMethod;
            proposal.PaymentTerm = summary.PaymentTerm;
            proposal.CommercialNote = summary.CommercialNote;
            proposal.ValidUntilDate = summary.ValidUntilDate;

            // (İsteğe bağlı) explicit update
            _proposalWriteRepository.Update(proposal);

            await _proposalWriteRepository.SaveAsync();
        }


        public async Task<string> CreateProposalAsync(CreateProposal createProposal)
        {
            try
            {
                var newProposal = new Proposal
                {
                    AccountId = Guid.Parse(createProposal.AccountId),
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
                ProposalName = proposal.ProposalName,
                AccountId = proposal.AccountId,
                CompanyName = proposal.Account.CompanyName,
                Status = proposal.Status.ToString(),
                ProposalDate = proposal.ProposalDate,
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


