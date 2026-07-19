using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using BehavioralReportEngine.Web.Models;

namespace BehavioralReportEngine.Web.ViewModels
{
    public class ReportPrintViewModel
    {
        private static readonly string[] PersianMonthNames =
        {
            "فروردین", "اردیبهشت", "خرداد", "تیر", "مرداد", "شهریور",
            "مهر", "آبان", "آذر", "دی", "بهمن", "اسفند"
        };

        // "12 / اردیبهشت / 1405" formatted Shamsi (Jalali) date, per the SELVA report's cover page.
        public static string ToPersianDate(DateTime date)
        {
            var calendar = new PersianCalendar();
            var day = calendar.GetDayOfMonth(date);
            var month = calendar.GetMonth(date);
            var year = calendar.GetYear(date);
            return $"{day} / {PersianMonthNames[month - 1]} / {year}";
        }

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
        // one stable, language-independent identifier (Title/TitleEn vary by game). Falls back
        // to matching Persian keywords in the (always-present) Title when the code doesn't hit,
        // since not every game seeds IndicatorCode with these English keywords.
        public static string GetIndicatorIcon(string indicatorCode, string title = null)
        {
            var code = (indicatorCode ?? "").ToUpperInvariant();
            if (code.Contains("LISTEN")) return "\U0001F442";
            if (code.Contains("CLARITY") || code.Contains("COMMUNICAT")) return "\U0001F4AC";
            if (code.Contains("EMOTION")) return "\U0001F9E0";
            if (code.Contains("EMPATH")) return "\U00002764";
            if (code.Contains("AWARE") || code.Contains("SELF")) return "\U0001F9ED";

            var text = title ?? "";
            if (text.Contains("شنیدن")) return "\U0001F442";
            if (text.Contains("ارتباط") || text.Contains("گفتگو")) return "\U0001F4AC";
            if (text.Contains("هیجان") || text.Contains("احساس")) return "\U0001F9E0";
            if (text.Contains("همدلی")) return "\U00002764";
            if (text.Contains("خودآگاهی")) return "\U0001F9ED";

            return "\U0001F3AF";
        }

        // Short reflective quote per indicator, matched by Persian keyword in the Title,
        // shown in the closing quote box of each indicator detail page.
        public static string GetIndicatorQuote(string title)
        {
            var text = title ?? "";
            if (text.Contains("شنیدن")) return "گوش دادن، فقط شنیدن صداها نیست؛ درک حضور انسان مقابل و پاسخ دادن با آگاهی است.";
            if (text.Contains("ارتباط")) return "وقتی پیام شما ساختاریافته باشد، دیگران سریع‌تر همراهتان می‌شوند و همکاری مؤثرتری شکل می‌گیرد.";
            if (text.Contains("هیجان")) return "کنترل هیجان به معنای نبود احساسات نیست؛ بلکه توانایی انتخاب آگاهانه پاسخ مناسب در هر شرایطی است.";
            if (text.Contains("همدلی")) return "همدلی یعنی با قلب دیگران دیدن دنیا، بدون اینکه مسیر خود را گم کنیم.";
            if (text.Contains("خودآگاهی")) return "خودآگاهی، نخستین گام برای انتخاب آگاهانه رفتار است.";
            return "هر گفتگو فرصتی است برای شناخت بهتر خود و دیگران.";
        }

        // Falls back to matching Persian keywords in the (always-present) pattern Name when
        // PatternCode doesn't contain the expected English keyword - same reasoning as
        // GetIndicatorIcon's fallback.
        public static string GetPatternIcon(string patternCode, string name = null)
        {
            var code = (patternCode ?? "").ToUpperInvariant();
            if (code.Contains("ANALYST")) return "\U0001F50D";
            if (code.Contains("OBSERV")) return "\U0001F441";
            if (code.Contains("MEDIAT")) return "\U0001F91D";
            if (code.Contains("PARTICIP") || code.Contains("ACTIVE")) return "\U0001F64B";
            if (code.Contains("NARRAT") || code.Contains("STORY")) return "\U0001F4AD";
            if (code.Contains("FACILIT")) return "\U0001F393";

            var text = name ?? "";
            if (text.Contains("تحلیل")) return "\U0001F50D";
            if (text.Contains("ناظر")) return "\U0001F441";
            if (text.Contains("میانجی")) return "\U0001F91D";
            if (text.Contains("مشارکت")) return "\U0001F64B";
            if (text.Contains("روایت")) return "\U0001F4AD";
            if (text.Contains("تسهیل")) return "\U0001F393";

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
