using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BehavioralReportEngine.Web.Models
{
    public class ActionPlanItem
    {
        [Key]
        public long ActionPlanItemId { get; set; }

        public long ReportId { get; set; }

        public int? IndicatorDefinitionId { get; set; }

        [Required]
        public string ActionDescription { get; set; } = string.Empty;

        [MaxLength(50)]
        public string TimeFrame { get; set; }

        [MaxLength(20)]
        public string Priority { get; set; }

        public int DisplayOrder { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int? CreatedBy { get; set; }

        [ForeignKey(nameof(ReportId))]
        public virtual Report Report { get; set; }

        [ForeignKey(nameof(IndicatorDefinitionId))]
        public virtual IndicatorDefinition IndicatorDefinition { get; set; }

    }
}