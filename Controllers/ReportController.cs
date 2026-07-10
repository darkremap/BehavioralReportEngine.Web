using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BehavioralReportEngine.Web.Data;
using BehavioralReportEngine.Web.Models;
using BehavioralReportEngine.Web.ViewModels;

namespace BehavioralReportEngine.Web.Controllers
{
    public class ReportController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReportController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Reports
        public async Task<IActionResult> Index()
        {
            var data = _context.Reports.Include(x => x.ReportTemplate).Include(x => x.GameSession).Include(x => x.Participant).Include(x => x.Team).Include(x => x.Organization).Include(x => x.GameVersion).Include(x => x.AppUser);
            return View(await data.ToListAsync());
        }

        // GET: /Reports/Details/5
        public async Task<IActionResult> Details(long id)
        {
            var item = await _context.Reports.Include(x => x.ReportTemplate).Include(x => x.GameSession).Include(x => x.Participant).Include(x => x.Team).Include(x => x.Organization).Include(x => x.GameVersion).Include(x => x.AppUser).FirstOrDefaultAsync(x => x.ReportId == id);
            if (item == null) return NotFound();
            return View(item);
        }

        // GET: /Reports/Create
        public IActionResult Create(int? participantId)
        {
            ViewBag.ReportTemplateId = new SelectList(_context.ReportTemplates.OrderBy(x => x.TemplateName), "ReportTemplateId", "TemplateName");
            ViewBag.SessionId = new SelectList(_context.GameSessions.OrderBy(x => x.SessionName), "SessionId", "SessionName");
            ViewBag.ParticipantId = new SelectList(_context.Participants.OrderBy(x => x.FirstNameFa), "ParticipantId", "FirstNameFa", participantId);
            ViewBag.TeamId = new SelectList(_context.Teams.OrderBy(x => x.TeamName), "TeamId", "TeamName");
            ViewBag.OrganizationId = new SelectList(_context.Organizations.OrderBy(x => x.Name), "OrganizationId", "Name");
            ViewBag.GameVersionId = new SelectList(_context.GameVersions.OrderBy(x => x.VersionNumber), "GameVersionId", "VersionNumber");
            ViewBag.GeneratedByUserId = new SelectList(_context.AppUsers.OrderBy(x => x.FullName), "UserId", "FullName");
            var model = participantId.HasValue ? new Report { ParticipantId = participantId } : null;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ReportId,ReportScope,ReportTemplateId,SessionId,ParticipantId,TeamId,OrganizationId,GameVersionId,ReportTitle,GeneratedAt,GeneratedByUserId,GenerationMethod,Status")] Report model)
        {
            if (ModelState.IsValid)
            {
                model.CreatedAt = DateTime.UtcNow;
                _context.Add(model);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.ReportTemplateId = new SelectList(_context.ReportTemplates.OrderBy(x => x.TemplateName), "ReportTemplateId", "TemplateName");
            ViewBag.SessionId = new SelectList(_context.GameSessions.OrderBy(x => x.SessionName), "SessionId", "SessionName");
            ViewBag.ParticipantId = new SelectList(_context.Participants.OrderBy(x => x.FirstNameFa), "ParticipantId", "FirstNameFa");
            ViewBag.TeamId = new SelectList(_context.Teams.OrderBy(x => x.TeamName), "TeamId", "TeamName");
            ViewBag.OrganizationId = new SelectList(_context.Organizations.OrderBy(x => x.Name), "OrganizationId", "Name");
            ViewBag.GameVersionId = new SelectList(_context.GameVersions.OrderBy(x => x.VersionNumber), "GameVersionId", "VersionNumber");
            ViewBag.GeneratedByUserId = new SelectList(_context.AppUsers.OrderBy(x => x.FullName), "UserId", "FullName");
            return View(model);
        }

        // GET: /Reports/Print/5
        public async Task<IActionResult> Print(long id)
        {
            var report = await _context.Reports
                .Include(x => x.Organization)
                .Include(x => x.Participant)
                .Include(x => x.GameSession)
                .Include(x => x.GameVersion).ThenInclude(x => x.Game)
                .Include(x => x.ReportTemplate)
                .FirstOrDefaultAsync(x => x.ReportId == id);
            if (report == null) return NotFound();

            var viewModel = new ReportPrintViewModel
            {
                Report = report,
                IndicatorScores = await _context.ReportIndicatorScores
                    .Include(x => x.IndicatorDefinition).ThenInclude(x => x.ScoringScale)
                    .Where(x => x.ReportId == id)
                    .OrderBy(x => x.IndicatorDefinition.DisplayOrder)
                    .ToListAsync(),
                PatternScores = await _context.ReportPatternScores
                    .Include(x => x.BehavioralPattern)
                    .Where(x => x.ReportId == id)
                    .OrderByDescending(x => x.ScoreValue)
                    .ToListAsync(),
                KeyMoments = await _context.ReportKeyMoments
                    .Include(x => x.KeyMoment)
                    .Where(x => x.ReportId == id)
                    .OrderBy(x => x.DisplayOrder)
                    .ToListAsync(),
                GrowthMapEntries = await _context.GrowthMapEntries
                    .Where(x => x.ReportId == id)
                    .OrderBy(x => x.DisplayOrder)
                    .ToListAsync(),
                ActionPlanItems = await _context.ActionPlanItems
                    .Where(x => x.ReportId == id)
                    .OrderBy(x => x.DisplayOrder)
                    .ToListAsync(),
            };

            return View(viewModel);
        }

        // GET: /Reports/Edit/5
        public async Task<IActionResult> Edit(long id)
        {
            var model = await _context.Reports.FindAsync(id);
            if (model == null) return NotFound();
            ViewBag.ReportTemplateId = new SelectList(_context.ReportTemplates.OrderBy(x => x.TemplateName), "ReportTemplateId", "TemplateName");
            ViewBag.SessionId = new SelectList(_context.GameSessions.OrderBy(x => x.SessionName), "SessionId", "SessionName");
            ViewBag.ParticipantId = new SelectList(_context.Participants.OrderBy(x => x.FirstNameFa), "ParticipantId", "FirstNameFa");
            ViewBag.TeamId = new SelectList(_context.Teams.OrderBy(x => x.TeamName), "TeamId", "TeamName");
            ViewBag.OrganizationId = new SelectList(_context.Organizations.OrderBy(x => x.Name), "OrganizationId", "Name");
            ViewBag.GameVersionId = new SelectList(_context.GameVersions.OrderBy(x => x.VersionNumber), "GameVersionId", "VersionNumber");
            ViewBag.GeneratedByUserId = new SelectList(_context.AppUsers.OrderBy(x => x.FullName), "UserId", "FullName");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("ReportId,ReportScope,ReportTemplateId,SessionId,ParticipantId,TeamId,OrganizationId,GameVersionId,ReportTitle,GeneratedAt,GeneratedByUserId,GenerationMethod,Status")] Report model)
        {
            if (id != model.ReportId) return NotFound();
            if (ModelState.IsValid)
            {
                try
                {
                    model.UpdatedAt = DateTime.UtcNow;
                    _context.Update(model);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Reports.Any(x => x.ReportId == id)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewBag.ReportTemplateId = new SelectList(_context.ReportTemplates.OrderBy(x => x.TemplateName), "ReportTemplateId", "TemplateName");
            ViewBag.SessionId = new SelectList(_context.GameSessions.OrderBy(x => x.SessionName), "SessionId", "SessionName");
            ViewBag.ParticipantId = new SelectList(_context.Participants.OrderBy(x => x.FirstNameFa), "ParticipantId", "FirstNameFa");
            ViewBag.TeamId = new SelectList(_context.Teams.OrderBy(x => x.TeamName), "TeamId", "TeamName");
            ViewBag.OrganizationId = new SelectList(_context.Organizations.OrderBy(x => x.Name), "OrganizationId", "Name");
            ViewBag.GameVersionId = new SelectList(_context.GameVersions.OrderBy(x => x.VersionNumber), "GameVersionId", "VersionNumber");
            ViewBag.GeneratedByUserId = new SelectList(_context.AppUsers.OrderBy(x => x.FullName), "UserId", "FullName");
            return View(model);
        }

        // GET: /Reports/Delete/5
        public async Task<IActionResult> Delete(long id)
        {
            var item = await _context.Reports.Include(x => x.ReportTemplate).Include(x => x.GameSession).Include(x => x.Participant).Include(x => x.Team).Include(x => x.Organization).Include(x => x.GameVersion).Include(x => x.AppUser).FirstOrDefaultAsync(x => x.ReportId == id);
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var model = await _context.Reports.FindAsync(id);
            if (model != null)
            {
                _context.Reports.Remove(model);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}