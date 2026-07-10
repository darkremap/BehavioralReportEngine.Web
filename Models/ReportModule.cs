using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BehavioralReportEngine.Web.Models
{
    public class ReportModule
    {
        [Key]
        public long ReportModuleId { get; set; }

        public long ReportId { get; set; }

        public int ReportModuleTypeId { get; set; }

        public int? IndicatorDefinitionId { get; set; }

        public int DisplayOrder { get; set; }

        public string ContentText { get; set; }

        public string ContentHtml { get; set; }

        public bool IsAiGenerated { get; set; }

        [MaxLength(100)]
        public string AiModel { get; set; }

        [MaxLength(200)]
        public string AiPromptRef { get; set; }

        public int? ReviewedByUserId { get; set; }

        public DateTime? ReviewedAt { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int? CreatedBy { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public int? UpdatedBy { get; set; }

        public bool IsDeleted { get; set; } = false;

        [ForeignKey(nameof(ReportId))]
        public virtual Report Report { get; set; }

        [ForeignKey(nameof(ReportModuleTypeId))]
        public virtual ReportModuleType ReportModuleType { get; set; }

        [ForeignKey(nameof(IndicatorDefinitionId))]
        public virtual IndicatorDefinition IndicatorDefinition { get; set; }

        [ForeignKey(nameof(ReviewedByUserId))]
        public virtual AppUser AppUser { get; set; }

    }
}