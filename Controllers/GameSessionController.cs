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
    public class GameSessionController : Controller
    {
        private readonly ApplicationDbContext _context;

        public GameSessionController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /GameSessions
        public async Task<IActionResult> Index()
        {
            var data = _context.GameSessions.Include(x => x.Organization).Include(x => x.GameVersion);
            return View(await data.ToListAsync());
        }

        // GET: /GameSessions/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var item = await _context.GameSessions.Include(x => x.Organization).Include(x => x.GameVersion).FirstOrDefaultAsync(x => x.SessionId == id);
            if (item == null) return NotFound();
            return View(item);
        }

        // GET: /GameSessions/Create
        public IActionResult Create()
        {
            ViewBag.OrganizationId = new SelectList(_context.Organizations.OrderBy(x => x.Name), "OrganizationId", "Name");
            ViewBag.GameVersionId = new SelectList(_context.GameVersions.OrderBy(x => x.VersionNumber), "GameVersionId", "VersionNumber");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SessionId,OrganizationId,GameVersionId,SessionCode,SessionName,SessionDate,StartTime,EndTime,Location,Status")] GameSession model)
        {
            if (ModelState.IsValid)
            {
                model.CreatedAt = DateTime.UtcNow;
                _context.Add(model);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.OrganizationId = new SelectList(_context.Organizations.OrderBy(x => x.Name), "OrganizationId", "Name");
            ViewBag.GameVersionId = new SelectList(_context.GameVersions.OrderBy(x => x.VersionNumber), "GameVersionId", "VersionNumber");
            return View(model);
        }

        // GET: /GameSessions/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var model = await _context.GameSessions.FindAsync(id);
            if (model == null) return NotFound();
            ViewBag.OrganizationId = new SelectList(_context.Organizations.OrderBy(x => x.Name), "OrganizationId", "Name");
            ViewBag.GameVersionId = new SelectList(_context.GameVersions.OrderBy(x => x.VersionNumber), "GameVersionId", "VersionNumber");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("SessionId,OrganizationId,GameVersionId,SessionCode,SessionName,SessionDate,StartTime,EndTime,Location,Status")] GameSession model)
        {
            if (id != model.SessionId) return NotFound();
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
                    if (!_context.GameSessions.Any(x => x.SessionId == id)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewBag.OrganizationId = new SelectList(_context.Organizations.OrderBy(x => x.Name), "OrganizationId", "Name");
            ViewBag.GameVersionId = new SelectList(_context.GameVersions.OrderBy(x => x.VersionNumber), "GameVersionId", "VersionNumber");
            return View(model);
        }

        // GET: /GameSessions/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _context.GameSessions.Include(x => x.Organization).Include(x => x.GameVersion).FirstOrDefaultAsync(x => x.SessionId == id);
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var model = await _context.GameSessions.FindAsync(id);
            if (model != null)
            {
                _context.GameSessions.Remove(model);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}