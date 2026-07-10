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
    public class KeyMomentController : Controller
    {
        private readonly ApplicationDbContext _context;

        public KeyMomentController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /KeyMoments
        public async Task<IActionResult> Index()
        {
            var data = _context.KeyMoments.Include(x => x.GameSession).Include(x => x.Team).Include(x => x.Participant).Include(x => x.MediaAsset).Include(x => x.TranscriptSegment);
            return View(await data.ToListAsync());
        }

        // GET: /KeyMoments/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var item = await _context.KeyMoments.Include(x => x.GameSession).Include(x => x.Team).Include(x => x.Participant).Include(x => x.MediaAsset).Include(x => x.TranscriptSegment).FirstOrDefaultAsync(x => x.KeyMomentId == id);
            if (item == null) return NotFound();
            return View(item);
        }

        // GET: /KeyMoments/Create
        public IActionResult Create()
        {
            ViewBag.SessionId = new SelectList(_context.GameSessions.OrderBy(x => x.SessionName), "SessionId", "SessionName");
            ViewBag.TeamId = new SelectList(_context.Teams.OrderBy(x => x.TeamName), "TeamId", "TeamName");
            ViewBag.ParticipantId = new SelectList(_context.Participants.OrderBy(x => x.FirstNameFa), "ParticipantId", "FirstNameFa");
            ViewBag.MediaAssetId = new SelectList(_context.MediaAssets.OrderBy(x => x.FileName), "MediaAssetId", "FileName");
            ViewBag.TranscriptSegmentId = new SelectList(_context.TranscriptSegments.OrderBy(x => x.TranscriptSegmentId), "TranscriptSegmentId", "TranscriptSegmentId");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("KeyMomentId,SessionId,TeamId,ParticipantId,MediaAssetId,TranscriptSegmentId,TimestampMs,Title,Description,Significance")] KeyMoment model)
        {
            if (ModelState.IsValid)
            {
                model.CreatedAt = DateTime.UtcNow;
                _context.Add(model);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.SessionId = new SelectList(_context.GameSessions.OrderBy(x => x.SessionName), "SessionId", "SessionName");
            ViewBag.TeamId = new SelectList(_context.Teams.OrderBy(x => x.TeamName), "TeamId", "TeamName");
            ViewBag.ParticipantId = new SelectList(_context.Participants.OrderBy(x => x.FirstNameFa), "ParticipantId", "FirstNameFa");
            ViewBag.MediaAssetId = new SelectList(_context.MediaAssets.OrderBy(x => x.FileName), "MediaAssetId", "FileName");
            ViewBag.TranscriptSegmentId = new SelectList(_context.TranscriptSegments.OrderBy(x => x.TranscriptSegmentId), "TranscriptSegmentId", "TranscriptSegmentId");
            return View(model);
        }

        // GET: /KeyMoments/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var model = await _context.KeyMoments.FindAsync(id);
            if (model == null) return NotFound();
            ViewBag.SessionId = new SelectList(_context.GameSessions.OrderBy(x => x.SessionName), "SessionId", "SessionName");
            ViewBag.TeamId = new SelectList(_context.Teams.OrderBy(x => x.TeamName), "TeamId", "TeamName");
            ViewBag.ParticipantId = new SelectList(_context.Participants.OrderBy(x => x.FirstNameFa), "ParticipantId", "FirstNameFa");
            ViewBag.MediaAssetId = new SelectList(_context.MediaAssets.OrderBy(x => x.FileName), "MediaAssetId", "FileName");
            ViewBag.TranscriptSegmentId = new SelectList(_context.TranscriptSegments.OrderBy(x => x.TranscriptSegmentId), "TranscriptSegmentId", "TranscriptSegmentId");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("KeyMomentId,SessionId,TeamId,ParticipantId,MediaAssetId,TranscriptSegmentId,TimestampMs,Title,Description,Significance")] KeyMoment model)
        {
            if (id != model.KeyMomentId) return NotFound();
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(model);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.KeyMoments.Any(x => x.KeyMomentId == id)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewBag.SessionId = new SelectList(_context.GameSessions.OrderBy(x => x.SessionName), "SessionId", "SessionName");
            ViewBag.TeamId = new SelectList(_context.Teams.OrderBy(x => x.TeamName), "TeamId", "TeamName");
            ViewBag.ParticipantId = new SelectList(_context.Participants.OrderBy(x => x.FirstNameFa), "ParticipantId", "FirstNameFa");
            ViewBag.MediaAssetId = new SelectList(_context.MediaAssets.OrderBy(x => x.FileName), "MediaAssetId", "FileName");
            ViewBag.TranscriptSegmentId = new SelectList(_context.TranscriptSegments.OrderBy(x => x.TranscriptSegmentId), "TranscriptSegmentId", "TranscriptSegmentId");
            return View(model);
        }

        // GET: /KeyMoments/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _context.KeyMoments.Include(x => x.GameSession).Include(x => x.Team).Include(x => x.Participant).Include(x => x.MediaAsset).Include(x => x.TranscriptSegment).FirstOrDefaultAsync(x => x.KeyMomentId == id);
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var model = await _context.KeyMoments.FindAsync(id);
            if (model != null)
            {
                _context.KeyMoments.Remove(model);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}