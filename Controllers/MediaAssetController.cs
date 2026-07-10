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
    public class MediaAssetController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MediaAssetController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /MediaAssets
        public async Task<IActionResult> Index()
        {
            var data = _context.MediaAssets.Include(x => x.GameSession);
            return View(await data.ToListAsync());
        }

        // GET: /MediaAssets/Details/5
        public async Task<IActionResult> Details(long id)
        {
            var item = await _context.MediaAssets.Include(x => x.GameSession).FirstOrDefaultAsync(x => x.MediaAssetId == id);
            if (item == null) return NotFound();
            return View(item);
        }

        // GET: /MediaAssets/Create
        public IActionResult Create()
        {
            ViewBag.SessionId = new SelectList(_context.GameSessions.OrderBy(x => x.SessionName), "SessionId", "SessionName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MediaAssetId,SessionId,MediaType,FileName,StoragePath,DurationSeconds,RecordedAt")] MediaAsset model)
        {
            if (ModelState.IsValid)
            {
                model.CreatedAt = DateTime.UtcNow;
                _context.Add(model);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.SessionId = new SelectList(_context.GameSessions.OrderBy(x => x.SessionName), "SessionId", "SessionName");
            return View(model);
        }

        // GET: /MediaAssets/Edit/5
        public async Task<IActionResult> Edit(long id)
        {
            var model = await _context.MediaAssets.FindAsync(id);
            if (model == null) return NotFound();
            ViewBag.SessionId = new SelectList(_context.GameSessions.OrderBy(x => x.SessionName), "SessionId", "SessionName");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("MediaAssetId,SessionId,MediaType,FileName,StoragePath,DurationSeconds,RecordedAt")] MediaAsset model)
        {
            if (id != model.MediaAssetId) return NotFound();
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(model);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.MediaAssets.Any(x => x.MediaAssetId == id)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewBag.SessionId = new SelectList(_context.GameSessions.OrderBy(x => x.SessionName), "SessionId", "SessionName");
            return View(model);
        }

        // GET: /MediaAssets/Delete/5
        public async Task<IActionResult> Delete(long id)
        {
            var item = await _context.MediaAssets.Include(x => x.GameSession).FirstOrDefaultAsync(x => x.MediaAssetId == id);
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var model = await _context.MediaAssets.FindAsync(id);
            if (model != null)
            {
                _context.MediaAssets.Remove(model);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}