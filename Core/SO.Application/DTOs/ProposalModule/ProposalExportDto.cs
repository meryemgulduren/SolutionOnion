using System;
using System.Collections.Generic;

namespace SO.Application.DTOs.ProposalModule
{
    public class ProposalExportDto
    {
        // Project Summary Bilgileri
        public Guid Id { get; set; }
        public string ProposalName { get; set; } = string.Empty;
        public string PreparedBy { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime ProposalDate { get; set; }
        public string ProjectDescription { get; set; } = string.Empty;
        public DateTime? ValidUntilDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Currency { get; set; } = "TRY";
        public string Title { get; set; } = string.Empty;
        // Client Information
        public string CompanyName { get; set; } = string.Empty;
    }
}