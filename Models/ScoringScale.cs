using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BehavioralReportEngine.Web.Models
{
    public class ScoringScale
    {
        [Key]
        public int ScoringScaleId { get; set; }

        [Required]
        [MaxLength(50)]
        public string ScaleCode { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        public decimal MinValue { get; set; }

        public decimal MaxValue { get; set; }

        public decimal StepValue { get; set; }

        public string Description { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int? CreatedBy { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public int? UpdatedBy { get; set; }

        public bool IsDeleted { get; set; } = false;

    }
}