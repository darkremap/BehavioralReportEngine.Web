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
    public class GameVersionController : Controller
    {
        private readonly ApplicationDbContext _context;

        public GameVersionController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /GameVersions
        public async Task<IActionResult> Index()
        {
            var data = _context.GameVersions.Include(x => x.Game);
            return View(await data.ToListAsync());
        }

        // GET: /GameVersions/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var item = await _context.GameVersions.Include(x => x.Game).FirstOrDefaultAsync(x => x.GameVersionId == id);
            if (item == null) return NotFound();
            return View(item);
        }

        // GET: /GameVersions/Create
        public IActionResult Create()
        {
            ViewBag.GameId = new SelectList(_context.Games.OrderBy(x => x.NameFa), "GameId", "NameFa");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("GameVersionId,GameId,VersionNumber,VersionLabel,EffectiveFrom,EffectiveTo,IsCurrent,ReleaseNotes")] GameVersion model)
        {
            if (ModelState.IsValid)
            {
                model.CreatedAt = DateTime.UtcNow;
                _context.Add(model);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.GameId = new SelectList(_context.Games.OrderBy(x => x.NameFa), "GameId", "NameFa");
            return View(model);
        }

        // GET: /GameVersions/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var model = await _context.GameVersions.FindAsync(id);
            if (model == null) return NotFound();
            ViewBag.GameId = new SelectList(_context.Games.OrderBy(x => x.NameFa), "GameId", "NameFa");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("GameVersionId,GameId,VersionNumber,VersionLabel,EffectiveFrom,EffectiveTo,IsCurrent,ReleaseNotes")] GameVersion model)
        {
            if (id != model.GameVersionId) return NotFound();
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
                    if (!_context.GameVersions.Any(x => x.GameVersionId == id)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewBag.GameId = new SelectList(_context.Games.OrderBy(x => x.NameFa), "GameId", "NameFa");
            return View(model);
        }

        // GET: /GameVersions/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _context.GameVersions.Include(x => x.Game).FirstOrDefaultAsync(x => x.GameVersionId == id);
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var model = await _context.GameVersions.FindAsync(id);
            if (model != null)
            {
                _context.GameVersions.Remove(model);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}