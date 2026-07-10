using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BehavioralReportEngine.Web.Models
{
    public class GameVersion
    {
        [Key]
        public int GameVersionId { get; set; }

        public int GameId { get; set; }

        [Required]
        [MaxLength(20)]
        public string VersionNumber { get; set; } = string.Empty;

        [MaxLength(100)]
        public string VersionLabel { get; set; }

        public DateTime EffectiveFrom { get; set; }

        public DateTime? EffectiveTo { get; set; }

        public bool IsCurrent { get; set; }

        public string ReleaseNotes { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int? CreatedBy { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public int? UpdatedBy { get; set; }

        public bool IsDeleted { get; set; } = false;

        public DateTime? DeletedAt { get; set; }

        public int? DeletedBy { get; set; }

        [ForeignKey(nameof(GameId))]
        public virtual Game Game { get; set; }

    }
}