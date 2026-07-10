using System.Collections.Generic;
using System.Linq;
using BehavioralReportEngine.Web.Models;

namespace BehavioralReportEngine.Web.ViewModels
{
    public class ReportPrintViewModel
    {
        public Report Report { get; set; }

        public List<ReportIndicatorScore> IndicatorScores { get; set; } = new List<ReportIndicatorScore>();

        public List<ReportPatternScore> PatternScores { get; set; } = new List<ReportPatternScore>();

        public List<ReportKeyMoment> KeyMoments { get; set; } = new List<ReportKeyMoment>();

        public List<GrowthMapEntry> GrowthMapEntries { get; set; } = new List<GrowthMapEntry>();

        public List<ActionPlanItem> ActionPlanItems { get; set; } = new List<ActionPlanItem>();

        // Bucketed 1-5 level for an indicator score, based on its % of the scoring scale max.
        public static ScoreLevel GetLevel(decimal scoreValue, decimal maxValue)
        {
            if (maxValue <= 0) return ScoreLevel.Average;
            var percent = scoreValue / maxValue * 5m;
            if (percent < 2m) return ScoreLevel.NeedsAttention;
            if (percent < 3m) return ScoreLevel.NeedsGrowth;
            if (percent < 4m) return ScoreLevel.Average;
            if (percent < 4.6m) return ScoreLevel.Good;
            return ScoreLevel.VeryStrong;
        }

        public static (string LabelFa, string Color) GetLevelInfo(ScoreLevel level)
        {
            switch (level)
            {
                case ScoreLevel.NeedsAttention: return ("نیاز به توجه", "#e63946");
                case ScoreLevel.NeedsGrowth: return ("نیاز به رشد", "#f4a261");
                case ScoreLevel.Average: return ("متوسط", "#e9c46a");
                case ScoreLevel.Good: return ("خوب", "#2a9d8f");
                default: return ("بسیار قوی", "#1b4332");
            }
        }

        public List<ReportPatternScore> DominantPatterns =>
            PatternScores.Where(p => p.IsDominantPattern).ToList();
    }

    public enum ScoreLevel
    {
        NeedsAttention,
        NeedsGrowth,
        Average,
        Good,
        VeryStrong
    }
}
