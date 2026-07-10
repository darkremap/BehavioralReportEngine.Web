using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BehavioralReportEngine.Web.Models
{
    public class IndicatorDefinition
    {
        [Key]
        public int IndicatorDefinitionId { get; set; }

        public int GameVersionId { get; set; }

        [Required]
        [MaxLength(50)]
        public string IndicatorCode { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string TitleEn { get; set; } = string.Empty;

        public string ShortDefinition { get; set; }

        public int DisplayOrder { get; set; }

        public int ScoringScaleId { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int? CreatedBy { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public int? UpdatedBy { get; set; }

        public bool IsDeleted { get; set; } = false;

        [ForeignKey(nameof(GameVersionId))]
        public virtual GameVersion GameVersion { get; set; }

        [ForeignKey(nameof(ScoringScaleId))]
        public virtual ScoringScale ScoringScale { get; set; }

    }
}