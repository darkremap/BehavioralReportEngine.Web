using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BehavioralReportEngine.Web.Models
{
    public class MediaAsset
    {
        [Key]
        public long MediaAssetId { get; set; }

        public int SessionId { get; set; }

        [Required]
        [MaxLength(20)]
        public string MediaType { get; set; } = string.Empty;

        [Required]
        [MaxLength(300)]
        public string FileName { get; set; } = string.Empty;

        [Required]
        [MaxLength(500)]
        public string StoragePath { get; set; } = string.Empty;

        public int? DurationSeconds { get; set; }

        public DateTime? RecordedAt { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int? CreatedBy { get; set; }

        public bool IsDeleted { get; set; } = false;

        [ForeignKey(nameof(SessionId))]
        public virtual GameSession GameSession { get; set; }

    }
}