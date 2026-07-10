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
    public class IndicatorDefinitionController : Controller
    {
        private readonly ApplicationDbContext _context;

        public IndicatorDefinitionController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /IndicatorDefinitions
        public async Task<IActionResult> Index()
        {
            var data = _context.IndicatorDefinitions.Include(x => x.GameVersion).Include(x => x.ScoringScale);
            return View(await data.ToListAsync());
        }

        // GET: /IndicatorDefinitions/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var item = await _context.IndicatorDefinitions.Include(x => x.GameVersion).Include(x => x.ScoringScale).FirstOrDefaultAsync(x => x.IndicatorDefinitionId == id);
            if (item == null) return NotFound();
            return View(item);
        }

        // GET: /IndicatorDefinitions/Create
        public IActionResult Create()
        {
            ViewBag.GameVersionId = new SelectList(_context.GameVersions.OrderBy(x => x.VersionNumber), "GameVersionId", "VersionNumber");
            ViewBag.ScoringScaleId = new SelectList(_context.ScoringScales.OrderBy(x => x.Name), "ScoringScaleId", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IndicatorDefinitionId,GameVersionId,IndicatorCode,Title,TitleEn,ShortDefinition,DisplayOrder,ScoringScaleId,IsActive")] IndicatorDefinition model)
        {
            if (ModelState.IsValid)
            {
                model.CreatedAt = DateTime.UtcNow;
                _context.Add(model);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.GameVersionId = new SelectList(_context.GameVersions.OrderBy(x => x.VersionNumber), "GameVersionId", "VersionNumber");
            ViewBag.ScoringScaleId = new SelectList(_context.ScoringScales.OrderBy(x => x.Name), "ScoringScaleId", "Name");
            return View(model);
        }

        // GET: /IndicatorDefinitions/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var model = await _context.IndicatorDefinitions.FindAsync(id);
            if (model == null) return NotFound();
            ViewBag.GameVersionId = new SelectList(_context.GameVersions.OrderBy(x => x.VersionNumber), "GameVersionId", "VersionNumber");
            ViewBag.ScoringScaleId = new SelectList(_context.ScoringScales.OrderBy(x => x.Name), "ScoringScaleId", "Name");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IndicatorDefinitionId,GameVersionId,IndicatorCode,Title,TitleEn,ShortDefinition,DisplayOrder,ScoringScaleId,IsActive")] IndicatorDefinition model)
        {
            if (id != model.IndicatorDefinitionId) return NotFound();
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
                    if (!_context.IndicatorDefinitions.Any(x => x.IndicatorDefinitionId == id)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewBag.GameVersionId = new SelectList(_context.GameVersions.OrderBy(x => x.VersionNumber), "GameVersionId", "VersionNumber");
            ViewBag.ScoringScaleId = new SelectList(_context.ScoringScales.OrderBy(x => x.Name), "ScoringScaleId", "Name");
            return View(model);
        }

        // GET: /IndicatorDefinitions/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _context.IndicatorDefinitions.Include(x => x.GameVersion).Include(x => x.ScoringScale).FirstOrDefaultAsync(x => x.IndicatorDefinitionId == id);
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var model = await _context.IndicatorDefinitions.FindAsync(id);
            if (model != null)
            {
                _context.IndicatorDefinitions.Remove(model);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}