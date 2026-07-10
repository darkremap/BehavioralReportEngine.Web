using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BehavioralReportEngine.Web.Models
{
    public class GameSession
    {
        [Key]
        public int SessionId { get; set; }

        public int OrganizationId { get; set; }

        public int GameVersionId { get; set; }

        [MaxLength(50)]
        public string SessionCode { get; set; }

        [MaxLength(200)]
        public string SessionName { get; set; }

        public DateTime SessionDate { get; set; }

        public DateTime? StartTime { get; set; }

        public DateTime? EndTime { get; set; }

        [MaxLength(200)]
        public string Location { get; set; }

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

        [ForeignKey(nameof(OrganizationId))]
        public virtual Organization Organization { get; set; }

        [ForeignKey(nameof(GameVersionId))]
        public virtual GameVersion GameVersion { get; set; }

    }
}