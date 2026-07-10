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
    public class TeamController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TeamController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Team
        public async Task<IActionResult> Index()
        {
            var teams = await _context.Teams
                .Include(t => t.GameSession)
                .ToListAsync();

            return View(teams);
        }

        // GET: Team/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var team = await _context.Teams
                .Include(t => t.GameSession)
                .FirstOrDefaultAsync(t => t.TeamId == id);

            if (team == null)
                return NotFound();

            return View(team);
        }

        // GET: Team/Create
        public IActionResult Create()
        {
            LoadSessions();
            return View();
        }

        // POST: Team/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SessionId,TeamName,TeamCode")] Team model)
        {
            if (ModelState.IsValid)
            {
                model.CreatedAt = DateTime.UtcNow;

                _context.Teams.Add(model);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            LoadSessions(model.SessionId);
            return View(model);
        }

        // GET: Team/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var team = await _context.Teams.FindAsync(id);

            if (team == null)
                return NotFound();

            LoadSessions(team.SessionId);

            return View(team);
        }

        // POST: Team/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TeamId,SessionId,TeamName,TeamCode")] Team model)
        {
            if (id != model.TeamId)
                return NotFound();

            if (ModelState.IsValid)
            {
                var team = await _context.Teams.FindAsync(id);

                if (team == null)
                    return NotFound();

                team.SessionId = model.SessionId;
                team.TeamName = model.TeamName;
                team.TeamCode = model.TeamCode;

                // اگر در مدل دارید
                // team.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            LoadSessions(model.SessionId);
            return View(model);
        }

        // GET: Team/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var team = await _context.Teams
                .Include(t => t.GameSession)
                .FirstOrDefaultAsync(t => t.TeamId == id);

            if (team == null)
                return NotFound();

            return View(team);
        }

        // POST: Team/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var team = await _context.Teams.FindAsync(id);

            if (team != null)
            {
                _context.Teams.Remove(team);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private void LoadSessions(int? selectedSession = null)
        {
            ViewBag.SessionId = new SelectList(
                _context.GameSessions.OrderBy(s => s.SessionName),
                "SessionId",
                "SessionName",
                selectedSession);
        }
    }
}