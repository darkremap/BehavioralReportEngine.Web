using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BehavioralReportEngine.Web.Models
{
    public class Team
    {
        [Key]
        public int TeamId { get; set; }

        public int SessionId { get; set; }

        [Required]
        [MaxLength(150)]
        public string TeamName { get; set; } = string.Empty;

        [MaxLength(50)]
        public string TeamCode { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int? CreatedBy { get; set; }

        public bool IsDeleted { get; set; } = false;

        [ForeignKey(nameof(SessionId))]
        public virtual GameSession GameSession { get; set; }

    }
}