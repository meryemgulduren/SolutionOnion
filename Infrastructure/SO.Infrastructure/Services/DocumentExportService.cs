using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.EntityFrameworkCore;
using SO.Application.DTOs.ProposalModule;
using SO.Application.DTOs.ProposalModule.Proposal;
using SO.Application.DTOs.Dashboard;
using SO.Application.DTOs.AccountModule;
using SO.Application.DTOs.AccountModule.Account;
using SO.Application.Abstractions.Services;
using SO.Persistence.Contexts;
using System;
using System.IO;
using System.Linq;
using System.Text;
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
                .Include(p => p.Account)
                .FirstOrDefaultAsync(p => p.Id == proposalId);

            if (proposal == null)
                throw new ArgumentException("Proposal not found");
            return new ProposalExportDto
            {
                Id = proposal.Id,
                ProposalName = proposal.ProposalName,
                PreparedBy = proposal.PreparedBy,
                Status = proposal.Status.ToString(),
                ProposalDate = proposal.ProposalDate,
                ProjectDescription = proposal.ProjectDescription ?? "",
                ValidUntilDate = proposal.ValidUntilDate,
                TotalAmount = proposal.TotalAmount,
                Currency = proposal.Currency,
                Title = proposal.ProposalName ?? string.Empty,
                CompanyName = proposal.Account?.CompanyName ?? ""
            };
        }
        public async Task<byte[]> ExportProposalToWordAsync(Guid proposalId)
        {
            var proposalData = await GetProposalExportDataAsync(proposalId);
            // Yeni alanlar ve ilişkiler için entity'i de çekelim
            var proposalEntity = await _context.Proposals
                .Include(p => p.Account)
                .Include(p => p.CompetitionCompanies)
                .Include(p => p.BusinessPartners)
                .Include(p => p.ProposalRisks)
                .FirstOrDefaultAsync(p => p.Id == proposalId);
            
            var addressLine = "";
            if (proposalEntity != null && proposalEntity.AddressId.HasValue)
            {
                var addr = await _context.Addresses.FirstOrDefaultAsync(a => a.Id == proposalEntity.AddressId.Value);
                addressLine = addr?.AddressLine1 ?? "";
            }

            using (var memoryStream = new MemoryStream())
            {
                using (var wordDocument = WordprocessingDocument.Create(memoryStream, WordprocessingDocumentType.Document))
                {
                    var mainPart = wordDocument.AddMainDocumentPart();
                    mainPart.Document = new Document();
                    var body = mainPart.Document.AppendChild(new Body());

                    // Başlık
                    AddTitle(body, "PROJE TEKLİF RAPORU");
                    AddTitle(body, proposalData.ProposalName, 2);

                    // Proje Özeti
                    AddSectionTitle(body, "1. PROJE ÖZETİ");
                    AddParagraph(body, $"Teklif Tarihi: {proposalData.ProposalDate:dd.MM.yyyy}");
                    AddParagraph(body, $"Hazırlayan: {proposalData.PreparedBy}");
                    AddParagraph(body, $"Müşteri: {proposalData.CompanyName}");
                    AddParagraph(body, $"Durum: {GetStatusTurkish(proposalData.Status)}");
                    AddParagraph(body, $"Toplam Tutar: {proposalData.TotalAmount:N2} {proposalData.Currency}");
                    if (proposalData.ValidUntilDate.HasValue)
                        AddParagraph(body, $"Geçerlilik Tarihi: {proposalData.ValidUntilDate.Value:dd.MM.yyyy}");

                    // Proje Açıklaması
                    if (!string.IsNullOrEmpty(proposalData.ProjectDescription))
                    {
                        AddSectionTitle(body, "2. PROJE AÇIKLAMASI");
                        AddParagraph(body, proposalData.ProjectDescription);
                    }

                    // 3. RİSKLER VE VARSAYIMLAR
                    if (proposalEntity != null && proposalEntity.ProposalRisks != null && proposalEntity.ProposalRisks.Any())
                    {
                        AddSectionTitle(body, "3. RİSKLER VE VARSAYIMLAR");
                        
                        var riskCounter = 1;
                        foreach (var risk in proposalEntity.ProposalRisks.OrderBy(r => r.Title))
                        {
                            if (risk.IsApplicable == true)
                            {
                                AddParagraph(body, $"3.{riskCounter}. {risk.Title}");
                                if (!string.IsNullOrWhiteSpace(risk.Description))
                                {
                                    AddParagraph(body, $"   {risk.Description}");
                                }
                                riskCounter++;
                            }
                        }
                    }

                    // 4. GENEL TANIM
                    if (proposalEntity != null)
                    {
                        AddSectionTitle(body, "4. GENEL TANIM");
                        
                        // Genel bilgiler tablosu
                        var generalInfoData = new List<string[]>();
                        generalInfoData.Add(new[] { "Şirket Adı", proposalData.CompanyName ?? "" });
                        generalInfoData.Add(new[] { "Hazırlayan", proposalData.PreparedBy ?? "" });
                        if (!string.IsNullOrWhiteSpace(addressLine)) 
                            generalInfoData.Add(new[] { "Müşteri Adresi", addressLine });
                        if (proposalEntity.OfferDurationDays.HasValue) 
                            generalInfoData.Add(new[] { "Teklif Süresi", $"{proposalEntity.OfferDurationDays} gün" });
                        if (proposalEntity.DeliveryDurationDays.HasValue) 
                            generalInfoData.Add(new[] { "Teslimat Süresi", $"{proposalEntity.DeliveryDurationDays} gün" });
                        if (!string.IsNullOrWhiteSpace(proposalEntity.OfferOwner)) 
                            generalInfoData.Add(new[] { "Teklif Sahibi", proposalEntity.OfferOwner });
                        if (proposalEntity.QuantityValue.HasValue || !string.IsNullOrWhiteSpace(proposalEntity.QuantityUnit))
                            generalInfoData.Add(new[] { "Miktar", $"{proposalEntity.QuantityValue?.ToString() ?? ""} {proposalEntity.QuantityUnit}" });
                        
                        AddTable(body, new[] { "Alan", "Değer" }, generalInfoData);
                        
                        if (!string.IsNullOrWhiteSpace(proposalEntity.GeneralNote))
                        {
                            AddParagraph(body, "Genel Notlar:");
                            AddParagraph(body, proposalEntity.GeneralNote);
                        }
                    }

                    // 5. TİCARİ BİLGİLER
                    if (proposalEntity != null)
                    {
                        AddSectionTitle(body, "5. TİCARİ BİLGİLER");
                        
                        var commercialData = new List<string[]>();
                        if (proposalEntity.TargetPrice.HasValue)
                            commercialData.Add(new[] { "Hedef Fiyat", $"{proposalEntity.TargetPrice.Value:N2} {proposalData.Currency}" });
                        if (!string.IsNullOrWhiteSpace(proposalEntity.PaymentMethod))
                            commercialData.Add(new[] { "Ödeme Yöntemi", proposalEntity.PaymentMethod });
                        if (!string.IsNullOrWhiteSpace(proposalEntity.PaymentTerm))
                            commercialData.Add(new[] { "Ödeme Koşulları", proposalEntity.PaymentTerm });
                        commercialData.Add(new[] { "Toplam Tutar", $"{proposalData.TotalAmount:N2} {proposalData.Currency}" });
                        
                        if (commercialData.Count > 0)
                        {
                            AddTable(body, new[] { "Alan", "Değer" }, commercialData);
                        }
                        
                        if (!string.IsNullOrWhiteSpace(proposalEntity.CommercialNote))
                        {
                            AddParagraph(body, "Ticari Notlar:");
                            AddParagraph(body, proposalEntity.CommercialNote);
                        }
                    }

                    // 6. REKABET ŞİRKETLERİ
                    AddSectionTitle(body, "6. REKABET ŞİRKETLERİ");
                    
                    var competitionData = new List<string[]>();
                    if (proposalEntity != null && proposalEntity.CompetitionCompanies.Any())
                    {
                        competitionData = proposalEntity.CompetitionCompanies.Select(cc => new[]
                        {
                            cc.CompanyName ?? "",
                            cc.CompetedPrice?.ToString("N2") + " " + proposalData.Currency ?? "",
                            cc.Notes ?? ""
                        }).ToList();
                    }
                    
                    AddTable(body, new[] { "Şirket Adı", "Rekabet Fiyatı", "Notlar" }, competitionData);

                    // 7. İŞ ORTAKLARI
                    AddSectionTitle(body, "7. İŞ ORTAKLARI");
                    
                    var partnerData = new List<string[]>();
                    if (proposalEntity != null && proposalEntity.BusinessPartners.Any())
                    {
                        partnerData = proposalEntity.BusinessPartners.Select(bp => new[]
                        {
                            bp.PartnerName ?? "",
                            bp.Role ?? "",
                            bp.ContactInfo ?? "",
                            bp.Notes ?? ""
                        }).ToList();
                    }
                    
                    AddTable(body, new[] { "Ortak Adı", "Rol", "İletişim Bilgisi", "Notlar" }, partnerData);

                    // 8. TEKNİK BİLGİLER
                    AddSectionTitle(body, "8. TEKNİK BİLGİLER");
                    AddParagraph(body, "Bu bölüm proje ile ilgili teknik detayları içermektedir.");
                    if (!string.IsNullOrEmpty(proposalData.ProjectDescription))
                    {
                        AddParagraph(body, "Proje Detayları:");
                        AddParagraph(body, proposalData.ProjectDescription);
                    }

                    mainPart.Document.Save();
                }

                return memoryStream.ToArray();
            }
        }

        private string GetStatusTurkish(string status)
        {
            return status switch
            {
                "Draft" => "Taslak",
                "Submitted" => "Gönderildi",
                "Approved" => "Onaylandı",
                "Rejected" => "Reddedildi",
                "InProgress" => "Devam Ediyor",
                "Completed" => "Tamamlandı",
                _ => status
            };
        }

        private void AddTitle(Body body, string title, int level = 1)
        {
            var titleParagraph = new Paragraph();
            var titleRun = new Run();
            var titleText = new Text(title);

            var runProperties = new RunProperties();
            runProperties.Bold = new Bold();
            runProperties.FontSize = new FontSize() { Val = level == 1 ? "36" : "28" }; // Daha büyük font
            runProperties.Color = new Color() { Val = "2F5496" }; // Mavi renk
            // Font family removed for compatibility

            titleRun.RunProperties = runProperties;
            titleRun.Append(titleText);
            titleParagraph.Append(titleRun);

            var paragraphProperties = new ParagraphProperties();
            paragraphProperties.Justification = new Justification() { Val = JustificationValues.Center };
            paragraphProperties.SpacingBetweenLines = new SpacingBetweenLines() { After = "240" }; // Daha fazla boşluk
            titleParagraph.ParagraphProperties = paragraphProperties;

            body.Append(titleParagraph);
            body.Append(new Paragraph()); // Boş satır
        }
        private void AddSectionTitle(Body body, string title)
        {
            var titleParagraph = new Paragraph();
            var titleRun = new Run();
            var titleText = new Text(title);

            var runProperties = new RunProperties();
            runProperties.Bold = new Bold();
            runProperties.FontSize = new FontSize() { Val = "20" }; // Daha büyük font
            runProperties.Color = new Color() { Val = "1F4E79" }; // Koyu mavi renk
            // Font family removed for compatibility

            titleRun.RunProperties = runProperties;
            titleRun.Append(titleText);
            titleParagraph.Append(titleRun);

            var paragraphProperties = new ParagraphProperties();
            paragraphProperties.SpacingBetweenLines = new SpacingBetweenLines() { After = "120" }; // Daha fazla boşluk
            titleParagraph.ParagraphProperties = paragraphProperties;

            body.Append(titleParagraph);
        }

        private void AddParagraph(Body body, string text)
        {
            var paragraph = new Paragraph();
            var run = new Run();
            var textElement = new Text(text);

            var runProperties = new RunProperties();
            runProperties.FontSize = new FontSize() { Val = "14" }; // Daha büyük font
            // Font family removed for compatibility
            run.RunProperties = runProperties;

            run.Append(textElement);
            paragraph.Append(run);

            var paragraphProperties = new ParagraphProperties();
            paragraphProperties.SpacingBetweenLines = new SpacingBetweenLines() { After = "120" }; // Daha fazla boşluk
            paragraph.ParagraphProperties = paragraphProperties;

            body.Append(paragraph);
        }

        private void AddTable(Body body, string[] headers, System.Collections.Generic.List<string[]> rows)
        {
            // Debug: Check if headers are provided
            if (headers == null || headers.Length == 0)
            {
                AddParagraph(body, "HATA: Tablo için başlık sağlanmadı");
                return;
            }

            // If no data, create empty table with headers only
            if (rows == null)
            {
                rows = new List<string[]>();
            }

            var table = new Table();
            
            // Table properties
            var tableProps = new TableProperties();
            tableProps.Append(new TableBorders(
                new TopBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 12 },
                new BottomBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 12 },
                new LeftBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 12 },
                new RightBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 12 },
                new InsideHorizontalBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 8 },
                new InsideVerticalBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 8 }
            ));
            tableProps.Append(new TableWidth { Type = TableWidthUnitValues.Pct, Width = "5000" });
            table.Append(tableProps);

            // Header row - MUTLAKA EKLENİR
            var headerRow = new TableRow();
            foreach (var header in headers)
            {
                var headerCell = new TableCell();
                var headerParagraph = new Paragraph();
                var headerRun = new Run();
                var headerText = new Text(header ?? "N/A");

                // Header styling
                var headerRunProps = new RunProperties();
                headerRunProps.Append(new Bold());
                headerRunProps.Append(new FontSize() { Val = "16" });
                headerRunProps.Append(new Color() { Val = "FFFFFF" });
                headerRun.RunProperties = headerRunProps;

                headerRun.Append(headerText);
                headerParagraph.Append(headerRun);
                
                // Center alignment
                var headerParaProps = new ParagraphProperties();
                headerParaProps.Append(new Justification() { Val = JustificationValues.Center });
                headerParaProps.Append(new SpacingBetweenLines() { After = "120" });
                headerParagraph.ParagraphProperties = headerParaProps;
                
                headerCell.Append(headerParagraph);

                // Header cell properties
                var headerCellProps = new TableCellProperties();
                headerCellProps.Append(new TableCellWidth { Type = TableWidthUnitValues.Pct, Width = (5000 / headers.Length).ToString() });
                headerCellProps.Append(new Shading { Val = ShadingPatternValues.Clear, Color = "2F5496" });
                headerCellProps.Append(new TableCellVerticalAlignment { Val = TableVerticalAlignmentValues.Center });
                var cellMargin = new TableCellMargin();
                cellMargin.TopMargin = new TopMargin() { Width = "100", Type = TableWidthUnitValues.Dxa };
                cellMargin.BottomMargin = new BottomMargin() { Width = "100", Type = TableWidthUnitValues.Dxa };
                cellMargin.LeftMargin = new LeftMargin() { Width = "100", Type = TableWidthUnitValues.Dxa };
                cellMargin.RightMargin = new RightMargin() { Width = "100", Type = TableWidthUnitValues.Dxa };
                headerCellProps.Append(cellMargin);
                headerCell.TableCellProperties = headerCellProps;

                headerRow.Append(headerCell);
            }
            table.Append(headerRow);

            // Data rows - eğer veri varsa ekle
            if (rows.Count > 0)
            {
                foreach (var row in rows)
                {
                    var dataRow = new TableRow();
                    foreach (var cellValue in row)
                    {
                        var dataCell = new TableCell();
                        var dataParagraph = new Paragraph();
                        var dataRun = new Run();
                        var dataText = new Text(cellValue ?? "");

                        // Data styling
                        var dataRunProps = new RunProperties();
                        dataRunProps.Append(new FontSize() { Val = "14" });
                        dataRun.RunProperties = dataRunProps;

                        dataRun.Append(dataText);
                        dataParagraph.Append(dataRun);

                        // Data paragraph properties
                        var dataParaProps = new ParagraphProperties();
                        dataParaProps.Append(new SpacingBetweenLines() { After = "120" });
                        dataParagraph.ParagraphProperties = dataParaProps;

                        dataCell.Append(dataParagraph);

                        // Data cell properties
                        var dataCellProps = new TableCellProperties();
                        dataCellProps.Append(new TableCellWidth { Type = TableWidthUnitValues.Pct, Width = (5000 / headers.Length).ToString() });
                        dataCellProps.Append(new TableCellVerticalAlignment { Val = TableVerticalAlignmentValues.Center });
                        var dataCellMargin = new TableCellMargin();
                        dataCellMargin.TopMargin = new TopMargin() { Width = "100", Type = TableWidthUnitValues.Dxa };
                        dataCellMargin.BottomMargin = new BottomMargin() { Width = "100", Type = TableWidthUnitValues.Dxa };
                        dataCellMargin.LeftMargin = new LeftMargin() { Width = "100", Type = TableWidthUnitValues.Dxa };
                        dataCellMargin.RightMargin = new RightMargin() { Width = "100", Type = TableWidthUnitValues.Dxa };
                        dataCellProps.Append(dataCellMargin);
                        dataCell.TableCellProperties = dataCellProps;

                        dataRow.Append(dataCell);
                    }
                    table.Append(dataRow);
                }
            }
            else
            {
                // Eğer veri yoksa boş bir satır ekle
                var emptyRow = new TableRow();
                for (int i = 0; i < headers.Length; i++)
                {
                    var emptyCell = new TableCell();
                    var emptyParagraph = new Paragraph();
                    var emptyRun = new Run();
                    var emptyText = new Text("");

                    var emptyRunProps = new RunProperties();
                    emptyRunProps.Append(new FontSize() { Val = "14" });
                    emptyRun.RunProperties = emptyRunProps;

                    emptyRun.Append(emptyText);
                    emptyParagraph.Append(emptyRun);

                    var emptyParaProps = new ParagraphProperties();
                    emptyParaProps.Append(new SpacingBetweenLines() { After = "120" });
                    emptyParagraph.ParagraphProperties = emptyParaProps;

                    emptyCell.Append(emptyParagraph);

                    var emptyCellProps = new TableCellProperties();
                    emptyCellProps.Append(new TableCellWidth { Type = TableWidthUnitValues.Pct, Width = (5000 / headers.Length).ToString() });
                    emptyCellProps.Append(new TableCellVerticalAlignment { Val = TableVerticalAlignmentValues.Center });
                    var emptyCellMargin = new TableCellMargin();
                    emptyCellMargin.TopMargin = new TopMargin() { Width = "100", Type = TableWidthUnitValues.Dxa };
                    emptyCellMargin.BottomMargin = new BottomMargin() { Width = "100", Type = TableWidthUnitValues.Dxa };
                    emptyCellMargin.LeftMargin = new LeftMargin() { Width = "100", Type = TableWidthUnitValues.Dxa };
                    emptyCellMargin.RightMargin = new RightMargin() { Width = "100", Type = TableWidthUnitValues.Dxa };
                    emptyCellProps.Append(emptyCellMargin);
                    emptyCell.TableCellProperties = emptyCellProps;

                    emptyRow.Append(emptyCell);
                }
                table.Append(emptyRow);
            }
            
            body.Append(table);
            body.Append(new Paragraph()); // Add space after table
        }

        // New Export All methods
        public async Task<byte[]> ExportDashboardToExcelAsync(DashboardStatisticsDto dashboardData)
        {
            var csv = new StringBuilder();
            csv.AppendLine("Dashboard Statistics");
            csv.AppendLine("Metric,Value");
            csv.AppendLine($"Draft Proposals,{dashboardData.DraftProposalsCount}");
            csv.AppendLine($"Sent Proposals,{dashboardData.SentProposalsCount}");
            csv.AppendLine($"Approved Proposals,{dashboardData.ApprovedProposalsCount}");
            csv.AppendLine($"Rejected Proposals,{dashboardData.RejectedProposalsCount}");
            csv.AppendLine($"Cancelled Proposals,{dashboardData.CancelledProposalsCount}");
            csv.AppendLine($"Total Proposals,{dashboardData.DraftProposalsCount + dashboardData.SentProposalsCount + dashboardData.ApprovedProposalsCount + dashboardData.RejectedProposalsCount + dashboardData.CancelledProposalsCount}");
            csv.AppendLine($"Total Revenue,{dashboardData.TotalRevenue:C}");
            
            return Encoding.UTF8.GetBytes(csv.ToString());
        }

        public async Task<byte[]> ExportProposalsToExcelAsync(List<ListProposal> proposals)
        {
            var csv = new StringBuilder();
            csv.AppendLine("ID,Proposal Name,Status,Created Date,Prepared By");
            
            foreach (var proposal in proposals)
            {
                csv.AppendLine($"{proposal.Id},{proposal.ProposalName},{proposal.Status},{proposal.CreatedDate:yyyy-MM-dd},{proposal.PreparedBy ?? ""}");
            }
            
            return Encoding.UTF8.GetBytes(csv.ToString());
        }

        public async Task<byte[]> ExportAccountsToExcelAsync(List<ListAccount> accounts)
        {
            var csv = new StringBuilder();
            csv.AppendLine("ID,Company Name,Contact Person,Email,Phone,Is Active,Created Date");
            
            foreach (var account in accounts)
            {
                csv.AppendLine($"{account.Id},{account.CompanyName},{account.ContactPerson},{account.Email},{account.PhoneNumber ?? ""},{account.IsActive},{account.CreatedDate:yyyy-MM-dd}");
            }
            
            return Encoding.UTF8.GetBytes(csv.ToString());
        }

        public async Task<byte[]> ExportAllDataToExcelAsync(DashboardStatisticsDto dashboardData, List<ListProposal> proposals, List<ListAccount> accounts)
        {
            var csv = new StringBuilder();
            
            // Dashboard Statistics
            csv.AppendLine("=== DASHBOARD STATISTICS ===");
            csv.AppendLine("Metric,Value");
            csv.AppendLine($"Draft Proposals,{dashboardData.DraftProposalsCount}");
            csv.AppendLine($"Sent Proposals,{dashboardData.SentProposalsCount}");
            csv.AppendLine($"Approved Proposals,{dashboardData.ApprovedProposalsCount}");
            csv.AppendLine($"Rejected Proposals,{dashboardData.RejectedProposalsCount}");
            csv.AppendLine($"Cancelled Proposals,{dashboardData.CancelledProposalsCount}");
            csv.AppendLine($"Total Proposals,{dashboardData.DraftProposalsCount + dashboardData.SentProposalsCount + dashboardData.ApprovedProposalsCount + dashboardData.RejectedProposalsCount + dashboardData.CancelledProposalsCount}");
            csv.AppendLine($"Total Revenue,{dashboardData.TotalRevenue:C}");
            csv.AppendLine();
            
            // Proposals
            csv.AppendLine("=== PROPOSALS ===");
            csv.AppendLine("ID,Proposal Name,Status,Created Date,Prepared By");
            foreach (var proposal in proposals)
            {
                csv.AppendLine($"{proposal.Id},{proposal.ProposalName},{proposal.Status},{proposal.CreatedDate:yyyy-MM-dd},{proposal.PreparedBy ?? ""}");
            }
            csv.AppendLine();
            
            // Accounts
            csv.AppendLine("=== ACCOUNTS ===");
            csv.AppendLine("ID,Company Name,Contact Person,Email,Phone,Is Active,Created Date");
            foreach (var account in accounts)
            {
                csv.AppendLine($"{account.Id},{account.CompanyName},{account.ContactPerson},{account.Email},{account.PhoneNumber ?? ""},{account.IsActive},{account.CreatedDate:yyyy-MM-dd}");
            }
            
            return Encoding.UTF8.GetBytes(csv.ToString());
        }

        // PDF Export methods (placeholder implementations)
        public async Task<byte[]> ExportProposalToPdfAsync(Guid proposalId)
        {
            // TODO: Implement PDF export using iTextSharp or similar library
            throw new NotImplementedException("PDF export will be implemented in the next phase");
        }

        public async Task<byte[]> ExportDashboardToPdfAsync(DashboardStatisticsDto dashboardData)
        {
            // TODO: Implement PDF export using iTextSharp or similar library
            throw new NotImplementedException("PDF export will be implemented in the next phase");
        }

        public async Task<byte[]> ExportProposalsToPdfAsync(List<ListProposal> queries)
        {
            // TODO: Implement PDF export using iTextSharp or similar library
            throw new NotImplementedException("PDF export will be implemented in the next phase");
        }

        // CSV Export methods
        public async Task<byte[]> ExportProposalsToCsvAsync(List<ListProposal> proposals)
        {
            var csv = new StringBuilder();
            csv.AppendLine("ID,Proposal Name,Status,Created Date,Prepared By");
            
            foreach (var proposal in proposals)
            {
                csv.AppendLine($"{proposal.Id},{proposal.ProposalName},{proposal.Status},{proposal.CreatedDate:yyyy-MM-dd},{proposal.PreparedBy ?? ""}");
            }
            
            return Encoding.UTF8.GetBytes(csv.ToString());
        }

        public async Task<byte[]> ExportAccountsToCsvAsync(List<ListAccount> accounts)
        {
            var csv = new StringBuilder();
            csv.AppendLine("ID,Company Name,Contact Person,Email,Phone,Is Active,Created Date");
            
            foreach (var account in accounts)
            {
                csv.AppendLine($"{account.Id},{account.CompanyName},{account.ContactPerson},{account.Email},{account.PhoneNumber ?? ""},{account.IsActive},{account.CreatedDate:yyyy-MM-dd}");
            }
            
            return Encoding.UTF8.GetBytes(csv.ToString());
        }

        public async Task<byte[]> ExportAllProposalsToWordAsync(List<Guid> proposalIds)
        {
            using var memoryStream = new MemoryStream();
            using var wordDocument = WordprocessingDocument.Create(memoryStream, WordprocessingDocumentType.Document);
            
            var mainPart = wordDocument.AddMainDocumentPart();
            mainPart.Document = new Document();
            var body = mainPart.Document.AppendChild(new Body());

            // Title with improved styling
            AddTitle(body, "ALL PROPOSALS REPORT");

            // Get proposals data
            var proposals = await _context.Proposals
                .Include(p => p.Account)
                .Where(p => proposalIds.Contains(p.Id))
                .ToListAsync();

            // Create table data
            var headers = new[] { "Proposal Name", "Company", "Prepared By", "Date", "Status", "Amount" };
            var rows = proposals.Select(p => new[]
            {
                p.ProposalName ?? "",
                p.Account?.CompanyName ?? "",
                p.PreparedBy ?? "",
                p.ProposalDate.ToString("dd.MM.yyyy"),
                p.Status.ToString(),
                p.TotalAmount.ToString("N2") + " " + (p.Currency ?? "USD")
            }).ToList();

            // Use improved table method
            AddTable(body, headers, rows);

            wordDocument.Save();
            return memoryStream.ToArray();
        }

        public async Task<byte[]> ExportAllProposalsToExcelAsync(List<Guid> proposalIds)
        {
            // Get proposals data
            var proposals = await _context.Proposals
                .Include(p => p.Account)
                .Where(p => proposalIds.Contains(p.Id))
                .Select(p => new ListProposal
                {
                    Id = p.Id,
                    ProposalName = p.ProposalName,
                    CompanyName = p.Account != null ? p.Account.CompanyName : "",
                    PreparedBy = p.PreparedBy,
                    ProposalDate = p.ProposalDate,
                    Status = p.Status.ToString(),
                    TotalAmount = p.TotalAmount,
                    CreatedDate = p.CreatedDate
                })
                .ToListAsync();

            return await ExportProposalsToExcelAsync(proposals);
        }
    }
}

