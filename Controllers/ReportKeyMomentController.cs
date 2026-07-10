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
    public class ReportKeyMomentController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReportKeyMomentController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /ReportKeyMoments
        public async Task<IActionResult> Index()
        {
            var data = _context.ReportKeyMoments.Include(x => x.Report).Include(x => x.KeyMoment);
            return View(await data.ToListAsync());
        }

        // GET: /ReportKeyMoments/Details/5
        public async Task<IActionResult> Details(long id)
        {
            var item = await _context.ReportKeyMoments.Include(x => x.Report).Include(x => x.KeyMoment).FirstOrDefaultAsync(x => x.ReportKeyMomentId == id);
            if (item == null) return NotFound();
            return View(item);
        }

        // GET: /ReportKeyMoments/Create
        public IActionResult Create()
        {
            ViewBag.ReportId = new SelectList(_context.Reports.OrderBy(x => x.ReportTitle), "ReportId", "ReportTitle");
            ViewBag.KeyMomentId = new SelectList(_context.KeyMoments.OrderBy(x => x.Title), "KeyMomentId", "Title");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ReportKeyMomentId,ReportId,KeyMomentId,DisplayOrder,NarrativeText")] ReportKeyMoment model)
        {
            if (ModelState.IsValid)
            {
                model.CreatedAt = DateTime.UtcNow;
                _context.Add(model);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.ReportId = new SelectList(_context.Reports.OrderBy(x => x.ReportTitle), "ReportId", "ReportTitle");
            ViewBag.KeyMomentId = new SelectList(_context.KeyMoments.OrderBy(x => x.Title), "KeyMomentId", "Title");
            return View(model);
        }

        // GET: /ReportKeyMoments/Edit/5
        public async Task<IActionResult> Edit(long id)
        {
            var model = await _context.ReportKeyMoments.FindAsync(id);
            if (model == null) return NotFound();
            ViewBag.ReportId = new SelectList(_context.Reports.OrderBy(x => x.ReportTitle), "ReportId", "ReportTitle");
            ViewBag.KeyMomentId = new SelectList(_context.KeyMoments.OrderBy(x => x.Title), "KeyMomentId", "Title");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("ReportKeyMomentId,ReportId,KeyMomentId,DisplayOrder,NarrativeText")] ReportKeyMoment model)
        {
            if (id != model.ReportKeyMomentId) return NotFound();
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(model);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.ReportKeyMoments.Any(x => x.ReportKeyMomentId == id)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewBag.ReportId = new SelectList(_context.Reports.OrderBy(x => x.ReportTitle), "ReportId", "ReportTitle");
            ViewBag.KeyMomentId = new SelectList(_context.KeyMoments.OrderBy(x => x.Title), "KeyMomentId", "Title");
            return View(model);
        }

        // GET: /ReportKeyMoments/Delete/5
        public async Task<IActionResult> Delete(long id)
        {
            var item = await _context.ReportKeyMoments.Include(x => x.Report).Include(x => x.KeyMoment).FirstOrDefaultAsync(x => x.ReportKeyMomentId == id);
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var model = await _context.ReportKeyMoments.FindAsync(id);
            if (model != null)
            {
                _context.ReportKeyMoments.Remove(model);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}