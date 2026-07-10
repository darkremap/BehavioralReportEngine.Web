using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BehavioralReportEngine.Web.Models
{
    public class Report
    {
        [Key]
        public long ReportId { get; set; }

        [Required]
        [MaxLength(20)]
        public string ReportScope { get; set; } = string.Empty;

        public int ReportTemplateId { get; set; }

        public int SessionId { get; set; }

        public int? ParticipantId { get; set; }

        public int? TeamId { get; set; }

        public int OrganizationId { get; set; }

        public int GameVersionId { get; set; }

        [MaxLength(300)]
        public string ReportTitle { get; set; }

        public DateTime? GeneratedAt { get; set; }

        public int? GeneratedByUserId { get; set; }

        [Required]
        [MaxLength(20)]
        public string GenerationMethod { get; set; } = string.Empty;

        [Required]
        [MaxLength(30)]
        public string Status { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int? CreatedBy { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public int? UpdatedBy { get; set; }

        public bool IsDeleted { get; set; } = false;

        public DateTime? DeletedAt { get; set; }

        public int? DeletedBy { get; set; }

        [ForeignKey(nameof(ReportTemplateId))]
        public virtual ReportTemplate ReportTemplate { get; set; }

        [ForeignKey(nameof(SessionId))]
        public virtual GameSession GameSession { get; set; }

        [ForeignKey(nameof(ParticipantId))]
        public virtual Participant Participant { get; set; }

        [ForeignKey(nameof(TeamId))]
        public virtual Team Team { get; set; }

        [ForeignKey(nameof(OrganizationId))]
        public virtual Organization Organization { get; set; }

        [ForeignKey(nameof(GameVersionId))]
        public virtual GameVersion GameVersion { get; set; }

        [ForeignKey(nameof(GeneratedByUserId))]
        public virtual AppUser AppUser { get; set; }

    }
}