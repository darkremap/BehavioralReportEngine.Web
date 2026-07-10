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
    public class ReportTemplateModuleController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReportTemplateModuleController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /ReportTemplateModules
        public async Task<IActionResult> Index()
        {
            var data = _context.ReportTemplateModules.Include(x => x.ReportTemplate).Include(x => x.ReportModuleType);
            return View(await data.ToListAsync());
        }

        // GET: /ReportTemplateModules/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var item = await _context.ReportTemplateModules.Include(x => x.ReportTemplate).Include(x => x.ReportModuleType).FirstOrDefaultAsync(x => x.ReportTemplateModuleId == id);
            if (item == null) return NotFound();
            return View(item);
        }

        // GET: /ReportTemplateModules/Create
        public IActionResult Create()
        {
            ViewBag.ReportTemplateId = new SelectList(_context.ReportTemplates.OrderBy(x => x.TemplateName), "ReportTemplateId", "TemplateName");
            ViewBag.ReportModuleTypeId = new SelectList(_context.ReportModuleTypes.OrderBy(x => x.Name), "ReportModuleTypeId", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ReportTemplateModuleId,ReportTemplateId,ReportModuleTypeId,DisplayOrder,IsRequired,ConfigJson")] ReportTemplateModule model)
        {
            if (ModelState.IsValid)
            {
                model.CreatedAt = DateTime.UtcNow;
                _context.Add(model);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.ReportTemplateId = new SelectList(_context.ReportTemplates.OrderBy(x => x.TemplateName), "ReportTemplateId", "TemplateName");
            ViewBag.ReportModuleTypeId = new SelectList(_context.ReportModuleTypes.OrderBy(x => x.Name), "ReportModuleTypeId", "Name");
            return View(model);
        }

        // GET: /ReportTemplateModules/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var model = await _context.ReportTemplateModules.FindAsync(id);
            if (model == null) return NotFound();
            ViewBag.ReportTemplateId = new SelectList(_context.ReportTemplates.OrderBy(x => x.TemplateName), "ReportTemplateId", "TemplateName");
            ViewBag.ReportModuleTypeId = new SelectList(_context.ReportModuleTypes.OrderBy(x => x.Name), "ReportModuleTypeId", "Name");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ReportTemplateModuleId,ReportTemplateId,ReportModuleTypeId,DisplayOrder,IsRequired,ConfigJson")] ReportTemplateModule model)
        {
            if (id != model.ReportTemplateModuleId) return NotFound();
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(model);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.ReportTemplateModules.Any(x => x.ReportTemplateModuleId == id)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewBag.ReportTemplateId = new SelectList(_context.ReportTemplates.OrderBy(x => x.TemplateName), "ReportTemplateId", "TemplateName");
            ViewBag.ReportModuleTypeId = new SelectList(_context.ReportModuleTypes.OrderBy(x => x.Name), "ReportModuleTypeId", "Name");
            return View(model);
        }

        // GET: /ReportTemplateModules/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _context.ReportTemplateModules.Include(x => x.ReportTemplate).Include(x => x.ReportModuleType).FirstOrDefaultAsync(x => x.ReportTemplateModuleId == id);
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var model = await _context.ReportTemplateModules.FindAsync(id);
            if (model != null)
            {
                _context.ReportTemplateModules.Remove(model);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}