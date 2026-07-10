using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BehavioralReportEngine.Web.Models
{
    public class Participant
    {
        [Key]
        public int ParticipantId { get; set; }

        public int OrganizationId { get; set; }

        [MaxLength(100)]
        public string PersonnelCode { get; set; }

        [Required]
        [MaxLength(100)]
        public string FirstNameFa { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string LastNameFa { get; set; } = string.Empty;

        [MaxLength(100)]
        public string FirstNameEn { get; set; }

        [MaxLength(100)]
        public string LastNameEn { get; set; }

        [MaxLength(256)]
        public string Email { get; set; }

        public int? DepartmentId { get; set; }

        public DateTime? BirthDate { get; set; }

        [MaxLength(20)]
        public string Gender { get; set; }

        [MaxLength(150)]
        public string JobTitle { get; set; }

        public string MetadataJson { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int? CreatedBy { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public int? UpdatedBy { get; set; }

        public bool IsDeleted { get; set; } = false;

        public DateTime? DeletedAt { get; set; }

        public int? DeletedBy { get; set; }

        [ForeignKey(nameof(OrganizationId))]
        public virtual Organization Organization { get; set; }

        [ForeignKey(nameof(DepartmentId))]
        public virtual Department Department { get; set; }

        [NotMapped]
        public string FullNameFa => $"{FirstNameFa} {LastNameFa}".Trim();

        [NotMapped]
        public string FullNameEn => $"{FirstNameEn} {LastNameEn}".Trim();

    }
}