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
    public class SessionFacilitatorController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SessionFacilitatorController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /SessionFacilitators
        public async Task<IActionResult> Index()
        {
            var data = _context.SessionFacilitators.Include(x => x.GameSession).Include(x => x.AppUser);
            return View(await data.ToListAsync());
        }

        // GET: /SessionFacilitators/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var item = await _context.SessionFacilitators.Include(x => x.GameSession).Include(x => x.AppUser).FirstOrDefaultAsync(x => x.SessionFacilitatorId == id);
            if (item == null) return NotFound();
            return View(item);
        }

        // GET: /SessionFacilitators/Create
        public IActionResult Create()
        {
            ViewBag.SessionId = new SelectList(_context.GameSessions.OrderBy(x => x.SessionName), "SessionId", "SessionName");
            ViewBag.UserId = new SelectList(_context.AppUsers.OrderBy(x => x.FullName), "UserId", "FullName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SessionFacilitatorId,SessionId,UserId,RoleInSession")] SessionFacilitator model)
        {
            if (ModelState.IsValid)
            {
                model.CreatedAt = DateTime.UtcNow;
                _context.Add(model);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.SessionId = new SelectList(_context.GameSessions.OrderBy(x => x.SessionName), "SessionId", "SessionName");
            ViewBag.UserId = new SelectList(_context.AppUsers.OrderBy(x => x.FullName), "UserId", "FullName");
            return View(model);
        }

        // GET: /SessionFacilitators/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var model = await _context.SessionFacilitators.FindAsync(id);
            if (model == null) return NotFound();
            ViewBag.SessionId = new SelectList(_context.GameSessions.OrderBy(x => x.SessionName), "SessionId", "SessionName");
            ViewBag.UserId = new SelectList(_context.AppUsers.OrderBy(x => x.FullName), "UserId", "FullName");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("SessionFacilitatorId,SessionId,UserId,RoleInSession")] SessionFacilitator model)
        {
            if (id != model.SessionFacilitatorId) return NotFound();
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(model);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.SessionFacilitators.Any(x => x.SessionFacilitatorId == id)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewBag.SessionId = new SelectList(_context.GameSessions.OrderBy(x => x.SessionName), "SessionId", "SessionName");
            ViewBag.UserId = new SelectList(_context.AppUsers.OrderBy(x => x.FullName), "UserId", "FullName");
            return View(model);
        }

        // GET: /SessionFacilitators/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _context.SessionFacilitators.Include(x => x.GameSession).Include(x => x.AppUser).FirstOrDefaultAsync(x => x.SessionFacilitatorId == id);
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var model = await _context.SessionFacilitators.FindAsync(id);
            if (model != null)
            {
                _context.SessionFacilitators.Remove(model);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}