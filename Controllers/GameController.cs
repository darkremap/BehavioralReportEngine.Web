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
    public class GameController : Controller
    {
        private readonly ApplicationDbContext _context;

        public GameController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Games
        public async Task<IActionResult> Index()
        {
            var data = _context.Games.Include(x => x.AppUser).Include(x => x.GameType);
            return View(await data.ToListAsync());
        }

        // GET: /Games/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var item = await _context.Games.Include(x => x.AppUser).Include(x => x.GameType).FirstOrDefaultAsync(x => x.GameId == id);
            if (item == null) return NotFound();
            return View(item);
        }

        // GET: /Games/Create
        public IActionResult Create()
        {
            ViewBag.AuthorUserId = new SelectList(_context.AppUsers.OrderBy(x => x.FullName), "UserId", "FullName");
            ViewBag.GameTypeId = new SelectList(_context.GameTypes.OrderBy(x => x.NameFa), "GameTypeId", "NameFa");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("GameId,GameCode,NameFa,NameEn,Summary,TitleFa,TitleEn,DescriptionFa,DescriptionEn,AuthorUserId,GameTypeId,DurationMinutes,IsActive")] Game model)
        {
            if (ModelState.IsValid)
            {
                model.CreatedAt = DateTime.UtcNow;
                _context.Add(model);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.AuthorUserId = new SelectList(_context.AppUsers.OrderBy(x => x.FullName), "UserId", "FullName");
            ViewBag.GameTypeId = new SelectList(_context.GameTypes.OrderBy(x => x.NameFa), "GameTypeId", "NameFa");
            return View(model);
        }

        // GET: /Games/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var model = await _context.Games.FindAsync(id);
            if (model == null) return NotFound();
            ViewBag.AuthorUserId = new SelectList(_context.AppUsers.OrderBy(x => x.FullName), "UserId", "FullName");
            ViewBag.GameTypeId = new SelectList(_context.GameTypes.OrderBy(x => x.NameFa), "GameTypeId", "NameFa");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("GameId,GameCode,NameFa,NameEn,Summary,TitleFa,TitleEn,DescriptionFa,DescriptionEn,AuthorUserId,GameTypeId,DurationMinutes,IsActive")] Game model)
        {
            if (id != model.GameId) return NotFound();
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
                    if (!_context.Games.Any(x => x.GameId == id)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewBag.AuthorUserId = new SelectList(_context.AppUsers.OrderBy(x => x.FullName), "UserId", "FullName");
            ViewBag.GameTypeId = new SelectList(_context.GameTypes.OrderBy(x => x.NameFa), "GameTypeId", "NameFa");
            return View(model);
        }

        // GET: /Games/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _context.Games.Include(x => x.AppUser).Include(x => x.GameType).FirstOrDefaultAsync(x => x.GameId == id);
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var model = await _context.Games.FindAsync(id);
            if (model != null)
            {
                _context.Games.Remove(model);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}