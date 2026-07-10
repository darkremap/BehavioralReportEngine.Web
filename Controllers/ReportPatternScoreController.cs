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
    public class ReportPatternScoreController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReportPatternScoreController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /ReportPatternScores
        public async Task<IActionResult> Index()
        {
            var data = _context.ReportPatternScores.Include(x => x.Report).Include(x => x.BehavioralPattern);
            return View(await data.ToListAsync());
        }

        // GET: /ReportPatternScores/Details/5
        public async Task<IActionResult> Details(long id)
        {
            var item = await _context.ReportPatternScores.Include(x => x.Report).Include(x => x.BehavioralPattern).FirstOrDefaultAsync(x => x.ReportPatternScoreId == id);
            if (item == null) return NotFound();
            return View(item);
        }

        // GET: /ReportPatternScores/Create
        public IActionResult Create()
        {
            ViewBag.ReportId = new SelectList(_context.Reports.OrderBy(x => x.ReportTitle), "ReportId", "ReportTitle");
            ViewBag.BehavioralPatternId = new SelectList(_context.BehavioralPatterns.OrderBy(x => x.NameEn), "BehavioralPatternId", "NameEn");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ReportPatternScoreId,ReportId,BehavioralPatternId,ScoreValue,IsDominantPattern,Description")] ReportPatternScore model)
        {
            if (ModelState.IsValid)
            {
                model.CreatedAt = DateTime.UtcNow;
                _context.Add(model);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.ReportId = new SelectList(_context.Reports.OrderBy(x => x.ReportTitle), "ReportId", "ReportTitle");
            ViewBag.BehavioralPatternId = new SelectList(_context.BehavioralPatterns.OrderBy(x => x.NameEn), "BehavioralPatternId", "NameEn");
            return View(model);
        }

        // GET: /ReportPatternScores/Edit/5
        public async Task<IActionResult> Edit(long id)
        {
            var model = await _context.ReportPatternScores.FindAsync(id);
            if (model == null) return NotFound();
            ViewBag.ReportId = new SelectList(_context.Reports.OrderBy(x => x.ReportTitle), "ReportId", "ReportTitle");
            ViewBag.BehavioralPatternId = new SelectList(_context.BehavioralPatterns.OrderBy(x => x.NameEn), "BehavioralPatternId", "NameEn");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("ReportPatternScoreId,ReportId,BehavioralPatternId,ScoreValue,IsDominantPattern,Description")] ReportPatternScore model)
        {
            if (id != model.ReportPatternScoreId) return NotFound();
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(model);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.ReportPatternScores.Any(x => x.ReportPatternScoreId == id)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewBag.ReportId = new SelectList(_context.Reports.OrderBy(x => x.ReportTitle), "ReportId", "ReportTitle");
            ViewBag.BehavioralPatternId = new SelectList(_context.BehavioralPatterns.OrderBy(x => x.NameEn), "BehavioralPatternId", "NameEn");
            return View(model);
        }

        // GET: /ReportPatternScores/Delete/5
        public async Task<IActionResult> Delete(long id)
        {
            var item = await _context.ReportPatternScores.Include(x => x.Report).Include(x => x.BehavioralPattern).FirstOrDefaultAsync(x => x.ReportPatternScoreId == id);
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var model = await _context.ReportPatternScores.FindAsync(id);
            if (model != null)
            {
                _context.ReportPatternScores.Remove(model);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}