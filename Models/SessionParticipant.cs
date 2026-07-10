using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BehavioralReportEngine.Web.Models
{
    public class SessionParticipant
    {
        [Key]
        public int SessionParticipantId { get; set; }

        public int SessionId { get; set; }

        public int? TeamId { get; set; }

        public int ParticipantId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int? CreatedBy { get; set; }

        public bool IsDeleted { get; set; } = false;

        [ForeignKey(nameof(SessionId))]
        public virtual GameSession GameSession { get; set; }

        [ForeignKey(nameof(TeamId))]
        public virtual Team Team { get; set; }

        [ForeignKey(nameof(ParticipantId))]
        public virtual Participant Participant { get; set; }

    }
}