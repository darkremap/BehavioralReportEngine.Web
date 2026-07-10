using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BehavioralReportEngine.Web.Models
{
    public class ReportTemplateModule
    {
        [Key]
        public int ReportTemplateModuleId { get; set; }

        public int ReportTemplateId { get; set; }

        public int ReportModuleTypeId { get; set; }

        public int DisplayOrder { get; set; }

        public bool IsRequired { get; set; }

        public string ConfigJson { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int? CreatedBy { get; set; }

        [ForeignKey(nameof(ReportTemplateId))]
        public virtual ReportTemplate ReportTemplate { get; set; }

        [ForeignKey(nameof(ReportModuleTypeId))]
        public virtual ReportModuleType ReportModuleType { get; set; }

    }
}