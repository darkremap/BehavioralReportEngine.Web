using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BehavioralReportEngine.Web.Models
{
    public class GamePattern
    {
        [Key]
        public int GamePatternId { get; set; }

        public int GameVersionId { get; set; }

        public int BehavioralPatternId { get; set; }

        [MaxLength(150)]
        public string CustomName { get; set; }

        public string CustomDescription { get; set; }

        public int DisplayOrder { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int? CreatedBy { get; set; }

        [ForeignKey(nameof(GameVersionId))]
        public virtual GameVersion GameVersion { get; set; }

        [ForeignKey(nameof(BehavioralPatternId))]
        public virtual BehavioralPattern BehavioralPattern { get; set; }

    }
}