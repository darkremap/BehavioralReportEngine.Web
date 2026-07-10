using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BehavioralReportEngine.Web.Models
{
    public class KeyMoment
    {
        [Key]
        public int KeyMomentId { get; set; }

        public int SessionId { get; set; }

        public int? TeamId { get; set; }

        public int? ParticipantId { get; set; }

        public long? MediaAssetId { get; set; }

        public long? TranscriptSegmentId { get; set; }

        public int TimestampMs { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        public string Description { get; set; }

        public string Significance { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int? CreatedBy { get; set; }

        public bool IsDeleted { get; set; } = false;

        [ForeignKey(nameof(SessionId))]
        public virtual GameSession GameSession { get; set; }

        [ForeignKey(nameof(TeamId))]
        public virtual Team Team { get; set; }

        [ForeignKey(nameof(ParticipantId))]
        public virtual Participant Participant { get; set; }

        [ForeignKey(nameof(MediaAssetId))]
        public virtual MediaAsset MediaAsset { get; set; }

        [ForeignKey(nameof(TranscriptSegmentId))]
        public virtual TranscriptSegment TranscriptSegment { get; set; }

    }
}