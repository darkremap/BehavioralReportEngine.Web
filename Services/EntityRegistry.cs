using BehavioralReportEngine.Web.Models;

namespace BehavioralReportEngine.Web.Services
{
    public record EntityRegistration(string Name, Type ClrType, string NavGroup, string DisplayLabel);

    // Mechanically derived from the old MVC nav menu (Views/Shared/_Layout.cshtml) and
    // Data/ApplicationDbContext.cs's DbSet list - one entry per entity, used both to build
    // the nav menu and to resolve the "{EntityName}" route segment to a CLR type at runtime.
    public static class EntityRegistry
    {
        public static readonly IReadOnlyList<EntityRegistration> All = new List<EntityRegistration>
        {
            // Core (Org & Users)
            new("Organization", typeof(Organization), "Core (Org & Users)", "Organization"),
            new("UserRole", typeof(UserRole), "Core (Org & Users)", "User Role"),
            new("AppUser", typeof(AppUser), "Core (Org & Users)", "App User"),
            new("AppUserRole", typeof(AppUserRole), "Core (Org & Users)", "App User Role"),
            new("Department", typeof(Department), "Core (Org & Users)", "Department"),
            new("Participant", typeof(Participant), "Core (Org & Users)", "Participant"),

            // Game Definitions
            new("GameType", typeof(GameType), "Game Definitions", "Game Type"),
            new("Game", typeof(Game), "Game Definitions", "Game"),
            new("GameVersion", typeof(GameVersion), "Game Definitions", "Game Version"),
            new("ScoringScale", typeof(ScoringScale), "Game Definitions", "Scoring Scale"),
            new("ScoringScaleLevel", typeof(ScoringScaleLevel), "Game Definitions", "Scoring Scale Level"),
            new("IndicatorDefinition", typeof(IndicatorDefinition), "Game Definitions", "Indicator Definition"),
            new("BehavioralPattern", typeof(BehavioralPattern), "Game Definitions", "Behavioral Pattern"),
            new("GamePattern", typeof(GamePattern), "Game Definitions", "Game Pattern"),

            // Sessions
            new("GameSession", typeof(GameSession), "Sessions", "Game Session"),
            new("SessionFacilitator", typeof(SessionFacilitator), "Sessions", "Session Facilitator"),
            new("Team", typeof(Team), "Sessions", "Team"),
            new("SessionParticipant", typeof(SessionParticipant), "Sessions", "Session Participant"),
            new("MediaAsset", typeof(MediaAsset), "Sessions", "Media Asset"),
            new("Transcript", typeof(Transcript), "Sessions", "Transcript"),
            new("TranscriptSegment", typeof(TranscriptSegment), "Sessions", "Transcript Segment"),
            new("Observation", typeof(Observation), "Sessions", "Observation"),
            new("KeyMoment", typeof(KeyMoment), "Sessions", "Key Moment"),

            // Report Templates
            new("ReportModuleType", typeof(ReportModuleType), "Report Templates", "Report Module Type"),
            new("ReportTemplate", typeof(ReportTemplate), "Report Templates", "Report Template"),
            new("ReportTemplateModule", typeof(ReportTemplateModule), "Report Templates", "Report Template Module"),

            // Reports
            new("Report", typeof(Report), "Reports", "Report"),
            new("ReportModule", typeof(ReportModule), "Reports", "Report Module"),
            new("ReportIndicatorScore", typeof(ReportIndicatorScore), "Reports", "Report Indicator Score"),
            new("ReportPatternScore", typeof(ReportPatternScore), "Reports", "Report Pattern Score"),
            new("ReportKeyMoment", typeof(ReportKeyMoment), "Reports", "Report Key Moment"),
            new("GrowthMapEntry", typeof(GrowthMapEntry), "Reports", "Growth Map Entry"),
            new("ActionPlanItem", typeof(ActionPlanItem), "Reports", "Action Plan Item"),
        };

        // Friendly text to show for a FK dropdown / details link, since EF's own metadata has
        // no notion of a "display" property - this is the one thing that can't be inferred.
        public static readonly IReadOnlyDictionary<Type, string> DisplayPropertyOverrides = new Dictionary<Type, string>
        {
            [typeof(Organization)] = nameof(Organization.Name),
            [typeof(UserRole)] = nameof(UserRole.RoleName),
            [typeof(AppUser)] = nameof(AppUser.FullName),
            [typeof(Department)] = nameof(Department.NameFa),
            [typeof(Participant)] = nameof(Participant.FirstNameFa),
            [typeof(GameType)] = nameof(GameType.NameFa),
            [typeof(Game)] = nameof(Game.NameFa),
            [typeof(GameVersion)] = nameof(GameVersion.VersionNumber),
            [typeof(ScoringScale)] = nameof(ScoringScale.Name),
            [typeof(IndicatorDefinition)] = nameof(IndicatorDefinition.Title),
            [typeof(BehavioralPattern)] = nameof(BehavioralPattern.Name),
            [typeof(GameSession)] = nameof(GameSession.SessionName),
            [typeof(Team)] = nameof(Team.TeamName),
            [typeof(MediaAsset)] = nameof(MediaAsset.FileName),
            [typeof(KeyMoment)] = nameof(KeyMoment.Title),
            [typeof(ReportModuleType)] = nameof(ReportModuleType.Name),
            [typeof(ReportTemplate)] = nameof(ReportTemplate.TemplateName),
            [typeof(Report)] = nameof(Report.ReportTitle),
        };

        public static EntityRegistration? Resolve(string name) =>
            All.FirstOrDefault(e => string.Equals(e.Name, name, StringComparison.OrdinalIgnoreCase));
    }
}
