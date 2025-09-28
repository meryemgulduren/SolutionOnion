using SO.Application.Abstractions.Services.ProposalModule;
using SO.Application.DTOs.ProposalModule.CompetitionCompany;
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
    public class CompetitionCompanyService : ICompetitionCompanyService
    {
        private readonly IReadRepository<CompetitionCompany> _competitionCompanyReadRepository;
        private readonly IWriteRepository<CompetitionCompany> _competitionCompanyWriteRepository;
        private readonly UserManager<AppUser> _userManager;

        public CompetitionCompanyService(
            IReadRepository<CompetitionCompany> competitionCompanyReadRepository,
            IWriteRepository<CompetitionCompany> competitionCompanyWriteRepository,
            UserManager<AppUser> userManager)
        {
            _competitionCompanyReadRepository = competitionCompanyReadRepository;
            _competitionCompanyWriteRepository = competitionCompanyWriteRepository;
            _userManager = userManager;
        }

        public async Task CreateCompetitionCompanyAsync(CreateCompetitionCompany createCompetitionCompany)
        {
            await _competitionCompanyWriteRepository.AddAsync(new()
            {
                ProposalId = createCompetitionCompany.ProposalId,
                CompanyName = createCompetitionCompany.CompanyName,
                CompetedPrice = createCompetitionCompany.CompetedPrice,
                Notes = createCompetitionCompany.Notes
            });
            await _competitionCompanyWriteRepository.SaveAsync();
        }

        public async Task UpdateCompetitionCompanyAsync(UpdateCompetitionCompany updateCompetitionCompany)
        {
            var competitionCompany = await _competitionCompanyReadRepository.GetByIdAsync(updateCompetitionCompany.Id.ToString());
            if (competitionCompany != null)
            {
                competitionCompany.ProposalId = updateCompetitionCompany.ProposalId;
                competitionCompany.CompanyName = updateCompetitionCompany.CompanyName;
                competitionCompany.CompetedPrice = updateCompetitionCompany.CompetedPrice;
                competitionCompany.Notes = updateCompetitionCompany.Notes;
                _competitionCompanyWriteRepository.Update(competitionCompany);
                await _competitionCompanyWriteRepository.SaveAsync();
            }
        }

        public async Task DeleteCompetitionCompanyAsync(string id)
        {
            await _competitionCompanyWriteRepository.RemoveAsync(id);
            await _competitionCompanyWriteRepository.SaveAsync();
        }

        public async Task<CompetitionCompanyDto> GetCompetitionCompanyByIdAsync(string id)
        {
            var competitionCompany = await _competitionCompanyReadRepository.GetByIdAsync(id);
            
            if (competitionCompany == null)
            {
                throw new ArgumentException($"CompetitionCompany with ID {id} not found.");
            }
            
            return new CompetitionCompanyDto
            {
                Id = competitionCompany.Id,
                ProposalId = competitionCompany.ProposalId,
                CompanyName = competitionCompany.CompanyName,
                CompetedPrice = competitionCompany.CompetedPrice,
                Notes = competitionCompany.Notes,
                CreatedDate = competitionCompany.CreatedDate,
                ModifiedDate = competitionCompany.ModifiedDate
            };
        }

        public async Task<List<CompetitionCompanyDto>> GetAllCompetitionCompaniesAsync()
        {
            var competitionCompanies = _competitionCompanyReadRepository.GetAll(false).ToList();
            System.Diagnostics.Debug.WriteLine($"CompetitionCompanyService: Found {competitionCompanies.Count} competition companies in database");
            
            var result = new List<CompetitionCompanyDto>();
            
            foreach (var competitionCompany in competitionCompanies)
            {
                var createdBy = "Unknown";
                if (!string.IsNullOrEmpty(competitionCompany.CreatedById))
                {
                    var user = await _userManager.FindByIdAsync(competitionCompany.CreatedById);
                    if (user != null)
                    {
                        createdBy = user.UserName ?? user.Email ?? "Unknown";
                    }
                }

                result.Add(new CompetitionCompanyDto
                {
                    Id = competitionCompany.Id,
                    ProposalId = competitionCompany.ProposalId,
                    CompanyName = competitionCompany.CompanyName,
                    CompetedPrice = competitionCompany.CompetedPrice,
                    Notes = competitionCompany.Notes,
                    CreatedDate = competitionCompany.CreatedDate,
                    ModifiedDate = competitionCompany.ModifiedDate,
                    CreatedById = competitionCompany.CreatedById,
                    CreatedBy = createdBy
                });
            }
            
            System.Diagnostics.Debug.WriteLine($"CompetitionCompanyService: Returning {result.Count} competition companies");
            return result;
        }

        public async Task<List<CompetitionCompanyDto>> GetCompetitionCompaniesByProposalIdAsync(string proposalId)
        {
            var competitionCompanies = _competitionCompanyReadRepository.GetAll(false)
                .Where(cc => cc.ProposalId.ToString() == proposalId)
                .ToList();
            
            var result = new List<CompetitionCompanyDto>();
            
            foreach (var competitionCompany in competitionCompanies)
            {
                var createdBy = "Unknown";
                if (!string.IsNullOrEmpty(competitionCompany.CreatedById))
                {
                    var user = await _userManager.FindByIdAsync(competitionCompany.CreatedById);
                    if (user != null)
                    {
                        createdBy = user.UserName ?? user.Email ?? "Unknown";
                    }
                }

                result.Add(new CompetitionCompanyDto
                {
                    Id = competitionCompany.Id,
                    ProposalId = competitionCompany.ProposalId,
                    CompanyName = competitionCompany.CompanyName,
                    CompetedPrice = competitionCompany.CompetedPrice,
                    Notes = competitionCompany.Notes,
                    CreatedDate = competitionCompany.CreatedDate,
                    ModifiedDate = competitionCompany.ModifiedDate,
                    CreatedById = competitionCompany.CreatedById,
                    CreatedBy = createdBy
                });
            }
            
            return result;
        }
    }
}
