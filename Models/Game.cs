using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BehavioralReportEngine.Web.Models
{
    public class Game
    {
        [Key]
        public int GameId { get; set; }

        [Required]
        [MaxLength(50)]
        public string GameCode { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string NameFa { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string NameEn { get; set; } = string.Empty;

        [MaxLength(500)]
        public string Summary { get; set; }

        [MaxLength(200)]
        public string TitleFa { get; set; }

        [MaxLength(200)]
        public string TitleEn { get; set; }

        public string DescriptionFa { get; set; }

        public string DescriptionEn { get; set; }

        public int? AuthorUserId { get; set; }

        public int? GameTypeId { get; set; }

        public int? DurationMinutes { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int? CreatedBy { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public int? UpdatedBy { get; set; }

        public bool IsDeleted { get; set; } = false;

        public DateTime? DeletedAt { get; set; }

        public int? DeletedBy { get; set; }

        [ForeignKey(nameof(AuthorUserId))]
        public virtual AppUser AppUser { get; set; }

        [ForeignKey(nameof(GameTypeId))]
        public virtual GameType GameType { get; set; }

    }
}