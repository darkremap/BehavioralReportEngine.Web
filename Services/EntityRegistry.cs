using MudBlazor;
using BehavioralReportEngine.Web.Models;

namespace BehavioralReportEngine.Web.Services
{
    public record EntityRegistration(string Name, Type ClrType, string NavGroup, string DisplayLabel, string Icon);

    // Mechanically derived from the old MVC nav menu (Views/Shared/_Layout.cshtml) and
    // Data/ApplicationDbContext.cs's DbSet list - one entry per entity, used both to build
    // the nav menu and to resolve the "{EntityName}" route segment to a CLR type at runtime.
    public static class EntityRegistry
    {
        public static readonly IReadOnlyDictionary<string, string> NavGroupIcons = new Dictionary<string, string>
        {
            ["Core (Org & Users)"] = Icons.Material.Filled.Groups,
            ["Game Definitions"] = Icons.Material.Filled.SportsEsports,
            ["Sessions"] = Icons.Material.Filled.EventNote,
            ["Report Templates"] = Icons.Material.Filled.Description,
            ["Reports"] = Icons.Material.Filled.Assessment,
        };

        public static readonly IReadOnlyList<EntityRegistration> All = new List<EntityRegistration>
        {
            // Core (Org & Users)
            new("Organization", typeof(Organization), "Core (Org & Users)", "Organization", Icons.Material.Filled.Business),
            new("UserRole", typeof(UserRole), "Core (Org & Users)", "User Role", Icons.Material.Filled.AdminPanelSettings),
            new("AppUser", typeof(AppUser), "Core (Org & Users)", "App User", Icons.Material.Filled.Person),
            new("AppUserRole", typeof(AppUserRole), "Core (Org & Users)", "App User Role", Icons.Material.Filled.AssignmentInd),
            new("Department", typeof(Department), "Core (Org & Users)", "Department", Icons.Material.Filled.AccountTree),
            new("Participant", typeof(Participant), "Core (Org & Users)", "Participant", Icons.Material.Filled.PeopleAlt),

            // Game Definitions
            new("GameType", typeof(GameType), "Game Definitions", "Game Type", Icons.Material.Filled.Category),
            new("Game", typeof(Game), "Game Definitions", "Game", Icons.Material.Filled.Casino),
            new("GameVersion", typeof(GameVersion), "Game Definitions", "Game Version", Icons.Material.Filled.Layers),
            new("ScoringScale", typeof(ScoringScale), "Game Definitions", "Scoring Scale", Icons.Material.Filled.Straighten),
            new("ScoringScaleLevel", typeof(ScoringScaleLevel), "Game Definitions", "Scoring Scale Level", Icons.Material.Filled.LinearScale),
            new("IndicatorDefinition", typeof(IndicatorDefinition), "Game Definitions", "Indicator Definition", Icons.Material.Filled.TrackChanges),
            new("BehavioralPattern", typeof(BehavioralPattern), "Game Definitions", "Behavioral Pattern", Icons.Material.Filled.Psychology),
            new("GamePattern", typeof(GamePattern), "Game Definitions", "Game Pattern", Icons.Material.Filled.Extension),

            // Sessions
            new("GameSession", typeof(GameSession), "Sessions", "Game Session", Icons.Material.Filled.Event),
            new("SessionFacilitator", typeof(SessionFacilitator), "Sessions", "Session Facilitator", Icons.Material.Filled.SupervisorAccount),
            new("Team", typeof(Team), "Sessions", "Team", Icons.Material.Filled.Groups3),
            new("SessionParticipant", typeof(SessionParticipant), "Sessions", "Session Participant", Icons.Material.Filled.PersonAdd),
            new("MediaAsset", typeof(MediaAsset), "Sessions", "Media Asset", Icons.Material.Filled.PermMedia),
            new("Transcript", typeof(Transcript), "Sessions", "Transcript", Icons.Material.Filled.Article),
            new("TranscriptSegment", typeof(TranscriptSegment), "Sessions", "Transcript Segment", Icons.Material.Filled.ShortText),
            new("Observation", typeof(Observation), "Sessions", "Observation", Icons.Material.Filled.Visibility),
            new("KeyMoment", typeof(KeyMoment), "Sessions", "Key Moment", Icons.Material.Filled.Star),

            // Report Templates
            new("ReportModuleType", typeof(ReportModuleType), "Report Templates", "Report Module Type", Icons.Material.Filled.ViewModule),
            new("ReportTemplate", typeof(ReportTemplate), "Report Templates", "Report Template", Icons.Material.Filled.Description),
            new("ReportTemplateModule", typeof(ReportTemplateModule), "Report Templates", "Report Template Module", Icons.Material.Filled.ViewList),

            // Reports
            new("Report", typeof(Report), "Reports", "Report", Icons.Material.Filled.Assessment),
            new("ReportModule", typeof(ReportModule), "Reports", "Report Module", Icons.Material.Filled.ViewAgenda),
            new("ReportIndicatorScore", typeof(ReportIndicatorScore), "Reports", "Report Indicator Score", Icons.Material.Filled.BarChart),
            new("ReportPatternScore", typeof(ReportPatternScore), "Reports", "Report Pattern Score", Icons.Material.Filled.Insights),
            new("ReportKeyMoment", typeof(ReportKeyMoment), "Reports", "Report Key Moment", Icons.Material.Filled.StarRate),
            new("GrowthMapEntry", typeof(GrowthMapEntry), "Reports", "Growth Map Entry", Icons.Material.Filled.TrendingUp),
            new("ActionPlanItem", typeof(ActionPlanItem), "Reports", "Action Plan Item", Icons.Material.Filled.Checklist),
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
