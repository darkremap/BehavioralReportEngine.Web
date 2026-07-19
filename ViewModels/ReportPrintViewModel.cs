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

        // Small icon per indicator, matched by IndicatorCode keyword since the code is the
        // one stable, language-independent identifier (Title/TitleEn vary by game).
        public static string GetIndicatorIcon(string indicatorCode)
        {
            var code = (indicatorCode ?? "").ToUpperInvariant();
            if (code.Contains("LISTEN")) return "\U0001F442";
            if (code.Contains("CLARITY") || code.Contains("COMMUNICAT")) return "\U0001F4AC";
            if (code.Contains("EMOTION")) return "\U0001F9E0";
            if (code.Contains("EMPATH")) return "\U00002764";
            if (code.Contains("AWARE") || code.Contains("SELF")) return "\U0001F9ED";
            return "\U0001F3AF";
        }

        public static string GetPatternIcon(string patternCode)
        {
            var code = (patternCode ?? "").ToUpperInvariant();
            if (code.Contains("ANALYST")) return "\U0001F50D";
            if (code.Contains("OBSERV")) return "\U0001F441";
            if (code.Contains("MEDIAT")) return "\U0001F91D";
            if (code.Contains("PARTICIP") || code.Contains("ACTIVE")) return "\U0001F64B";
            if (code.Contains("NARRAT") || code.Contains("STORY")) return "\U0001F4AD";
            if (code.Contains("FACILIT")) return "\U0001F393";
            return "\U0001F539";
        }

        // Best-effort color/icon per growth-map column, matched by keyword in the (free-text) Area
        // label since growth areas aren't backed by a fixed enum.
        public static (string Icon, string Color) GetGrowthColumnStyle(string area)
        {
            var text = area ?? "";
            if (text.Contains("ادامه")) return ("\U0001F4C8", "#2a9d8f");
            if (text.Contains("تمرین")) return ("\U0001F4AA", "#457b9d");
            if (text.Contains("مراقب")) return ("\U000026A0", "#e76f51");
            return ("\U0001F3AF", "#457b9d");
        }
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
