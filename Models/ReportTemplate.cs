using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BehavioralReportEngine.Web.Models
{
    public class ReportTemplate
    {
        [Key]
        public int ReportTemplateId { get; set; }

        public int? GameVersionId { get; set; }

        [Required]
        [MaxLength(20)]
        public string ReportScope { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string TemplateName { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string VersionNumber { get; set; } = string.Empty;

        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int? CreatedBy { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public int? UpdatedBy { get; set; }

        public bool IsDeleted { get; set; } = false;

        [ForeignKey(nameof(GameVersionId))]
        public virtual GameVersion GameVersion { get; set; }

    }
}