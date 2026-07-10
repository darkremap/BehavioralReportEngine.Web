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
    public class ReportTemplateController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReportTemplateController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /ReportTemplates
        public async Task<IActionResult> Index()
        {
            var data = _context.ReportTemplates.Include(x => x.GameVersion);
            return View(await data.ToListAsync());
        }

        // GET: /ReportTemplates/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var item = await _context.ReportTemplates.Include(x => x.GameVersion).FirstOrDefaultAsync(x => x.ReportTemplateId == id);
            if (item == null) return NotFound();
            return View(item);
        }

        // GET: /ReportTemplates/Create
        public IActionResult Create()
        {
            ViewBag.GameVersionId = new SelectList(_context.GameVersions.OrderBy(x => x.VersionNumber), "GameVersionId", "VersionNumber");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ReportTemplateId,GameVersionId,ReportScope,TemplateName,VersionNumber,IsActive")] ReportTemplate model)
        {
            if (ModelState.IsValid)
            {
                model.CreatedAt = DateTime.UtcNow;
                _context.Add(model);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.GameVersionId = new SelectList(_context.GameVersions.OrderBy(x => x.VersionNumber), "GameVersionId", "VersionNumber");
            return View(model);
        }

        // GET: /ReportTemplates/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var model = await _context.ReportTemplates.FindAsync(id);
            if (model == null) return NotFound();
            ViewBag.GameVersionId = new SelectList(_context.GameVersions.OrderBy(x => x.VersionNumber), "GameVersionId", "VersionNumber");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ReportTemplateId,GameVersionId,ReportScope,TemplateName,VersionNumber,IsActive")] ReportTemplate model)
        {
            if (id != model.ReportTemplateId) return NotFound();
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
                    if (!_context.ReportTemplates.Any(x => x.ReportTemplateId == id)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewBag.GameVersionId = new SelectList(_context.GameVersions.OrderBy(x => x.VersionNumber), "GameVersionId", "VersionNumber");
            return View(model);
        }

        // GET: /ReportTemplates/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _context.ReportTemplates.Include(x => x.GameVersion).FirstOrDefaultAsync(x => x.ReportTemplateId == id);
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var model = await _context.ReportTemplates.FindAsync(id);
            if (model != null)
            {
                _context.ReportTemplates.Remove(model);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}