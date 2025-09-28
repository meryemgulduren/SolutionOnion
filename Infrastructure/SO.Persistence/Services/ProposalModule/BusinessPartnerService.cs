using SO.Application.Abstractions.Services.ProposalModule;
using SO.Application.DTOs.ProposalModule.BusinessPartner;
using SO.Application.Repositories;
using SO.Domain.Entities.ProposalModule;
using SO.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SO.Persistence.Services.ProposalModule
{
    public class BusinessPartnerService : IBusinessPartnerService
    {
        private readonly IReadRepository<BusinessPartner> _businessPartnerReadRepository;
        private readonly IWriteRepository<BusinessPartner> _businessPartnerWriteRepository;
        private readonly UserManager<AppUser> _userManager;

        public BusinessPartnerService(
            IReadRepository<BusinessPartner> businessPartnerReadRepository,
            IWriteRepository<BusinessPartner> businessPartnerWriteRepository,
            UserManager<AppUser> userManager)
        {
            _businessPartnerReadRepository = businessPartnerReadRepository;
            _businessPartnerWriteRepository = businessPartnerWriteRepository;
            _userManager = userManager;
        }

        public async Task CreateBusinessPartnerAsync(CreateBusinessPartner createBusinessPartner)
        {
            await _businessPartnerWriteRepository.AddAsync(new()
            {
                ProposalId = createBusinessPartner.ProposalId,
                PartnerName = createBusinessPartner.PartnerName,
                Role = createBusinessPartner.Role,
                ContactInfo = createBusinessPartner.ContactInfo,
                Notes = createBusinessPartner.Notes
            });
            await _businessPartnerWriteRepository.SaveAsync();
        }

        public async Task UpdateBusinessPartnerAsync(UpdateBusinessPartner updateBusinessPartner)
        {
            var businessPartner = await _businessPartnerReadRepository.GetByIdAsync(updateBusinessPartner.Id.ToString());
            if (businessPartner != null)
            {
                businessPartner.ProposalId = updateBusinessPartner.ProposalId;
                businessPartner.PartnerName = updateBusinessPartner.PartnerName;
                businessPartner.Role = updateBusinessPartner.Role;
                businessPartner.ContactInfo = updateBusinessPartner.ContactInfo;
                businessPartner.Notes = updateBusinessPartner.Notes;
                _businessPartnerWriteRepository.Update(businessPartner);
                await _businessPartnerWriteRepository.SaveAsync();
            }
        }

        public async Task DeleteBusinessPartnerAsync(string id)
        {
            await _businessPartnerWriteRepository.RemoveAsync(id);
            await _businessPartnerWriteRepository.SaveAsync();
        }

        public async Task<BusinessPartnerDto> GetBusinessPartnerByIdAsync(string id)
        {
            var businessPartner = await _businessPartnerReadRepository.GetByIdAsync(id);
            
            if (businessPartner == null)
            {
                throw new ArgumentException($"BusinessPartner with ID {id} not found.");
            }
            
            return new BusinessPartnerDto
            {
                Id = businessPartner.Id,
                ProposalId = businessPartner.ProposalId,
                PartnerName = businessPartner.PartnerName,
                Role = businessPartner.Role,
                ContactInfo = businessPartner.ContactInfo,
                Notes = businessPartner.Notes,
                CreatedDate = businessPartner.CreatedDate,
                ModifiedDate = businessPartner.ModifiedDate
            };
        }

        public async Task<List<BusinessPartnerDto>> GetAllBusinessPartnersAsync()
        {
            var businessPartners = _businessPartnerReadRepository.GetAll(false).ToList();
            System.Diagnostics.Debug.WriteLine($"BusinessPartnerService: Found {businessPartners.Count} business partners in database");
            
            var result = new List<BusinessPartnerDto>();
            
            foreach (var businessPartner in businessPartners)
            {
                var createdBy = "Unknown";
                if (!string.IsNullOrEmpty(businessPartner.CreatedById))
                {
                    var user = await _userManager.FindByIdAsync(businessPartner.CreatedById);
                    if (user != null)
                    {
                        createdBy = user.UserName ?? user.Email ?? "Unknown";
                    }
                }

                result.Add(new BusinessPartnerDto
                {
                    Id = businessPartner.Id,
                    ProposalId = businessPartner.ProposalId,
                    PartnerName = businessPartner.PartnerName,
                    Role = businessPartner.Role,
                    ContactInfo = businessPartner.ContactInfo,
                    Notes = businessPartner.Notes,
                    CreatedDate = businessPartner.CreatedDate,
                    ModifiedDate = businessPartner.ModifiedDate,
                    CreatedById = businessPartner.CreatedById,
                    CreatedBy = createdBy
                });
            }
            
            System.Diagnostics.Debug.WriteLine($"BusinessPartnerService: Returning {result.Count} business partners");
            return result;
        }

        public async Task<List<BusinessPartnerDto>> GetBusinessPartnersByProposalIdAsync(string proposalId)
        {
            var businessPartners = _businessPartnerReadRepository.GetAll(false)
                .Where(bp => bp.ProposalId.ToString() == proposalId)
                .ToList();
            
            var result = new List<BusinessPartnerDto>();
            
            foreach (var businessPartner in businessPartners)
            {
                var createdBy = "Unknown";
                if (!string.IsNullOrEmpty(businessPartner.CreatedById))
                {
                    var user = await _userManager.FindByIdAsync(businessPartner.CreatedById);
                    if (user != null)
                    {
                        createdBy = user.UserName ?? user.Email ?? "Unknown";
                    }
                }

                result.Add(new BusinessPartnerDto
                {
                    Id = businessPartner.Id,
                    ProposalId = businessPartner.ProposalId,
                    PartnerName = businessPartner.PartnerName,
                    Role = businessPartner.Role,
                    ContactInfo = businessPartner.ContactInfo,
                    Notes = businessPartner.Notes,
                    CreatedDate = businessPartner.CreatedDate,
                    ModifiedDate = businessPartner.ModifiedDate,
                    CreatedById = businessPartner.CreatedById,
                    CreatedBy = createdBy
                });
            }
            
            return result;
        }
    }
}
