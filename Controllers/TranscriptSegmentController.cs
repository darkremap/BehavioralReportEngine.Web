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
    public class TranscriptSegmentController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TranscriptSegmentController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /TranscriptSegments
        public async Task<IActionResult> Index()
        {
            var data = _context.TranscriptSegments.Include(x => x.Transcript).Include(x => x.Participant);
            return View(await data.ToListAsync());
        }

        // GET: /TranscriptSegments/Details/5
        public async Task<IActionResult> Details(long id)
        {
            var item = await _context.TranscriptSegments.Include(x => x.Transcript).Include(x => x.Participant).FirstOrDefaultAsync(x => x.TranscriptSegmentId == id);
            if (item == null) return NotFound();
            return View(item);
        }

        // GET: /TranscriptSegments/Create
        public IActionResult Create()
        {
            ViewBag.TranscriptId = new SelectList(_context.Transcripts.OrderBy(x => x.TranscriptId), "TranscriptId", "TranscriptId");
            ViewBag.SpeakerParticipantId = new SelectList(_context.Participants.OrderBy(x => x.FirstNameFa), "ParticipantId", "FirstNameFa");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TranscriptSegmentId,TranscriptId,SpeakerParticipantId,SpeakerLabel,StartTimeMs,EndTimeMs,SegmentText")] TranscriptSegment model)
        {
            if (ModelState.IsValid)
            {
                model.CreatedAt = DateTime.UtcNow;
                _context.Add(model);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.TranscriptId = new SelectList(_context.Transcripts.OrderBy(x => x.TranscriptId), "TranscriptId", "TranscriptId");
            ViewBag.SpeakerParticipantId = new SelectList(_context.Participants.OrderBy(x => x.FirstNameFa), "ParticipantId", "FirstNameFa");
            return View(model);
        }

        // GET: /TranscriptSegments/Edit/5
        public async Task<IActionResult> Edit(long id)
        {
            var model = await _context.TranscriptSegments.FindAsync(id);
            if (model == null) return NotFound();
            ViewBag.TranscriptId = new SelectList(_context.Transcripts.OrderBy(x => x.TranscriptId), "TranscriptId", "TranscriptId");
            ViewBag.SpeakerParticipantId = new SelectList(_context.Participants.OrderBy(x => x.FirstNameFa), "ParticipantId", "FirstNameFa");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("TranscriptSegmentId,TranscriptId,SpeakerParticipantId,SpeakerLabel,StartTimeMs,EndTimeMs,SegmentText")] TranscriptSegment model)
        {
            if (id != model.TranscriptSegmentId) return NotFound();
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(model);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.TranscriptSegments.Any(x => x.TranscriptSegmentId == id)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewBag.TranscriptId = new SelectList(_context.Transcripts.OrderBy(x => x.TranscriptId), "TranscriptId", "TranscriptId");
            ViewBag.SpeakerParticipantId = new SelectList(_context.Participants.OrderBy(x => x.FirstNameFa), "ParticipantId", "FirstNameFa");
            return View(model);
        }

        // GET: /TranscriptSegments/Delete/5
        public async Task<IActionResult> Delete(long id)
        {
            var item = await _context.TranscriptSegments.Include(x => x.Transcript).Include(x => x.Participant).FirstOrDefaultAsync(x => x.TranscriptSegmentId == id);
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var model = await _context.TranscriptSegments.FindAsync(id);
            if (model != null)
            {
                _context.TranscriptSegments.Remove(model);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}