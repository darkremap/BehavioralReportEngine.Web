using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BehavioralReportEngine.Web.Models
{
    public class AppUserRole
    {
        [Key]
        public int AppUserRoleId { get; set; }

        public int UserId { get; set; }

        public int RoleId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int? CreatedBy { get; set; }

        [ForeignKey(nameof(UserId))]
        public virtual AppUser AppUser { get; set; }

        [ForeignKey(nameof(RoleId))]
        public virtual UserRole UserRole { get; set; }

    }
}