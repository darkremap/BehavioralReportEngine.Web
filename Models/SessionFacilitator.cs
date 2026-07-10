using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BehavioralReportEngine.Web.Models
{
    public class SessionFacilitator
    {
        [Key]
        public int SessionFacilitatorId { get; set; }

        public int SessionId { get; set; }

        public int UserId { get; set; }

        [MaxLength(50)]
        public string RoleInSession { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int? CreatedBy { get; set; }

        [ForeignKey(nameof(SessionId))]
        public virtual GameSession GameSession { get; set; }

        [ForeignKey(nameof(UserId))]
        public virtual AppUser AppUser { get; set; }

    }
}