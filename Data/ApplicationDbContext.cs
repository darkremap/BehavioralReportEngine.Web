using Microsoft.EntityFrameworkCore;
using BehavioralReportEngine.Web.Models;

namespace BehavioralReportEngine.Web.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Organization> Organizations { get; set; } = null!;
        public DbSet<UserRole> UserRoles { get; set; } = null!;
        public DbSet<AppUser> AppUsers { get; set; } = null!;
        public DbSet<AppUserRole> AppUserRoles { get; set; } = null!;
        public DbSet<Department> Departments { get; set; } = null!;
        public DbSet<Participant> Participants { get; set; } = null!;
        public DbSet<GameType> GameTypes { get; set; } = null!;
        public DbSet<Game> Games { get; set; } = null!;
        public DbSet<GameVersion> GameVersions { get; set; } = null!;
        public DbSet<ScoringScale> ScoringScales { get; set; } = null!;
        public DbSet<ScoringScaleLevel> ScoringScaleLevels { get; set; } = null!;
        public DbSet<IndicatorDefinition> IndicatorDefinitions { get; set; } = null!;
        public DbSet<BehavioralPattern> BehavioralPatterns { get; set; } = null!;
        public DbSet<GamePattern> GamePatterns { get; set; } = null!;
        public DbSet<GameSession> GameSessions { get; set; } = null!;
        public DbSet<SessionFacilitator> SessionFacilitators { get; set; } = null!;
        public DbSet<Team> Teams { get; set; } = null!;
        public DbSet<SessionParticipant> SessionParticipants { get; set; } = null!;
        public DbSet<MediaAsset> MediaAssets { get; set; } = null!;
        public DbSet<Transcript> Transcripts { get; set; } = null!;
        public DbSet<TranscriptSegment> TranscriptSegments { get; set; } = null!;
        public DbSet<Observation> Observations { get; set; } = null!;
        public DbSet<KeyMoment> KeyMoments { get; set; } = null!;
        public DbSet<ReportModuleType> ReportModuleTypes { get; set; } = null!;
        public DbSet<ReportTemplate> ReportTemplates { get; set; } = null!;
        public DbSet<ReportTemplateModule> ReportTemplateModules { get; set; } = null!;
        public DbSet<Report> Reports { get; set; } = null!;
        public DbSet<ReportModule> ReportModules { get; set; } = null!;
        public DbSet<ReportIndicatorScore> ReportIndicatorScores { get; set; } = null!;
        public DbSet<ReportPatternScore> ReportPatternScores { get; set; } = null!;
        public DbSet<ReportKeyMoment> ReportKeyMoments { get; set; } = null!;
        public DbSet<GrowthMapEntry> GrowthMapEntries { get; set; } = null!;
        public DbSet<ActionPlanItem> ActionPlanItems { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Organization>(entity =>
            {
                entity.ToTable("Organization", "core");
                entity.HasKey(x => x.OrganizationId);
                entity.Property(x => x.OrganizationCode).IsRequired();
                entity.Property(x => x.Name).IsRequired();
            });

            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.ToTable("UserRole", "core");
                entity.HasKey(x => x.RoleId);
                entity.Property(x => x.RoleCode).IsRequired();
                entity.Property(x => x.RoleName).IsRequired();
            });

            modelBuilder.Entity<AppUser>(entity =>
            {
                entity.ToTable("AppUser", "core");
                entity.HasKey(x => x.UserId);
                entity.Property(x => x.FullName).IsRequired();
                entity.Property(x => x.Email).IsRequired();
                entity.HasOne(x => x.Organization)
                    .WithMany()
                    .HasForeignKey(x => x.OrganizationId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<AppUserRole>(entity =>
            {
                entity.ToTable("AppUserRole", "core");
                entity.HasKey(x => x.AppUserRoleId);
                entity.HasOne(x => x.AppUser)
                    .WithMany()
                    .HasForeignKey(x => x.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(x => x.UserRole)
                    .WithMany()
                    .HasForeignKey(x => x.RoleId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Department>(entity =>
            {
                entity.ToTable("Department", "core");
                entity.HasKey(x => x.DepartmentId);
                entity.Property(x => x.DepartmentCode).IsRequired();
                entity.Property(x => x.NameFa).IsRequired();
            });

            modelBuilder.Entity<Participant>(entity =>
            {
                entity.ToTable("Participant", "core");
                entity.HasKey(x => x.ParticipantId);
                entity.Property(x => x.FirstNameFa).IsRequired();
                entity.Property(x => x.LastNameFa).IsRequired();
                entity.HasOne(x => x.Organization)
                    .WithMany()
                    .HasForeignKey(x => x.OrganizationId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(x => x.Department)
                    .WithMany()
                    .HasForeignKey(x => x.DepartmentId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<GameType>(entity =>
            {
                entity.ToTable("GameType", "game");
                entity.HasKey(x => x.GameTypeId);
                entity.Property(x => x.TypeCode).IsRequired();
                entity.Property(x => x.NameFa).IsRequired();
                entity.Property(x => x.NameEn).IsRequired();
            });

            modelBuilder.Entity<Game>(entity =>
            {
                entity.ToTable("Game", "game");
                entity.HasKey(x => x.GameId);
                entity.Property(x => x.GameCode).IsRequired();
                entity.Property(x => x.NameFa).IsRequired();
                entity.Property(x => x.NameEn).IsRequired();
                entity.HasOne(x => x.AppUser)
                    .WithMany()
                    .HasForeignKey(x => x.AuthorUserId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(x => x.GameType)
                    .WithMany()
                    .HasForeignKey(x => x.GameTypeId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<GameVersion>(entity =>
            {
                entity.ToTable("GameVersion", "game");
                entity.HasKey(x => x.GameVersionId);
                entity.Property(x => x.VersionNumber).IsRequired();
                entity.HasOne(x => x.Game)
                    .WithMany()
                    .HasForeignKey(x => x.GameId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<ScoringScale>(entity =>
            {
                entity.ToTable("ScoringScale", "game");
                entity.HasKey(x => x.ScoringScaleId);
                entity.Property(x => x.ScaleCode).IsRequired();
                entity.Property(x => x.Name).IsRequired();
                entity.Property(x => x.MinValue).HasColumnType("decimal(5,2)");
                entity.Property(x => x.MaxValue).HasColumnType("decimal(5,2)");
                entity.Property(x => x.StepValue).HasColumnType("decimal(5,2)");
            });

            modelBuilder.Entity<ScoringScaleLevel>(entity =>
            {
                entity.ToTable("ScoringScaleLevel", "game");
                entity.HasKey(x => x.ScoringScaleLevelId);
                entity.Property(x => x.LevelValue).HasColumnType("decimal(5,2)");
                entity.Property(x => x.LevelLabel).IsRequired();
                entity.HasOne(x => x.ScoringScale)
                    .WithMany()
                    .HasForeignKey(x => x.ScoringScaleId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<IndicatorDefinition>(entity =>
            {
                entity.ToTable("IndicatorDefinition", "game");
                entity.HasKey(x => x.IndicatorDefinitionId);
                entity.Property(x => x.IndicatorCode).IsRequired();
                entity.Property(x => x.Title).IsRequired();
                entity.Property(x => x.TitleEn).IsRequired();
                entity.HasOne(x => x.GameVersion)
                    .WithMany()
                    .HasForeignKey(x => x.GameVersionId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(x => x.ScoringScale)
                    .WithMany()
                    .HasForeignKey(x => x.ScoringScaleId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<BehavioralPattern>(entity =>
            {
                entity.ToTable("BehavioralPattern", "game");
                entity.HasKey(x => x.BehavioralPatternId);
                entity.Property(x => x.PatternCode).IsRequired();
                entity.Property(x => x.Name).IsRequired();
                entity.Property(x => x.NameEn).IsRequired();
            });

            modelBuilder.Entity<GamePattern>(entity =>
            {
                entity.ToTable("GamePattern", "game");
                entity.HasKey(x => x.GamePatternId);
                entity.HasOne(x => x.GameVersion)
                    .WithMany()
                    .HasForeignKey(x => x.GameVersionId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(x => x.BehavioralPattern)
                    .WithMany()
                    .HasForeignKey(x => x.BehavioralPatternId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<GameSession>(entity =>
            {
                entity.ToTable("GameSession", "session");
                entity.HasKey(x => x.SessionId);
                entity.Property(x => x.Status).IsRequired();
                entity.HasOne(x => x.Organization)
                    .WithMany()
                    .HasForeignKey(x => x.OrganizationId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(x => x.GameVersion)
                    .WithMany()
                    .HasForeignKey(x => x.GameVersionId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<SessionFacilitator>(entity =>
            {
                entity.ToTable("SessionFacilitator", "session");
                entity.HasKey(x => x.SessionFacilitatorId);
                entity.HasOne(x => x.GameSession)
                    .WithMany()
                    .HasForeignKey(x => x.SessionId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(x => x.AppUser)
                    .WithMany()
                    .HasForeignKey(x => x.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Team>(entity =>
            {
                entity.ToTable("Team", "session");
                entity.HasKey(x => x.TeamId);
                entity.Property(x => x.TeamName).IsRequired();
                entity.HasOne(x => x.GameSession)
                    .WithMany()
                    .HasForeignKey(x => x.SessionId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<SessionParticipant>(entity =>
            {
                entity.ToTable("SessionParticipant", "session");
                entity.HasKey(x => x.SessionParticipantId);
                entity.HasOne(x => x.GameSession)
                    .WithMany()
                    .HasForeignKey(x => x.SessionId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(x => x.Team)
                    .WithMany()
                    .HasForeignKey(x => x.TeamId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(x => x.Participant)
                    .WithMany()
                    .HasForeignKey(x => x.ParticipantId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<MediaAsset>(entity =>
            {
                entity.ToTable("MediaAsset", "session");
                entity.HasKey(x => x.MediaAssetId);
                entity.Property(x => x.MediaType).IsRequired();
                entity.Property(x => x.FileName).IsRequired();
                entity.Property(x => x.StoragePath).IsRequired();
                entity.HasOne(x => x.GameSession)
                    .WithMany()
                    .HasForeignKey(x => x.SessionId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Transcript>(entity =>
            {
                entity.ToTable("Transcript", "session");
                entity.HasKey(x => x.TranscriptId);
                entity.HasOne(x => x.GameSession)
                    .WithMany()
                    .HasForeignKey(x => x.SessionId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(x => x.MediaAsset)
                    .WithMany()
                    .HasForeignKey(x => x.MediaAssetId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<TranscriptSegment>(entity =>
            {
                entity.ToTable("TranscriptSegment", "session");
                entity.HasKey(x => x.TranscriptSegmentId);
                entity.Property(x => x.SegmentText).IsRequired();
                entity.HasOne(x => x.Transcript)
                    .WithMany()
                    .HasForeignKey(x => x.TranscriptId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(x => x.Participant)
                    .WithMany()
                    .HasForeignKey(x => x.SpeakerParticipantId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Observation>(entity =>
            {
                entity.ToTable("Observation", "session");
                entity.HasKey(x => x.ObservationId);
                entity.Property(x => x.ObservationText).IsRequired();
                entity.HasOne(x => x.GameSession)
                    .WithMany()
                    .HasForeignKey(x => x.SessionId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(x => x.Participant)
                    .WithMany()
                    .HasForeignKey(x => x.ParticipantId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(x => x.Team)
                    .WithMany()
                    .HasForeignKey(x => x.TeamId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(x => x.AppUser)
                    .WithMany()
                    .HasForeignKey(x => x.ObservedByUserId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(x => x.IndicatorDefinition)
                    .WithMany()
                    .HasForeignKey(x => x.IndicatorDefinitionId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(x => x.BehavioralPattern)
                    .WithMany()
                    .HasForeignKey(x => x.BehavioralPatternId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<KeyMoment>(entity =>
            {
                entity.ToTable("KeyMoment", "session");
                entity.HasKey(x => x.KeyMomentId);
                entity.Property(x => x.Title).IsRequired();
                entity.HasOne(x => x.GameSession)
                    .WithMany()
                    .HasForeignKey(x => x.SessionId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(x => x.Team)
                    .WithMany()
                    .HasForeignKey(x => x.TeamId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(x => x.Participant)
                    .WithMany()
                    .HasForeignKey(x => x.ParticipantId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(x => x.MediaAsset)
                    .WithMany()
                    .HasForeignKey(x => x.MediaAssetId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(x => x.TranscriptSegment)
                    .WithMany()
                    .HasForeignKey(x => x.TranscriptSegmentId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<ReportModuleType>(entity =>
            {
                entity.ToTable("ReportModuleType", "template");
                entity.HasKey(x => x.ReportModuleTypeId);
                entity.Property(x => x.ModuleCode).IsRequired();
                entity.Property(x => x.Name).IsRequired();
            });

            modelBuilder.Entity<ReportTemplate>(entity =>
            {
                entity.ToTable("ReportTemplate", "template");
                entity.HasKey(x => x.ReportTemplateId);
                entity.Property(x => x.ReportScope).IsRequired();
                entity.Property(x => x.TemplateName).IsRequired();
                entity.Property(x => x.VersionNumber).IsRequired();
                entity.HasOne(x => x.GameVersion)
                    .WithMany()
                    .HasForeignKey(x => x.GameVersionId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<ReportTemplateModule>(entity =>
            {
                entity.ToTable("ReportTemplateModule", "template");
                entity.HasKey(x => x.ReportTemplateModuleId);
                entity.HasOne(x => x.ReportTemplate)
                    .WithMany()
                    .HasForeignKey(x => x.ReportTemplateId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(x => x.ReportModuleType)
                    .WithMany()
                    .HasForeignKey(x => x.ReportModuleTypeId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Report>(entity =>
            {
                entity.ToTable("Report", "report");
                entity.HasKey(x => x.ReportId);
                entity.Property(x => x.ReportScope).IsRequired();
                entity.Property(x => x.GenerationMethod).IsRequired();
                entity.Property(x => x.Status).IsRequired();
                entity.HasOne(x => x.ReportTemplate)
                    .WithMany()
                    .HasForeignKey(x => x.ReportTemplateId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(x => x.GameSession)
                    .WithMany()
                    .HasForeignKey(x => x.SessionId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(x => x.Participant)
                    .WithMany()
                    .HasForeignKey(x => x.ParticipantId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(x => x.Team)
                    .WithMany()
                    .HasForeignKey(x => x.TeamId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(x => x.Organization)
                    .WithMany()
                    .HasForeignKey(x => x.OrganizationId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(x => x.GameVersion)
                    .WithMany()
                    .HasForeignKey(x => x.GameVersionId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(x => x.AppUser)
                    .WithMany()
                    .HasForeignKey(x => x.GeneratedByUserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<ReportModule>(entity =>
            {
                entity.ToTable("ReportModule", "report");
                entity.HasKey(x => x.ReportModuleId);
                entity.HasOne(x => x.Report)
                    .WithMany()
                    .HasForeignKey(x => x.ReportId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(x => x.ReportModuleType)
                    .WithMany()
                    .HasForeignKey(x => x.ReportModuleTypeId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(x => x.IndicatorDefinition)
                    .WithMany()
                    .HasForeignKey(x => x.IndicatorDefinitionId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(x => x.AppUser)
                    .WithMany()
                    .HasForeignKey(x => x.ReviewedByUserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<ReportIndicatorScore>(entity =>
            {
                entity.ToTable("ReportIndicatorScore", "report");
                entity.HasKey(x => x.ReportIndicatorScoreId);
                entity.Property(x => x.ScoreValue).HasColumnType("decimal(5,2)");
                entity.HasOne(x => x.Report)
                    .WithMany()
                    .HasForeignKey(x => x.ReportId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(x => x.IndicatorDefinition)
                    .WithMany()
                    .HasForeignKey(x => x.IndicatorDefinitionId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(x => x.ScoringScale)
                    .WithMany()
                    .HasForeignKey(x => x.ScoringScaleId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<ReportPatternScore>(entity =>
            {
                entity.ToTable("ReportPatternScore", "report");
                entity.HasKey(x => x.ReportPatternScoreId);
                entity.Property(x => x.ScoreValue).HasColumnType("decimal(5,2)");
                entity.HasOne(x => x.Report)
                    .WithMany()
                    .HasForeignKey(x => x.ReportId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(x => x.BehavioralPattern)
                    .WithMany()
                    .HasForeignKey(x => x.BehavioralPatternId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<ReportKeyMoment>(entity =>
            {
                entity.ToTable("ReportKeyMoment", "report");
                entity.HasKey(x => x.ReportKeyMomentId);
                entity.HasOne(x => x.Report)
                    .WithMany()
                    .HasForeignKey(x => x.ReportId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(x => x.KeyMoment)
                    .WithMany()
                    .HasForeignKey(x => x.KeyMomentId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<GrowthMapEntry>(entity =>
            {
                entity.ToTable("GrowthMapEntry", "report");
                entity.HasKey(x => x.GrowthMapEntryId);
                entity.Property(x => x.Area).IsRequired();
                entity.HasOne(x => x.Report)
                    .WithMany()
                    .HasForeignKey(x => x.ReportId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(x => x.IndicatorDefinition)
                    .WithMany()
                    .HasForeignKey(x => x.IndicatorDefinitionId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<ActionPlanItem>(entity =>
            {
                entity.ToTable("ActionPlanItem", "report");
                entity.HasKey(x => x.ActionPlanItemId);
                entity.Property(x => x.ActionDescription).IsRequired();
                entity.HasOne(x => x.Report)
                    .WithMany()
                    .HasForeignKey(x => x.ReportId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(x => x.IndicatorDefinition)
                    .WithMany()
                    .HasForeignKey(x => x.IndicatorDefinitionId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

        }
    }
}