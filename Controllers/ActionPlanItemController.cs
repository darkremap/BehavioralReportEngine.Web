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
    public class ActionPlanItemController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ActionPlanItemController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /ActionPlanItems
        public async Task<IActionResult> Index()
        {
            var data = _context.ActionPlanItems.Include(x => x.Report).Include(x => x.IndicatorDefinition);
            return View(await data.ToListAsync());
        }

        // GET: /ActionPlanItems/Details/5
        public async Task<IActionResult> Details(long id)
        {
            var item = await _context.ActionPlanItems.Include(x => x.Report).Include(x => x.IndicatorDefinition).FirstOrDefaultAsync(x => x.ActionPlanItemId == id);
            if (item == null) return NotFound();
            return View(item);
        }

        // GET: /ActionPlanItems/Create
        public IActionResult Create()
        {
            ViewBag.ReportId = new SelectList(_context.Reports.OrderBy(x => x.ReportTitle), "ReportId", "ReportTitle");
            ViewBag.IndicatorDefinitionId = new SelectList(_context.IndicatorDefinitions.OrderBy(x => x.TitleEn), "IndicatorDefinitionId", "TitleEn");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ActionPlanItemId,ReportId,IndicatorDefinitionId,ActionDescription,TimeFrame,Priority,DisplayOrder")] ActionPlanItem model)
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
            return View(model);
        }

        // GET: /ActionPlanItems/Edit/5
        public async Task<IActionResult> Edit(long id)
        {
            var model = await _context.ActionPlanItems.FindAsync(id);
            if (model == null) return NotFound();
            ViewBag.ReportId = new SelectList(_context.Reports.OrderBy(x => x.ReportTitle), "ReportId", "ReportTitle");
            ViewBag.IndicatorDefinitionId = new SelectList(_context.IndicatorDefinitions.OrderBy(x => x.TitleEn), "IndicatorDefinitionId", "TitleEn");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("ActionPlanItemId,ReportId,IndicatorDefinitionId,ActionDescription,TimeFrame,Priority,DisplayOrder")] ActionPlanItem model)
        {
            if (id != model.ActionPlanItemId) return NotFound();
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(model);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.ActionPlanItems.Any(x => x.ActionPlanItemId == id)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewBag.ReportId = new SelectList(_context.Reports.OrderBy(x => x.ReportTitle), "ReportId", "ReportTitle");
            ViewBag.IndicatorDefinitionId = new SelectList(_context.IndicatorDefinitions.OrderBy(x => x.TitleEn), "IndicatorDefinitionId", "TitleEn");
            return View(model);
        }

        // GET: /ActionPlanItems/Delete/5
        public async Task<IActionResult> Delete(long id)
        {
            var item = await _context.ActionPlanItems.Include(x => x.Report).Include(x => x.IndicatorDefinition).FirstOrDefaultAsync(x => x.ActionPlanItemId == id);
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var model = await _context.ActionPlanItems.FindAsync(id);
            if (model != null)
            {
                _context.ActionPlanItems.Remove(model);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}