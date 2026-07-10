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
    public class TranscriptController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TranscriptController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Transcripts
        public async Task<IActionResult> Index()
        {
            var data = _context.Transcripts.Include(x => x.GameSession).Include(x => x.MediaAsset);
            return View(await data.ToListAsync());
        }

        // GET: /Transcripts/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var item = await _context.Transcripts.Include(x => x.GameSession).Include(x => x.MediaAsset).FirstOrDefaultAsync(x => x.TranscriptId == id);
            if (item == null) return NotFound();
            return View(item);
        }

        // GET: /Transcripts/Create
        public IActionResult Create()
        {
            ViewBag.SessionId = new SelectList(_context.GameSessions.OrderBy(x => x.SessionName), "SessionId", "SessionName");
            ViewBag.MediaAssetId = new SelectList(_context.MediaAssets.OrderBy(x => x.FileName), "MediaAssetId", "FileName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TranscriptId,SessionId,MediaAssetId,Language,FullText")] Transcript model)
        {
            if (ModelState.IsValid)
            {
                model.CreatedAt = DateTime.UtcNow;
                _context.Add(model);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.SessionId = new SelectList(_context.GameSessions.OrderBy(x => x.SessionName), "SessionId", "SessionName");
            ViewBag.MediaAssetId = new SelectList(_context.MediaAssets.OrderBy(x => x.FileName), "MediaAssetId", "FileName");
            return View(model);
        }

        // GET: /Transcripts/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var model = await _context.Transcripts.FindAsync(id);
            if (model == null) return NotFound();
            ViewBag.SessionId = new SelectList(_context.GameSessions.OrderBy(x => x.SessionName), "SessionId", "SessionName");
            ViewBag.MediaAssetId = new SelectList(_context.MediaAssets.OrderBy(x => x.FileName), "MediaAssetId", "FileName");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TranscriptId,SessionId,MediaAssetId,Language,FullText")] Transcript model)
        {
            if (id != model.TranscriptId) return NotFound();
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(model);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Transcripts.Any(x => x.TranscriptId == id)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewBag.SessionId = new SelectList(_context.GameSessions.OrderBy(x => x.SessionName), "SessionId", "SessionName");
            ViewBag.MediaAssetId = new SelectList(_context.MediaAssets.OrderBy(x => x.FileName), "MediaAssetId", "FileName");
            return View(model);
        }

        // GET: /Transcripts/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _context.Transcripts.Include(x => x.GameSession).Include(x => x.MediaAsset).FirstOrDefaultAsync(x => x.TranscriptId == id);
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var model = await _context.Transcripts.FindAsync(id);
            if (model != null)
            {
                _context.Transcripts.Remove(model);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}