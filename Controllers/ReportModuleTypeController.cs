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
    public class ReportModuleTypeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReportModuleTypeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /ReportModuleTypes
        public async Task<IActionResult> Index()
        {
            var data = _context.ReportModuleTypes;
            return View(await data.ToListAsync());
        }

        // GET: /ReportModuleTypes/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var item = await _context.ReportModuleTypes.FirstOrDefaultAsync(x => x.ReportModuleTypeId == id);
            if (item == null) return NotFound();
            return View(item);
        }

        // GET: /ReportModuleTypes/Create
        public IActionResult Create()
        {

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ReportModuleTypeId,ModuleCode,Name,Description")] ReportModuleType model)
        {
            if (ModelState.IsValid)
            {
                _context.Add(model);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        // GET: /ReportModuleTypes/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var model = await _context.ReportModuleTypes.FindAsync(id);
            if (model == null) return NotFound();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ReportModuleTypeId,ModuleCode,Name,Description")] ReportModuleType model)
        {
            if (id != model.ReportModuleTypeId) return NotFound();
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(model);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.ReportModuleTypes.Any(x => x.ReportModuleTypeId == id)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        // GET: /ReportModuleTypes/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _context.ReportModuleTypes.FirstOrDefaultAsync(x => x.ReportModuleTypeId == id);
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var model = await _context.ReportModuleTypes.FindAsync(id);
            if (model != null)
            {
                _context.ReportModuleTypes.Remove(model);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}