using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BehavioralReportEngine.Web.Data;
using BehavioralReportEngine.Web.Models;

namespace BehavioralReportEngine.Web.Controllers
{
    public class ObservationController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ObservationController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Observations
        public async Task<IActionResult> Index()
        {
            var data = _context.Observations.Include(x => x.GameSession).Include(x => x.Participant).Include(x => x.Team).Include(x => x.AppUser).Include(x => x.IndicatorDefinition).Include(x => x.BehavioralPattern);
            return View(await data.ToListAsync());
        }

        // GET: /Observations/Details/5
        public async Task<IActionResult> Details(long id)
        {
            var item = await _context.Observations.Include(x => x.GameSession).Include(x => x.Participant).Include(x => x.Team).Include(x => x.AppUser).Include(x => x.IndicatorDefinition).Include(x => x.BehavioralPattern).FirstOrDefaultAsync(x => x.ObservationId == id);
            if (item == null) return NotFound();
            return View(item);
        }

        // GET: /Observations/Create
        public IActionResult Create()
        {
            ViewBag.SessionId = new SelectList(_context.GameSessions.OrderBy(x => x.SessionName), "SessionId", "SessionName");
            ViewBag.ParticipantId = new SelectList(_context.Participants.OrderBy(x => x.FirstNameFa), "ParticipantId", "FirstNameFa");
            ViewBag.TeamId = new SelectList(_context.Teams.OrderBy(x => x.TeamName), "TeamId", "TeamName");
            ViewBag.ObservedByUserId = new SelectList(_context.AppUsers.OrderBy(x => x.FullName), "UserId", "FullName");
            ViewBag.IndicatorDefinitionId = new SelectList(_context.IndicatorDefinitions.OrderBy(x => x.TitleEn), "IndicatorDefinitionId", "TitleEn");
            ViewBag.BehavioralPatternId = new SelectList(_context.BehavioralPatterns.OrderBy(x => x.NameEn), "BehavioralPatternId", "NameEn");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ObservationId,SessionId,ParticipantId,TeamId,ObservedByUserId,IndicatorDefinitionId,BehavioralPatternId,ObservationTimeMs,ObservationText")] Observation model)
        {
            if (ModelState.IsValid)
            {
                model.CreatedAt = DateTime.UtcNow;
                _context.Add(model);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.SessionId = new SelectList(_context.GameSessions.OrderBy(x => x.SessionName), "SessionId", "SessionName");
            ViewBag.ParticipantId = new SelectList(_context.Participants.OrderBy(x => x.FirstNameFa), "ParticipantId", "FirstNameFa");
            ViewBag.TeamId = new SelectList(_context.Teams.OrderBy(x => x.TeamName), "TeamId", "TeamName");
            ViewBag.ObservedByUserId = new SelectList(_context.AppUsers.OrderBy(x => x.FullName), "UserId", "FullName");
            ViewBag.IndicatorDefinitionId = new SelectList(_context.IndicatorDefinitions.OrderBy(x => x.TitleEn), "IndicatorDefinitionId", "TitleEn");
            ViewBag.BehavioralPatternId = new SelectList(_context.BehavioralPatterns.OrderBy(x => x.NameEn), "BehavioralPatternId", "NameEn");
            return View(model);
        }

        // GET: /Observations/Edit/5
        public async Task<IActionResult> Edit(long id)
        {
            var model = await _context.Observations.FindAsync(id);
            if (model == null) return NotFound();
            ViewBag.SessionId = new SelectList(_context.GameSessions.OrderBy(x => x.SessionName), "SessionId", "SessionName");
            ViewBag.ParticipantId = new SelectList(_context.Participants.OrderBy(x => x.FirstNameFa), "ParticipantId", "FirstNameFa");
            ViewBag.TeamId = new SelectList(_context.Teams.OrderBy(x => x.TeamName), "TeamId", "TeamName");
            ViewBag.ObservedByUserId = new SelectList(_context.AppUsers.OrderBy(x => x.FullName), "UserId", "FullName");
            ViewBag.IndicatorDefinitionId = new SelectList(_context.IndicatorDefinitions.OrderBy(x => x.TitleEn), "IndicatorDefinitionId", "TitleEn");
            ViewBag.BehavioralPatternId = new SelectList(_context.BehavioralPatterns.OrderBy(x => x.NameEn), "BehavioralPatternId", "NameEn");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("ObservationId,SessionId,ParticipantId,TeamId,ObservedByUserId,IndicatorDefinitionId,BehavioralPatternId,ObservationTimeMs,ObservationText")] Observation model)
        {
            if (id != model.ObservationId) return NotFound();
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(model);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Observations.Any(x => x.ObservationId == id)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewBag.SessionId = new SelectList(_context.GameSessions.OrderBy(x => x.SessionName), "SessionId", "SessionName");
            ViewBag.ParticipantId = new SelectList(_context.Participants.OrderBy(x => x.FirstNameFa), "ParticipantId", "FirstNameFa");
            ViewBag.TeamId = new SelectList(_context.Teams.OrderBy(x => x.TeamName), "TeamId", "TeamName");
            ViewBag.ObservedByUserId = new SelectList(_context.AppUsers.OrderBy(x => x.FullName), "UserId", "FullName");
            ViewBag.IndicatorDefinitionId = new SelectList(_context.IndicatorDefinitions.OrderBy(x => x.TitleEn), "IndicatorDefinitionId", "TitleEn");
            ViewBag.BehavioralPatternId = new SelectList(_context.BehavioralPatterns.OrderBy(x => x.NameEn), "BehavioralPatternId", "NameEn");
            return View(model);
        }

        // GET: /Observations/Delete/5
        public async Task<IActionResult> Delete(long id)
        {
            var item = await _context.Observations.Include(x => x.GameSession).Include(x => x.Participant).Include(x => x.Team).Include(x => x.AppUser).Include(x => x.IndicatorDefinition).Include(x => x.BehavioralPattern).FirstOrDefaultAsync(x => x.ObservationId == id);
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var model = await _context.Observations.FindAsync(id);
            if (model != null)
            {
                _context.Observations.Remove(model);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}