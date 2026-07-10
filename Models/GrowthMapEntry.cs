using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BehavioralReportEngine.Web.Models
{
    public class GrowthMapEntry
    {
        [Key]
        public long GrowthMapEntryId { get; set; }

        public long ReportId { get; set; }

        public int? IndicatorDefinitionId { get; set; }

        [Required]
        [MaxLength(200)]
        public string Area { get; set; } = string.Empty;

        public string CurrentState { get; set; }

        public string TargetState { get; set; }

        public int DisplayOrder { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int? CreatedBy { get; set; }

        [ForeignKey(nameof(ReportId))]
        public virtual Report Report { get; set; }

        [ForeignKey(nameof(IndicatorDefinitionId))]
        public virtual IndicatorDefinition IndicatorDefinition { get; set; }

    }
}