using SO.Application.DTOs.ProposalModule.CompetitionCompany;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SO.Application.Abstractions.Services.ProposalModule
{
    public interface ICompetitionCompanyService
    {
        Task CreateCompetitionCompanyAsync(CreateCompetitionCompany createCompetitionCompany);
        Task UpdateCompetitionCompanyAsync(UpdateCompetitionCompany updateCompetitionCompany);
        Task DeleteCompetitionCompanyAsync(string id);
        Task<List<CompetitionCompanyDto>> GetAllCompetitionCompaniesAsync();
        Task<CompetitionCompanyDto> GetCompetitionCompanyByIdAsync(string id);
        Task<List<CompetitionCompanyDto>> GetCompetitionCompaniesByProposalIdAsync(string proposalId);
    }
}
