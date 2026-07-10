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
    public class ReportModuleController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReportModuleController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /ReportModules
        public async Task<IActionResult> Index()
        {
            var data = _context.ReportModules.Include(x => x.Report).Include(x => x.ReportModuleType).Include(x => x.IndicatorDefinition).Include(x => x.AppUser);
            return View(await data.ToListAsync());
        }

        // GET: /ReportModules/Details/5
        public async Task<IActionResult> Details(long id)
        {
            var item = await _context.ReportModules.Include(x => x.Report).Include(x => x.ReportModuleType).Include(x => x.IndicatorDefinition).Include(x => x.AppUser).FirstOrDefaultAsync(x => x.ReportModuleId == id);
            if (item == null) return NotFound();
            return View(item);
        }

        // GET: /ReportModules/Create
        public IActionResult Create()
        {
            ViewBag.ReportId = new SelectList(_context.Reports.OrderBy(x => x.ReportTitle), "ReportId", "ReportTitle");
            ViewBag.ReportModuleTypeId = new SelectList(_context.ReportModuleTypes.OrderBy(x => x.Name), "ReportModuleTypeId", "Name");
            ViewBag.IndicatorDefinitionId = new SelectList(_context.IndicatorDefinitions.OrderBy(x => x.TitleEn), "IndicatorDefinitionId", "TitleEn");
            ViewBag.ReviewedByUserId = new SelectList(_context.AppUsers.OrderBy(x => x.FullName), "UserId", "FullName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ReportModuleId,ReportId,ReportModuleTypeId,IndicatorDefinitionId,DisplayOrder,ContentText,ContentHtml,IsAiGenerated,AiModel,AiPromptRef,ReviewedByUserId,ReviewedAt")] ReportModule model)
        {
            if (ModelState.IsValid)
            {
                model.CreatedAt = DateTime.UtcNow;
                _context.Add(model);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.ReportId = new SelectList(_context.Reports.OrderBy(x => x.ReportTitle), "ReportId", "ReportTitle");
            ViewBag.ReportModuleTypeId = new SelectList(_context.ReportModuleTypes.OrderBy(x => x.Name), "ReportModuleTypeId", "Name");
            ViewBag.IndicatorDefinitionId = new SelectList(_context.IndicatorDefinitions.OrderBy(x => x.TitleEn), "IndicatorDefinitionId", "TitleEn");
            ViewBag.ReviewedByUserId = new SelectList(_context.AppUsers.OrderBy(x => x.FullName), "UserId", "FullName");
            return View(model);
        }

        // GET: /ReportModules/Edit/5
        public async Task<IActionResult> Edit(long id)
        {
            var model = await _context.ReportModules.FindAsync(id);
            if (model == null) return NotFound();
            ViewBag.ReportId = new SelectList(_context.Reports.OrderBy(x => x.ReportTitle), "ReportId", "ReportTitle");
            ViewBag.ReportModuleTypeId = new SelectList(_context.ReportModuleTypes.OrderBy(x => x.Name), "ReportModuleTypeId", "Name");
            ViewBag.IndicatorDefinitionId = new SelectList(_context.IndicatorDefinitions.OrderBy(x => x.TitleEn), "IndicatorDefinitionId", "TitleEn");
            ViewBag.ReviewedByUserId = new SelectList(_context.AppUsers.OrderBy(x => x.FullName), "UserId", "FullName");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("ReportModuleId,ReportId,ReportModuleTypeId,IndicatorDefinitionId,DisplayOrder,ContentText,ContentHtml,IsAiGenerated,AiModel,AiPromptRef,ReviewedByUserId,ReviewedAt")] ReportModule model)
        {
            if (id != model.ReportModuleId) return NotFound();
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
                    if (!_context.ReportModules.Any(x => x.ReportModuleId == id)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewBag.ReportId = new SelectList(_context.Reports.OrderBy(x => x.ReportTitle), "ReportId", "ReportTitle");
            ViewBag.ReportModuleTypeId = new SelectList(_context.ReportModuleTypes.OrderBy(x => x.Name), "ReportModuleTypeId", "Name");
            ViewBag.IndicatorDefinitionId = new SelectList(_context.IndicatorDefinitions.OrderBy(x => x.TitleEn), "IndicatorDefinitionId", "TitleEn");
            ViewBag.ReviewedByUserId = new SelectList(_context.AppUsers.OrderBy(x => x.FullName), "UserId", "FullName");
            return View(model);
        }

        // GET: /ReportModules/Delete/5
        public async Task<IActionResult> Delete(long id)
        {
            var item = await _context.ReportModules.Include(x => x.Report).Include(x => x.ReportModuleType).Include(x => x.IndicatorDefinition).Include(x => x.AppUser).FirstOrDefaultAsync(x => x.ReportModuleId == id);
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var model = await _context.ReportModules.FindAsync(id);
            if (model != null)
            {
                _context.ReportModules.Remove(model);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}