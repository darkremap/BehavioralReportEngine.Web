using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BehavioralReportEngine.Web.Models
{
    public class GameType
    {
        [Key]
        public int GameTypeId { get; set; }

        [Required]
        [MaxLength(50)]
        public string TypeCode { get; set; } = string.Empty;

        [Required]
        [MaxLength(150)]
        public string NameFa { get; set; } = string.Empty;

        [Required]
        [MaxLength(150)]
        public string NameEn { get; set; } = string.Empty;

        public string Description { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int? CreatedBy { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public int? UpdatedBy { get; set; }

        public bool IsDeleted { get; set; } = false;

    }
}