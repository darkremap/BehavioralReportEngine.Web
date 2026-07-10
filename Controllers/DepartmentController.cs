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
    public class DepartmentController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DepartmentController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Departments
        public async Task<IActionResult> Index()
        {
            var data = _context.Departments;
            return View(await data.ToListAsync());
        }

        // GET: /Departments/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var item = await _context.Departments.FirstOrDefaultAsync(x => x.DepartmentId == id);
            if (item == null) return NotFound();
            return View(item);
        }

        // GET: /Departments/Create
        public IActionResult Create()
        {

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DepartmentId,DepartmentCode,NameFa,NameEn,Description,IsActive")] Department model)
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

        // GET: /Departments/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var model = await _context.Departments.FindAsync(id);
            if (model == null) return NotFound();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("DepartmentId,DepartmentCode,NameFa,NameEn,Description,IsActive")] Department model)
        {
            if (id != model.DepartmentId) return NotFound();
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
                    if (!_context.Departments.Any(x => x.DepartmentId == id)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        // GET: /Departments/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _context.Departments.FirstOrDefaultAsync(x => x.DepartmentId == id);
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var model = await _context.Departments.FindAsync(id);
            if (model != null)
            {
                _context.Departments.Remove(model);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}