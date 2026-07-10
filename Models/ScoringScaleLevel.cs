using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BehavioralReportEngine.Web.Models
{
    public class ScoringScaleLevel
    {
        [Key]
        public int ScoringScaleLevelId { get; set; }

        public int ScoringScaleId { get; set; }

        public decimal LevelValue { get; set; }

        [Required]
        [MaxLength(100)]
        public string LevelLabel { get; set; } = string.Empty;

        public string LevelDescription { get; set; }

        [ForeignKey(nameof(ScoringScaleId))]
        public virtual ScoringScale ScoringScale { get; set; }

    }
}