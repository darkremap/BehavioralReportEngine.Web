using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BehavioralReportEngine.Web.Models
{
    public class ReportPatternScore
    {
        [Key]
        public long ReportPatternScoreId { get; set; }

        public long ReportId { get; set; }

        public int BehavioralPatternId { get; set; }

        public decimal? ScoreValue { get; set; }

        public bool IsDominantPattern { get; set; }

        public string Description { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int? CreatedBy { get; set; }

        [ForeignKey(nameof(ReportId))]
        public virtual Report Report { get; set; }

        [ForeignKey(nameof(BehavioralPatternId))]
        public virtual BehavioralPattern BehavioralPattern { get; set; }

    }
}