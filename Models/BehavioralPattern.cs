using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BehavioralReportEngine.Web.Models
{
    public class BehavioralPattern
    {
        [Key]
        public int BehavioralPatternId { get; set; }

        [Required]
        [MaxLength(50)]
        public string PatternCode { get; set; } = string.Empty;

        [Required]
        [MaxLength(150)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(150)]
        public string NameEn { get; set; } = string.Empty;

        public string Description { get; set; }

        public bool IsGlobal { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int? CreatedBy { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public int? UpdatedBy { get; set; }

        public bool IsDeleted { get; set; } = false;

    }
}