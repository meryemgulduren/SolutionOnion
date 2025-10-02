using Microsoft.EntityFrameworkCore;
using SO.Application.Abstractions.Services.ProposalModule;
using SO.Application.DTOs.ProposalModule.ProposalRisk;
using SO.Application.Repositories;
using SO.Domain.Entities.ProposalModule;

namespace SO.Persistence.Services.ProposalModule
{
    public class ProposalRiskService : IProposalRiskService
    {
        private readonly IWriteRepository<ProposalRisk> _writeRepository;
        private readonly IReadRepository<ProposalRisk> _readRepository;

        // Predefined risk categories from the image
        private static readonly string[] DefaultRiskTitles = new[]
        {
            "Planlama",
            "Projelendirme",
            "Yazılım",
            "Kurulum",
            "İmalat",
            "Mekanik",
            "Test&Devreye Alma",
            "Eğitim",
            "Teknik Destek"
        };

        public ProposalRiskService(
            IWriteRepository<ProposalRisk> writeRepository,
            IReadRepository<ProposalRisk> readRepository)
        {
            _writeRepository = writeRepository;
            _readRepository = readRepository;
        }

        public async Task<List<ProposalRiskDto>> GetRisksByProposalIdAsync(Guid proposalId)
        {
            var risks = await _readRepository.Table
                .Where(r => r.ProposalId == proposalId && !r.IsDeleted)
                .OrderBy(r => r.CreatedDate)
                .Select(r => new ProposalRiskDto
                {
                    Id = r.Id,
                    ProposalId = r.ProposalId,
                    Title = r.Title,
                    Description = r.Description,
                    IsApplicable = r.IsApplicable
                })
                .ToListAsync();

            return risks;
        }

        public async Task<ProposalRiskDto> CreateRiskAsync(ProposalRiskDto dto)
        {
            var risk = new ProposalRisk
            {
                Id = Guid.NewGuid(),
                ProposalId = dto.ProposalId,
                Title = dto.Title,
                Description = dto.Description,
                IsApplicable = dto.IsApplicable,
                CreatedDate = DateTime.UtcNow,
                ModifiedDate = DateTime.UtcNow
            };

            await _writeRepository.AddAsync(risk);
            await _writeRepository.SaveAsync();

            dto.Id = risk.Id;
            return dto;
        }

        public async Task<ProposalRiskDto> UpdateRiskAsync(ProposalRiskDto dto)
        {
            var risk = await _writeRepository.Table
                .FirstOrDefaultAsync(r => r.Id == dto.Id);

            if (risk == null)
                throw new Exception("Risk not found");

            risk.Title = dto.Title;
            risk.Description = dto.Description;
            risk.IsApplicable = dto.IsApplicable;
            risk.ModifiedDate = DateTime.UtcNow;

            _writeRepository.Update(risk);
            await _writeRepository.SaveAsync();

            return dto;
        }

        public async Task<bool> DeleteRiskAsync(Guid id)
        {
            var risk = await _writeRepository.Table
                .FirstOrDefaultAsync(r => r.Id == id);

            if (risk == null)
                return false;

            risk.IsDeleted = true;
            risk.ModifiedDate = DateTime.UtcNow;

            _writeRepository.Update(risk);
            await _writeRepository.SaveAsync();

            return true;
        }

        public async Task InitializeDefaultRisksAsync(Guid proposalId)
        {
            // Check if risks already exist
            var existingCount = await _readRepository.Table
                .CountAsync(r => r.ProposalId == proposalId && !r.IsDeleted);

            if (existingCount > 0)
                return; // Already initialized

            // Create default risks
            var risks = DefaultRiskTitles.Select(title => new ProposalRisk
            {
                Id = Guid.NewGuid(),
                ProposalId = proposalId,
                Title = title,
                Description = null,
                IsApplicable = null,
                CreatedDate = DateTime.UtcNow,
                ModifiedDate = DateTime.UtcNow
            }).ToList();

            foreach (var risk in risks)
            {
                await _writeRepository.AddAsync(risk);
            }

            await _writeRepository.SaveAsync();
        }
    }
} 