using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BehavioralReportEngine.Web.Models
{
    public class Observation
    {
        [Key]
        public long ObservationId { get; set; }

        public int SessionId { get; set; }

        public int? ParticipantId { get; set; }

        public int? TeamId { get; set; }

        public int ObservedByUserId { get; set; }

        public int? IndicatorDefinitionId { get; set; }

        public int? BehavioralPatternId { get; set; }

        public int? ObservationTimeMs { get; set; }

        [Required]
        public string ObservationText { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsDeleted { get; set; } = false;

        [ForeignKey(nameof(SessionId))]
        public virtual GameSession GameSession { get; set; }

        [ForeignKey(nameof(ParticipantId))]
        public virtual Participant Participant { get; set; }

        [ForeignKey(nameof(TeamId))]
        public virtual Team Team { get; set; }

        [ForeignKey(nameof(ObservedByUserId))]
        public virtual AppUser AppUser { get; set; }

        [ForeignKey(nameof(IndicatorDefinitionId))]
        public virtual IndicatorDefinition IndicatorDefinition { get; set; }

        [ForeignKey(nameof(BehavioralPatternId))]
        public virtual BehavioralPattern BehavioralPattern { get; set; }

    }
}