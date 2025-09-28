using MediatR;
using SO.Application.Abstractions.Services.ProposalModule;
using SO.Application.Features.Queries.ProposalModule.CompetitionCompany.GetAllCompetitionCompany;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SO.Application.Features.Queries.ProposalModule.CompetitionCompany.GetAllCompetitionCompany
{
    public class GetAllCompetitionCompanyQueryHandler : IRequestHandler<GetAllCompetitionCompanyQueryRequest, GetAllCompetitionCompanyQueryResponse>
    {
        private readonly ICompetitionCompanyService _competitionCompanyService;

        public GetAllCompetitionCompanyQueryHandler(ICompetitionCompanyService competitionCompanyService)
        {
            _competitionCompanyService = competitionCompanyService;
        }

        public async Task<GetAllCompetitionCompanyQueryResponse> Handle(GetAllCompetitionCompanyQueryRequest request, CancellationToken cancellationToken)
        {
            var allCompetitionCompanies = await _competitionCompanyService.GetAllCompetitionCompaniesAsync();

            // Admin ise tüm competition company'leri göster
            if (request.IsAdmin)
            {
                return new() { Result = allCompetitionCompanies };
            }

            // User ise sadece kendi oluşturduğu competition company'leri göster
            if (!string.IsNullOrEmpty(request.CurrentUserId))
            {
                var filteredCompetitionCompanies = allCompetitionCompanies?.Where(cc => 
                    cc.CreatedById == request.CurrentUserId).ToList();
                return new() { Result = filteredCompetitionCompanies ?? new List<SO.Application.DTOs.ProposalModule.CompetitionCompany.CompetitionCompanyDto>() };
            }
            
            // Kullanıcı ID yoksa boş liste döndür
            return new() { Result = new List<SO.Application.DTOs.ProposalModule.CompetitionCompany.CompetitionCompanyDto>() };
        }
    }
}
