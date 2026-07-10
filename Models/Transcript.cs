using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BehavioralReportEngine.Web.Models
{
    public class Transcript
    {
        [Key]
        public int TranscriptId { get; set; }

        public int SessionId { get; set; }

        public long? MediaAssetId { get; set; }

        [MaxLength(10)]
        public string Language { get; set; }

        public string FullText { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int? CreatedBy { get; set; }

        public bool IsDeleted { get; set; } = false;

        [ForeignKey(nameof(SessionId))]
        public virtual GameSession GameSession { get; set; }

        [ForeignKey(nameof(MediaAssetId))]
        public virtual MediaAsset MediaAsset { get; set; }

    }
}