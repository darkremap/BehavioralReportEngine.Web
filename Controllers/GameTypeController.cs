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
    public class GameTypeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public GameTypeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /GameTypes
        public async Task<IActionResult> Index()
        {
            var data = _context.GameTypes;
            return View(await data.ToListAsync());
        }

        // GET: /GameTypes/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var item = await _context.GameTypes.FirstOrDefaultAsync(x => x.GameTypeId == id);
            if (item == null) return NotFound();
            return View(item);
        }

        // GET: /GameTypes/Create
        public IActionResult Create()
        {

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("GameTypeId,TypeCode,NameFa,NameEn,Description,IsActive")] GameType model)
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

        // GET: /GameTypes/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var model = await _context.GameTypes.FindAsync(id);
            if (model == null) return NotFound();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("GameTypeId,TypeCode,NameFa,NameEn,Description,IsActive")] GameType model)
        {
            if (id != model.GameTypeId) return NotFound();
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
                    if (!_context.GameTypes.Any(x => x.GameTypeId == id)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        // GET: /GameTypes/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _context.GameTypes.FirstOrDefaultAsync(x => x.GameTypeId == id);
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var model = await _context.GameTypes.FindAsync(id);
            if (model != null)
            {
                _context.GameTypes.Remove(model);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}