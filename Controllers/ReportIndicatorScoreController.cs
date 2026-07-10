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
    public class ReportIndicatorScoreController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReportIndicatorScoreController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /ReportIndicatorScores
        public async Task<IActionResult> Index()
        {
            var data = _context.ReportIndicatorScores.Include(x => x.Report).Include(x => x.IndicatorDefinition).Include(x => x.ScoringScale);
            return View(await data.ToListAsync());
        }

        // GET: /ReportIndicatorScores/Details/5
        public async Task<IActionResult> Details(long id)
        {
            var item = await _context.ReportIndicatorScores.Include(x => x.Report).Include(x => x.IndicatorDefinition).Include(x => x.ScoringScale).FirstOrDefaultAsync(x => x.ReportIndicatorScoreId == id);
            if (item == null) return NotFound();
            return View(item);
        }

        // GET: /ReportIndicatorScores/Create
        public IActionResult Create()
        {
            ViewBag.ReportId = new SelectList(_context.Reports.OrderBy(x => x.ReportTitle), "ReportId", "ReportTitle");
            ViewBag.IndicatorDefinitionId = new SelectList(_context.IndicatorDefinitions.OrderBy(x => x.TitleEn), "IndicatorDefinitionId", "TitleEn");
            ViewBag.ScoringScaleId = new SelectList(_context.ScoringScales.OrderBy(x => x.Name), "ScoringScaleId", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ReportIndicatorScoreId,ReportId,IndicatorDefinitionId,ScoreValue,ScoringScaleId,ObservedEvidence,IndicatorBehaviors,GrowthOpportunities,Strengths,PracticalRecommendation,IsAiGenerated")] ReportIndicatorScore model)
        {
            if (ModelState.IsValid)
            {
                model.CreatedAt = DateTime.UtcNow;
                _context.Add(model);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.ReportId = new SelectList(_context.Reports.OrderBy(x => x.ReportTitle), "ReportId", "ReportTitle");
            ViewBag.IndicatorDefinitionId = new SelectList(_context.IndicatorDefinitions.OrderBy(x => x.TitleEn), "IndicatorDefinitionId", "TitleEn");
            ViewBag.ScoringScaleId = new SelectList(_context.ScoringScales.OrderBy(x => x.Name), "ScoringScaleId", "Name");
            return View(model);
        }

        // GET: /ReportIndicatorScores/Edit/5
        public async Task<IActionResult> Edit(long id)
        {
            var model = await _context.ReportIndicatorScores.FindAsync(id);
            if (model == null) return NotFound();
            ViewBag.ReportId = new SelectList(_context.Reports.OrderBy(x => x.ReportTitle), "ReportId", "ReportTitle");
            ViewBag.IndicatorDefinitionId = new SelectList(_context.IndicatorDefinitions.OrderBy(x => x.TitleEn), "IndicatorDefinitionId", "TitleEn");
            ViewBag.ScoringScaleId = new SelectList(_context.ScoringScales.OrderBy(x => x.Name), "ScoringScaleId", "Name");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("ReportIndicatorScoreId,ReportId,IndicatorDefinitionId,ScoreValue,ScoringScaleId,ObservedEvidence,IndicatorBehaviors,GrowthOpportunities,Strengths,PracticalRecommendation,IsAiGenerated")] ReportIndicatorScore model)
        {
            if (id != model.ReportIndicatorScoreId) return NotFound();
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
                    if (!_context.ReportIndicatorScores.Any(x => x.ReportIndicatorScoreId == id)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewBag.ReportId = new SelectList(_context.Reports.OrderBy(x => x.ReportTitle), "ReportId", "ReportTitle");
            ViewBag.IndicatorDefinitionId = new SelectList(_context.IndicatorDefinitions.OrderBy(x => x.TitleEn), "IndicatorDefinitionId", "TitleEn");
            ViewBag.ScoringScaleId = new SelectList(_context.ScoringScales.OrderBy(x => x.Name), "ScoringScaleId", "Name");
            return View(model);
        }

        // GET: /ReportIndicatorScores/Delete/5
        public async Task<IActionResult> Delete(long id)
        {
            var item = await _context.ReportIndicatorScores.Include(x => x.Report).Include(x => x.IndicatorDefinition).Include(x => x.ScoringScale).FirstOrDefaultAsync(x => x.ReportIndicatorScoreId == id);
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var model = await _context.ReportIndicatorScores.FindAsync(id);
            if (model != null)
            {
                _context.ReportIndicatorScores.Remove(model);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}