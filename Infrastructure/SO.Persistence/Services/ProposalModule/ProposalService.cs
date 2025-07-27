using Microsoft.EntityFrameworkCore;
using SO.Application.Abstractions.Services.ProposalModule;
using SO.Application.DTOs.ProposalModule.BusinessObjective;
using SO.Application.DTOs.ProposalModule.CriticalSuccessFactor;
using SO.Application.DTOs.ProposalModule.Milestone;
using SO.Application.DTOs.ProposalModule.Proposal;
using SO.Application.DTOs.ProposalModule.ProposalItem;
using SO.Application.DTOs.ProposalModule.ResourceRequirement;
using SO.Application.DTOs.ProposalModule.SuccessCriterion;
using SO.Application.Repositories.ProposalModule;
using SO.Domain.Entities.ProposalModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SO.Persistence.Services.ProposalModule
{
    public class ProposalService : IProposalService
    {
        private readonly IProposalWriteRepository _proposalWriteRepository;
        private readonly IProposalReadRepository _proposalReadRepository;

        public ProposalService(IProposalWriteRepository proposalWriteRepository, IProposalReadRepository proposalReadRepository)
        {
            _proposalWriteRepository = proposalWriteRepository;
            _proposalReadRepository = proposalReadRepository;
        }
        public async Task UpdateProposalSummaryAsync(UpdateProposalSummary summary)
        {
            // Önce ilgili teklifi ve mevcut hedeflerini veritabanından çekiyoruz.
            var proposal = await _proposalReadRepository.GetWhere(p => p.Id.ToString() == summary.ProposalId)
                .Include(p => p.BusinessObjectives)
                .FirstOrDefaultAsync();

            if (proposal != null)
            {
                // Ana alanları güncelle
                proposal.ProjectDescription = summary.ProjectDescription;
                proposal.StatementOfNeed = summary.StatementOfNeed;

                // Mevcut hedefleri temizle
                proposal.BusinessObjectives.Clear();

                // Formdan gelen yeni hedefleri ekle
                foreach (var objectiveDto in summary.BusinessObjectives)
                {
                    proposal.BusinessObjectives.Add(new Domain.Entities.ProposalModule.BusinessObjective
                    {
                        Objective = objectiveDto.Objective,
                        Alignment = objectiveDto.Alignment,

                    });
                }

                // Değişiklikleri kaydet
                await _proposalWriteRepository.SaveAsync();
            }
        }

        public async Task<string> CreateProposalAsync(CreateProposal createProposal)
        {
            var newProposal = new Proposal
            {
                AccountId = Guid.Parse(createProposal.AccountId),
                ProposalName = createProposal.ProposalName,
                PreparedBy = createProposal.PreparedBy,
                ProjectDescription = createProposal.ProjectDescription, // YENİ
                StatementOfNeed = createProposal.StatementOfNeed,       // YENİ
                ProposalDate = DateTime.UtcNow,
                Status = ProposalStatus.Draft
            };

            // Gelen DTO'daki BusinessObjectives listesini Entity'ye çevirip ekliyoruz
            foreach (var objective in createProposal.BusinessObjectives)
            {
                newProposal.BusinessObjectives.Add(new BusinessObjective
                {
                    Objective = objective.Objective,
                    Alignment = objective.Alignment
                });
            }

            // Gelen DTO'daki ProposalItems listesini Entity'ye çevirip ekliyoruz
            foreach (var item in createProposal.ProposalItems)
            {
                newProposal.ProposalItems.Add(new ProposalItem
                {
                    Name = item.Name,
                    Quantity = item.Quantity,
                    Unit = item.Unit,
                    UnitPrice = item.UnitPrice
                });
            }

            newProposal.TotalAmount = newProposal.ProposalItems.Sum(pi => pi.TotalPrice);

            await _proposalWriteRepository.AddAsync(newProposal);
            await _proposalWriteRepository.SaveAsync();

            return newProposal.Id.ToString(); // YENİ: Oluşturulan teklifin ID'sini döndürüyoruz.
        }

        public async Task DeleteProposalAsync(string id)
        {
            await _proposalWriteRepository.RemoveAsync(id);
            await _proposalWriteRepository.SaveAsync();
        }

        public async Task<List<ListProposal>> GetAllProposalsAsync()
        {
            return await _proposalReadRepository.GetAll(false)
                .Include(p => p.Account)
                .Select(p => new ListProposal
                {
                    Id = p.Id,
                    ProposalName = p.ProposalName,
                    CompanyName = p.Account.CompanyName,
                    ProposalDate = p.ProposalDate,
                    TotalAmount = p.TotalAmount,
                    Status = p.Status.ToString()
                }).ToListAsync();
        }

        public async Task<SingleProposal> GetProposalByIdAsync(string id)
        {
            var proposal = await _proposalReadRepository.GetWhere(p => p.Id.ToString() == id)
                .Include(p => p.Account)
                .Include(p => p.BusinessObjectives) // YENİ
                .Include(p => p.ProposalItems)
                .Include(p => p.Milestones)
                .Include(p => p.SuccessCriteria)
                .Include(p => p.CriticalSuccessFactors)
                .FirstOrDefaultAsync();

            if (proposal == null)
                return null;

            return new SingleProposal
            {
                Id = proposal.Id,
                ProposalName = proposal.ProposalName,
                CompanyName = proposal.Account.CompanyName,
                Status = proposal.Status.ToString(),
                ProposalDate = proposal.ProposalDate,
                ProjectDescription = proposal.ProjectDescription, // YENİ
                StatementOfNeed = proposal.StatementOfNeed,       // YENİ
                TotalAmount = proposal.TotalAmount,
                Currency = proposal.Currency,
                ProjectApproach = proposal.ProjectApproach,
               
                Phasing = proposal.Phasing,
                OutsourcingPlans = proposal.OutsourcingPlans,
                Interoperability = proposal.Interoperability,

                BusinessObjectives = proposal.BusinessObjectives.Select(bo => new ListBusinessObjective
                {
                    Objective = bo.Objective,
                    Alignment = bo.Alignment
                }).ToList(), // YENİ

                ProposalItems = proposal.ProposalItems.Select(pi => new ListProposalItem
                {
                    Id = pi.Id,
                    Name = pi.Name,
                    Quantity = pi.Quantity,
                    Unit = pi.Unit,
                    UnitPrice = pi.UnitPrice,
                    TotalPrice = pi.TotalPrice
                }).ToList(),

                Milestones = proposal.Milestones.Select(m => new ListMilestone
                {
                    Name = m.Name,
                    PlannedCompletionDate = m.PlannedCompletionDate,
                    IsCompleted = m.IsCompleted
                }).ToList(),

              

                CriticalSuccessFactors = proposal.CriticalSuccessFactors.Select(csf => new ListCriticalSuccessFactor
                {
                    Description = csf.Description
                }).ToList(),
               
                ResourceRequirements = proposal.ResourceRequirements
                .Select(r => new ListResourceRequirement
                {
                 Resource = r.Resource,
                 Description = r.Description
                 }).ToList(),

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
        public async Task UpdateProposalInitiatorSponsorAsync(UpdateProposalInitiatorSponsor initiatorSponsor)
        {
            var proposal = await _proposalReadRepository.GetWhere(p => p.Id.ToString() == initiatorSponsor.ProposalId)
                .Include(p => p.ProjectStakeholders)
                .FirstOrDefaultAsync();

            if (proposal != null)
            {
                proposal.ProjectStakeholders.Clear();
                foreach (var stakeholderDto in initiatorSponsor.Stakeholders)
                {
                    proposal.ProjectStakeholders.Add(new Domain.Entities.ProposalModule.ProjectStakeholder
                    {
                        Role = stakeholderDto.Role,
                        Name = stakeholderDto.Name,
                        Responsibilities = stakeholderDto.Responsibilities,
                        ProposalId = proposal.Id
                    });
                }
                await _proposalWriteRepository.SaveAsync();
            }
        }
        public async Task UpdateProposalCustomersAndDeliverablesAsync(UpdateProposalCustomersAndDeliverables dto)
        {
            var proposal = await _proposalReadRepository.GetWhere(p => p.Id.ToString() == dto.ProposalId)
                .Include(p => p.CustomerBeneficiaries)
                .Include(p => p.Milestones)
                .FirstOrDefaultAsync();

            if (proposal != null)
            {
                // Mevcut listeleri temizle
                proposal.CustomerBeneficiaries.Clear();
                proposal.Milestones.Clear();

                // Formdan gelen yeni Customer/Beneficiary'leri ekle
                foreach (var beneficiaryDto in dto.CustomerBeneficiaries)
                {
                    proposal.CustomerBeneficiaries.Add(new Domain.Entities.ProposalModule.CustomerBeneficiary
                    {
                        Beneficiary = beneficiaryDto.Beneficiary,
                        NeedsAndConcern = beneficiaryDto.NeedsAndConcern,
                         ProposalId = proposal.Id
                    });
                }

                // Formdan gelen yeni Deliverable'ları (Milestone) ekle
                foreach (var milestoneDto in dto.Milestones)
                {
                    proposal.Milestones.Add(new Domain.Entities.ProposalModule.Milestone
                    {
                        Name = milestoneDto.Name,
                        Description = milestoneDto.Description,
                        PlannedCompletionDate = milestoneDto.PlannedCompletionDate,
                        ProposalId = proposal.Id
                    });
                }

                await _proposalWriteRepository.SaveAsync();
            }

        }
        public async Task UpdateProposalApproachAsync(UpdateProposalApproach dto)
        {
            var proposal = await _proposalReadRepository.GetWhere(p => p.Id.ToString() == dto.ProposalId)
                .Include(p => p.Milestones)
                .FirstOrDefaultAsync();

            if (proposal != null)
            {
                // Metin alanlarını güncelle
                proposal.Phasing = dto.Phasing;
                proposal.OutsourcingPlans = dto.OutsourcingPlans;
                proposal.Interoperability = dto.Interoperability;

                // Mevcut Timeframe (Milestones) listesini temizle
                proposal.Milestones.Clear();

                // Formdan gelen yeni Timeframe (Milestones) satırlarını ekle
                if (dto.Milestones != null)
                {
                    foreach (var milestoneDto in dto.Milestones)
                    {
                        proposal.Milestones.Add(new Domain.Entities.ProposalModule.Milestone
                        {
                            Name = milestoneDto.Name,
                            Description = milestoneDto.Description,
                            PlannedCompletionDate = milestoneDto.PlannedCompletionDate,
                            ProposalId = proposal.Id
                        });
                    }
                }

                await _proposalWriteRepository.SaveAsync();
            }
        }
        public async Task UpdateProposalResourceRequirementsAsync(UpdateProposalResourceRequirements dto)
        {
            var proposal = await _proposalReadRepository.GetWhere(p => p.Id.ToString() == dto.ProposalId)
                .Include(p => p.ResourceRequirements)
                .FirstOrDefaultAsync();

            if (proposal != null)
            {
                proposal.ResourceRequirements.Clear();
                if (dto.ResourceRequirements != null)
                {
                    foreach (var requirementDto in dto.ResourceRequirements)
                    {
                        proposal.ResourceRequirements.Add(new Domain.Entities.ProposalModule.ResourceRequirement
                        {
                            Resource = requirementDto.Resource,
                            Description = requirementDto.Description,
                            ProposalId = proposal.Id // 💥 EKLENEN SATIR
                        });
                    }
                }
                await _proposalWriteRepository.SaveAsync();
            }
        }
    }
}


