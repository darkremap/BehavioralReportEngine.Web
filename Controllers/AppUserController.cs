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
    public class AppUserController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AppUserController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /AppUsers
        public async Task<IActionResult> Index()
        {
            var data = _context.AppUsers.Include(x => x.Organization);
            return View(await data.ToListAsync());
        }

        // GET: /AppUsers/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var item = await _context.AppUsers.Include(x => x.Organization).FirstOrDefaultAsync(x => x.UserId == id);
            if (item == null) return NotFound();
            return View(item);
        }

        // GET: /AppUsers/Create
        public IActionResult Create()
        {
            ViewBag.OrganizationId = new SelectList(_context.Organizations.OrderBy(x => x.Name), "OrganizationId", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserId,OrganizationId,FullName,Email,IsActive")] AppUser model)
        {
            if (ModelState.IsValid)
            {
                model.CreatedAt = DateTime.UtcNow;
                _context.Add(model);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.OrganizationId = new SelectList(_context.Organizations.OrderBy(x => x.Name), "OrganizationId", "Name");
            return View(model);
        }

        // GET: /AppUsers/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var model = await _context.AppUsers.FindAsync(id);
            if (model == null) return NotFound();
            ViewBag.OrganizationId = new SelectList(_context.Organizations.OrderBy(x => x.Name), "OrganizationId", "Name");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UserId,OrganizationId,FullName,Email,IsActive")] AppUser model)
        {
            if (id != model.UserId) return NotFound();
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
                    if (!_context.AppUsers.Any(x => x.UserId == id)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewBag.OrganizationId = new SelectList(_context.Organizations.OrderBy(x => x.Name), "OrganizationId", "Name");
            return View(model);
        }

        // GET: /AppUsers/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _context.AppUsers.Include(x => x.Organization).FirstOrDefaultAsync(x => x.UserId == id);
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var model = await _context.AppUsers.FindAsync(id);
            if (model != null)
            {
                _context.AppUsers.Remove(model);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}