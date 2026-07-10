using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BehavioralReportEngine.Web.Models
{
    public class TranscriptSegment
    {
        [Key]
        public long TranscriptSegmentId { get; set; }

        public int TranscriptId { get; set; }

        public int? SpeakerParticipantId { get; set; }

        [MaxLength(100)]
        public string SpeakerLabel { get; set; }

        public int StartTimeMs { get; set; }

        public int? EndTimeMs { get; set; }

        [Required]
        public string SegmentText { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey(nameof(TranscriptId))]
        public virtual Transcript Transcript { get; set; }

        [ForeignKey(nameof(SpeakerParticipantId))]
        public virtual Participant Participant { get; set; }

    }
}