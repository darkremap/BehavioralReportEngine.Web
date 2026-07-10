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
    public class BehavioralPatternController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BehavioralPatternController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /BehavioralPatterns
        public async Task<IActionResult> Index()
        {
            var data = _context.BehavioralPatterns;
            return View(await data.ToListAsync());
        }

        // GET: /BehavioralPatterns/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var item = await _context.BehavioralPatterns.FirstOrDefaultAsync(x => x.BehavioralPatternId == id);
            if (item == null) return NotFound();
            return View(item);
        }

        // GET: /BehavioralPatterns/Create
        public IActionResult Create()
        {

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BehavioralPatternId,PatternCode,Name,NameEn,Description,IsGlobal")] BehavioralPattern model)
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

        // GET: /BehavioralPatterns/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var model = await _context.BehavioralPatterns.FindAsync(id);
            if (model == null) return NotFound();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BehavioralPatternId,PatternCode,Name,NameEn,Description,IsGlobal")] BehavioralPattern model)
        {
            if (id != model.BehavioralPatternId) return NotFound();
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
                    if (!_context.BehavioralPatterns.Any(x => x.BehavioralPatternId == id)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        // GET: /BehavioralPatterns/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _context.BehavioralPatterns.FirstOrDefaultAsync(x => x.BehavioralPatternId == id);
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var model = await _context.BehavioralPatterns.FindAsync(id);
            if (model != null)
            {
                _context.BehavioralPatterns.Remove(model);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}