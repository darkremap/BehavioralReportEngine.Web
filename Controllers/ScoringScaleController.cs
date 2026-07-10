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
    public class ScoringScaleController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ScoringScaleController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /ScoringScales
        public async Task<IActionResult> Index()
        {
            var data = _context.ScoringScales;
            return View(await data.ToListAsync());
        }

        // GET: /ScoringScales/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var item = await _context.ScoringScales.FirstOrDefaultAsync(x => x.ScoringScaleId == id);
            if (item == null) return NotFound();
            return View(item);
        }

        // GET: /ScoringScales/Create
        public IActionResult Create()
        {

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ScoringScaleId,ScaleCode,Name,MinValue,MaxValue,StepValue,Description")] ScoringScale model)
        {
            if (ModelState.IsValid)
            {
                model.CreatedAt = DateTime.UtcNow;
                _context.Add(model);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        // GET: /ScoringScales/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var model = await _context.ScoringScales.FindAsync(id);
            if (model == null) return NotFound();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ScoringScaleId,ScaleCode,Name,MinValue,MaxValue,StepValue,Description")] ScoringScale model)
        {
            if (id != model.ScoringScaleId) return NotFound();
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
                    if (!_context.ScoringScales.Any(x => x.ScoringScaleId == id)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        // GET: /ScoringScales/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _context.ScoringScales.FirstOrDefaultAsync(x => x.ScoringScaleId == id);
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var model = await _context.ScoringScales.FindAsync(id);
            if (model != null)
            {
                _context.ScoringScales.Remove(model);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}