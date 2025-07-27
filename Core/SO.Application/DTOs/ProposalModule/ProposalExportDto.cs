using System;
using System.Collections.Generic;

namespace SO.Application.DTOs.ProposalModule
{
    public class ProposalExportDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Status { get; set; }
        public decimal EstimatedBudget { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string ClientName { get; set; }
        public string ProjectManager { get; set; }
        public string TechnicalLead { get; set; }
        
        public List<ResourceRequirementExportDto> ResourceRequirements { get; set; } = new List<ResourceRequirementExportDto>();
    }
    
    public class ResourceRequirementExportDto
    {
        public string ResourceType { get; set; }
        public string SkillLevel { get; set; }
        public int RequiredCount { get; set; }
        public decimal HourlyRate { get; set; }
        public int EstimatedHours { get; set; }
        public string Description { get; set; }
        public decimal TotalCost { get; set; }
    }
}