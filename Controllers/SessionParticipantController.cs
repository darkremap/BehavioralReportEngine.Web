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
    public class SessionParticipantController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SessionParticipantController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /SessionParticipants
        public async Task<IActionResult> Index()
        {
            var data = _context.SessionParticipants.Include(x => x.GameSession).Include(x => x.Team).Include(x => x.Participant);
            return View(await data.ToListAsync());
        }

        // GET: /SessionParticipants/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var item = await _context.SessionParticipants.Include(x => x.GameSession).Include(x => x.Team).Include(x => x.Participant).FirstOrDefaultAsync(x => x.SessionParticipantId == id);
            if (item == null) return NotFound();
            return View(item);
        }

        // GET: /SessionParticipants/Create
        public IActionResult Create()
        {
            ViewBag.SessionId = new SelectList(_context.GameSessions.OrderBy(x => x.SessionName), "SessionId", "SessionName");
            ViewBag.TeamId = new SelectList(_context.Teams.OrderBy(x => x.TeamName), "TeamId", "TeamName");
            ViewBag.ParticipantId = new SelectList(_context.Participants.OrderBy(x => x.FirstNameFa), "ParticipantId", "FirstNameFa");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SessionParticipantId,SessionId,TeamId,ParticipantId")] SessionParticipant model)
        {
            if (ModelState.IsValid)
            {
                model.CreatedAt = DateTime.UtcNow;
                _context.Add(model);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.SessionId = new SelectList(_context.GameSessions.OrderBy(x => x.SessionName), "SessionId", "SessionName");
            ViewBag.TeamId = new SelectList(_context.Teams.OrderBy(x => x.TeamName), "TeamId", "TeamName");
            ViewBag.ParticipantId = new SelectList(_context.Participants.OrderBy(x => x.FirstNameFa), "ParticipantId", "FirstNameFa");
            return View(model);
        }

        // GET: /SessionParticipants/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var model = await _context.SessionParticipants.FindAsync(id);
            if (model == null) return NotFound();
            ViewBag.SessionId = new SelectList(_context.GameSessions.OrderBy(x => x.SessionName), "SessionId", "SessionName");
            ViewBag.TeamId = new SelectList(_context.Teams.OrderBy(x => x.TeamName), "TeamId", "TeamName");
            ViewBag.ParticipantId = new SelectList(_context.Participants.OrderBy(x => x.FirstNameFa), "ParticipantId", "FirstNameFa");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("SessionParticipantId,SessionId,TeamId,ParticipantId")] SessionParticipant model)
        {
            if (id != model.SessionParticipantId) return NotFound();
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(model);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.SessionParticipants.Any(x => x.SessionParticipantId == id)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewBag.SessionId = new SelectList(_context.GameSessions.OrderBy(x => x.SessionName), "SessionId", "SessionName");
            ViewBag.TeamId = new SelectList(_context.Teams.OrderBy(x => x.TeamName), "TeamId", "TeamName");
            ViewBag.ParticipantId = new SelectList(_context.Participants.OrderBy(x => x.FirstNameFa), "ParticipantId", "FirstNameFa");
            return View(model);
        }

        // GET: /SessionParticipants/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _context.SessionParticipants.Include(x => x.GameSession).Include(x => x.Team).Include(x => x.Participant).FirstOrDefaultAsync(x => x.SessionParticipantId == id);
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var model = await _context.SessionParticipants.FindAsync(id);
            if (model != null)
            {
                _context.SessionParticipants.Remove(model);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}