using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BehavioralReportEngine.Web.Models
{
    public class ReportIndicatorScore
    {
        [Key]
        public long ReportIndicatorScoreId { get; set; }

        public long ReportId { get; set; }

        public int IndicatorDefinitionId { get; set; }

        public decimal ScoreValue { get; set; }

        public int ScoringScaleId { get; set; }

        public string ObservedEvidence { get; set; }

        public string IndicatorBehaviors { get; set; }

        public string GrowthOpportunities { get; set; }

        public string Strengths { get; set; }

        public string PracticalRecommendation { get; set; }

        public bool IsAiGenerated { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int? CreatedBy { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public int? UpdatedBy { get; set; }

        [ForeignKey(nameof(ReportId))]
        public virtual Report Report { get; set; }

        [ForeignKey(nameof(IndicatorDefinitionId))]
        public virtual IndicatorDefinition IndicatorDefinition { get; set; }

        [ForeignKey(nameof(ScoringScaleId))]
        public virtual ScoringScale ScoringScale { get; set; }

    }
}