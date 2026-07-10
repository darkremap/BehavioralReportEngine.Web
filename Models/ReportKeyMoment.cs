using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BehavioralReportEngine.Web.Models
{
    public class ReportKeyMoment
    {
        [Key]
        public long ReportKeyMomentId { get; set; }

        public long ReportId { get; set; }

        public int KeyMomentId { get; set; }

        public int DisplayOrder { get; set; }

        public string NarrativeText { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int? CreatedBy { get; set; }

        [ForeignKey(nameof(ReportId))]
        public virtual Report Report { get; set; }

        [ForeignKey(nameof(KeyMomentId))]
        public virtual KeyMoment KeyMoment { get; set; }

    }
}