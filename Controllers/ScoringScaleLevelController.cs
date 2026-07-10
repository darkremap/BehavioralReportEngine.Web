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
    public class ScoringScaleLevelController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ScoringScaleLevelController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /ScoringScaleLevels
        public async Task<IActionResult> Index()
        {
            var data = _context.ScoringScaleLevels.Include(x => x.ScoringScale);
            return View(await data.ToListAsync());
        }

        // GET: /ScoringScaleLevels/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var item = await _context.ScoringScaleLevels.Include(x => x.ScoringScale).FirstOrDefaultAsync(x => x.ScoringScaleLevelId == id);
            if (item == null) return NotFound();
            return View(item);
        }

        // GET: /ScoringScaleLevels/Create
        public IActionResult Create()
        {
            ViewBag.ScoringScaleId = new SelectList(_context.ScoringScales.OrderBy(x => x.Name), "ScoringScaleId", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ScoringScaleLevelId,ScoringScaleId,LevelValue,LevelLabel,LevelDescription")] ScoringScaleLevel model)
        {
            if (ModelState.IsValid)
            {
                _context.Add(model);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.ScoringScaleId = new SelectList(_context.ScoringScales.OrderBy(x => x.Name), "ScoringScaleId", "Name");
            return View(model);
        }

        // GET: /ScoringScaleLevels/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var model = await _context.ScoringScaleLevels.FindAsync(id);
            if (model == null) return NotFound();
            ViewBag.ScoringScaleId = new SelectList(_context.ScoringScales.OrderBy(x => x.Name), "ScoringScaleId", "Name");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ScoringScaleLevelId,ScoringScaleId,LevelValue,LevelLabel,LevelDescription")] ScoringScaleLevel model)
        {
            if (id != model.ScoringScaleLevelId) return NotFound();
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(model);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.ScoringScaleLevels.Any(x => x.ScoringScaleLevelId == id)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewBag.ScoringScaleId = new SelectList(_context.ScoringScales.OrderBy(x => x.Name), "ScoringScaleId", "Name");
            return View(model);
        }

        // GET: /ScoringScaleLevels/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _context.ScoringScaleLevels.Include(x => x.ScoringScale).FirstOrDefaultAsync(x => x.ScoringScaleLevelId == id);
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var model = await _context.ScoringScaleLevels.FindAsync(id);
            if (model != null)
            {
                _context.ScoringScaleLevels.Remove(model);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}