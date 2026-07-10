using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BehavioralReportEngine.Web.Models
{
    public class ReportModuleType
    {
        [Key]
        public int ReportModuleTypeId { get; set; }

        [Required]
        [MaxLength(50)]
        public string ModuleCode { get; set; } = string.Empty;

        [Required]
        [MaxLength(150)]
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; }

    }
}