using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.EntityFrameworkCore;
using SO.Application.DTOs.ProposalModule;
using SO.Application.Interfaces.Services;
using SO.Persistence.Contexts;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SO.Infrastructure.Services
{
    public class DocumentExportService : IDocumentExportService
    {
        private readonly SODbContext _context;

        public DocumentExportService(SODbContext context)
        {
            _context = context;
        }

        public async Task<ProposalExportDto> GetProposalExportDataAsync(Guid proposalId)
        {
            var proposal = await _context.Proposals
                .Include(p => p.ResourceRequirements)
                .Include(p => p.Account)
                .FirstOrDefaultAsync(p => p.Id == proposalId);

            if (proposal == null)
                throw new ArgumentException("Proposal not found", nameof(proposalId));

            return new ProposalExportDto
            {
                Id = proposal.Id,
                Title = proposal.ProposalName,
                Description = proposal.ProjectDescription ?? "",
                CreatedDate = proposal.CreatedDate,
                Status = proposal.Status.ToString(),
                EstimatedBudget = proposal.TotalAmount,
                StartDate = null, // Bu bilgi entity'de yok
                EndDate = proposal.ValidUntilDate,
                ClientName = proposal.Account?.CompanyName ?? "",
                ProjectManager = proposal.PreparedBy,
                TechnicalLead = "", // Bu bilgi entity'de yok
                ResourceRequirements = proposal.ResourceRequirements.Select(rr => new ResourceRequirementExportDto
                {
                    ResourceType = rr.Resource,
                    SkillLevel = "N/A", // Bu bilgi entity'de yok
                    RequiredCount = 1, // Bu bilgi entity'de yok
                    HourlyRate = 0, // Bu bilgi entity'de yok
                    EstimatedHours = 0, // Bu bilgi entity'de yok
                    Description = rr.Description ?? "",
                    TotalCost = 0 // Hesaplanamıyor çünkü gerekli bilgiler yok
                }).ToList()
            };
        }

        public async Task<byte[]> ExportProposalToWordAsync(Guid proposalId)
        {
            var proposalData = await GetProposalExportDataAsync(proposalId);

            using (var stream = new MemoryStream())
            {
                using (var document = WordprocessingDocument.Create(stream, WordprocessingDocumentType.Document))
                {
                    var mainPart = document.AddMainDocumentPart();
                    mainPart.Document = new Document();
                    var body = mainPart.Document.AppendChild(new Body());

                    // Başlık
                    AddTitle(body, "PROJE TEKLİF RAPORU");
                    AddParagraph(body, "");

                    // Proje Bilgileri
                    AddHeading(body, "PROJE BİLGİLERİ");
                    AddParagraph(body, $"Proje Adı: {proposalData.Title}");
                    AddParagraph(body, $"Açıklama: {proposalData.Description}");
                    AddParagraph(body, $"Durum: {proposalData.Status}");
                    AddParagraph(body, $"Oluşturma Tarihi: {proposalData.CreatedDate:dd.MM.yyyy}");
                    AddParagraph(body, $"Tahmini Bütçe: {proposalData.EstimatedBudget:C}");
                    
                    if (proposalData.StartDate.HasValue)
                        AddParagraph(body, $"Başlangıç Tarihi: {proposalData.StartDate.Value:dd.MM.yyyy}");
                    
                    if (proposalData.EndDate.HasValue)
                        AddParagraph(body, $"Bitiş Tarihi: {proposalData.EndDate.Value:dd.MM.yyyy}");
                    
                    AddParagraph(body, $"Müşteri: {proposalData.ClientName}");
                    AddParagraph(body, $"Proje Yöneticisi: {proposalData.ProjectManager}");
                    AddParagraph(body, $"Teknik Lider: {proposalData.TechnicalLead}");
                    AddParagraph(body, "");

                    // Kaynak Gereksinimleri
                    AddHeading(body, "KAYNAK GEREKSİNİMLERİ");
                    
                    if (proposalData.ResourceRequirements.Any())
                    {
                        // Tablo oluştur
                        var table = CreateResourceRequirementsTable(proposalData.ResourceRequirements);
                        body.AppendChild(table);
                        
                        // Toplam maliyet
                        var totalCost = proposalData.ResourceRequirements.Sum(rr => rr.TotalCost);
                        AddParagraph(body, "");
                        AddParagraph(body, $"TOPLAM MALİYET: {totalCost:C}", true);
                    }
                    else
                    {
                        AddParagraph(body, "Henüz kaynak gereksinimi tanımlanmamıştır.");
                    }

                    // Alt bilgi
                    AddParagraph(body, "");
                    AddParagraph(body, "");
                    AddParagraph(body, $"Rapor Oluşturma Tarihi: {DateTime.Now:dd.MM.yyyy HH:mm}");
                }

                return stream.ToArray();
            }
        }

        private void AddTitle(Body body, string text)
        {
            var paragraph = new Paragraph();
            var run = new Run();
            var runProperties = new RunProperties();
            runProperties.Append(new Bold());
            runProperties.Append(new FontSize() { Val = "24" });
            runProperties.Append(new Justification() { Val = JustificationValues.Center });
            
            run.Append(runProperties);
            run.Append(new Text(text));
            paragraph.Append(run);
            
            var paragraphProperties = new ParagraphProperties();
            paragraphProperties.Append(new Justification() { Val = JustificationValues.Center });
            paragraph.Append(paragraphProperties);
            
            body.Append(paragraph);
        }

        private void AddHeading(Body body, string text)
        {
            var paragraph = new Paragraph();
            var run = new Run();
            var runProperties = new RunProperties();
            runProperties.Append(new Bold());
            runProperties.Append(new FontSize() { Val = "16" });
            
            run.Append(runProperties);
            run.Append(new Text(text));
            paragraph.Append(run);
            body.Append(paragraph);
        }

        private void AddParagraph(Body body, string text, bool bold = false)
        {
            var paragraph = new Paragraph();
            var run = new Run();
            
            if (bold)
            {
                var runProperties = new RunProperties();
                runProperties.Append(new Bold());
                run.Append(runProperties);
            }
            
            run.Append(new Text(text));
            paragraph.Append(run);
            body.Append(paragraph);
        }

        private Table CreateResourceRequirementsTable(System.Collections.Generic.List<ResourceRequirementExportDto> resources)
        {
            var table = new Table();

            // Tablo özellikleri
            var tableProperties = new TableProperties();
            tableProperties.Append(new TableBorders(
                new TopBorder() { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 12 },
                new BottomBorder() { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 12 },
                new LeftBorder() { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 12 },
                new RightBorder() { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 12 },
                new InsideHorizontalBorder() { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 12 },
                new InsideVerticalBorder() { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 12 }
            ));
            table.Append(tableProperties);

            // Başlık satırı
            var headerRow = new TableRow();
            headerRow.Append(CreateHeaderCell("Kaynak Türü"));
            headerRow.Append(CreateHeaderCell("Yetenek Seviyesi"));
            headerRow.Append(CreateHeaderCell("Gerekli Sayı"));
            headerRow.Append(CreateHeaderCell("Saatlik Ücret"));
            headerRow.Append(CreateHeaderCell("Tahmini Saat"));
            headerRow.Append(CreateHeaderCell("Açıklama"));
            headerRow.Append(CreateHeaderCell("Toplam Maliyet"));
            table.Append(headerRow);

            // Veri satırları
            foreach (var resource in resources)
            {
                var row = new TableRow();
                row.Append(CreateCell(resource.ResourceType));
                row.Append(CreateCell(resource.SkillLevel));
                row.Append(CreateCell(resource.RequiredCount.ToString()));
                row.Append(CreateCell(resource.HourlyRate.ToString("C")));
                row.Append(CreateCell(resource.EstimatedHours.ToString()));
                row.Append(CreateCell(resource.Description));
                row.Append(CreateCell(resource.TotalCost.ToString("C")));
                table.Append(row);
            }

            return table;
        }

        private TableCell CreateHeaderCell(string text)
        {
            var cell = new TableCell();
            var paragraph = new Paragraph();
            var run = new Run();
            var runProperties = new RunProperties();
            runProperties.Append(new Bold());
            
            run.Append(runProperties);
            run.Append(new Text(text));
            paragraph.Append(run);
            cell.Append(paragraph);
            
            var cellProperties = new TableCellProperties();
            cellProperties.Append(new Shading() { Val = ShadingPatternValues.Clear, Fill = "CCCCCC" });
            cell.Append(cellProperties);
            
            return cell;
        }

        private TableCell CreateCell(string text)
        {
            var cell = new TableCell();
            var paragraph = new Paragraph();
            var run = new Run();
            run.Append(new Text(text ?? ""));
            paragraph.Append(run);
            cell.Append(paragraph);
            return cell;
        }
    }
}