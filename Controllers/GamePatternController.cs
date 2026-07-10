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
    public class GamePatternController : Controller
    {
        private readonly ApplicationDbContext _context;

        public GamePatternController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /GamePatterns
        public async Task<IActionResult> Index()
        {
            var data = _context.GamePatterns.Include(x => x.GameVersion).Include(x => x.BehavioralPattern);
            return View(await data.ToListAsync());
        }

        // GET: /GamePatterns/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var item = await _context.GamePatterns.Include(x => x.GameVersion).Include(x => x.BehavioralPattern).FirstOrDefaultAsync(x => x.GamePatternId == id);
            if (item == null) return NotFound();
            return View(item);
        }

        // GET: /GamePatterns/Create
        public IActionResult Create()
        {
            ViewBag.GameVersionId = new SelectList(_context.GameVersions.OrderBy(x => x.VersionNumber), "GameVersionId", "VersionNumber");
            ViewBag.BehavioralPatternId = new SelectList(_context.BehavioralPatterns.OrderBy(x => x.NameEn), "BehavioralPatternId", "NameEn");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("GamePatternId,GameVersionId,BehavioralPatternId,CustomName,CustomDescription,DisplayOrder")] GamePattern model)
        {
            if (ModelState.IsValid)
            {
                model.CreatedAt = DateTime.UtcNow;
                _context.Add(model);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.GameVersionId = new SelectList(_context.GameVersions.OrderBy(x => x.VersionNumber), "GameVersionId", "VersionNumber");
            ViewBag.BehavioralPatternId = new SelectList(_context.BehavioralPatterns.OrderBy(x => x.NameEn), "BehavioralPatternId", "NameEn");
            return View(model);
        }

        // GET: /GamePatterns/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var model = await _context.GamePatterns.FindAsync(id);
            if (model == null) return NotFound();
            ViewBag.GameVersionId = new SelectList(_context.GameVersions.OrderBy(x => x.VersionNumber), "GameVersionId", "VersionNumber");
            ViewBag.BehavioralPatternId = new SelectList(_context.BehavioralPatterns.OrderBy(x => x.NameEn), "BehavioralPatternId", "NameEn");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("GamePatternId,GameVersionId,BehavioralPatternId,CustomName,CustomDescription,DisplayOrder")] GamePattern model)
        {
            if (id != model.GamePatternId) return NotFound();
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(model);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.GamePatterns.Any(x => x.GamePatternId == id)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewBag.GameVersionId = new SelectList(_context.GameVersions.OrderBy(x => x.VersionNumber), "GameVersionId", "VersionNumber");
            ViewBag.BehavioralPatternId = new SelectList(_context.BehavioralPatterns.OrderBy(x => x.NameEn), "BehavioralPatternId", "NameEn");
            return View(model);
        }

        // GET: /GamePatterns/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _context.GamePatterns.Include(x => x.GameVersion).Include(x => x.BehavioralPattern).FirstOrDefaultAsync(x => x.GamePatternId == id);
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var model = await _context.GamePatterns.FindAsync(id);
            if (model != null)
            {
                _context.GamePatterns.Remove(model);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}