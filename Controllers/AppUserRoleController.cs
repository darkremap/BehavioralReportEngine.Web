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
    public class AppUserRoleController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AppUserRoleController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /AppUserRoles
        public async Task<IActionResult> Index()
        {
            var data = _context.AppUserRoles.Include(x => x.AppUser).Include(x => x.UserRole);
            return View(await data.ToListAsync());
        }

        // GET: /AppUserRoles/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var item = await _context.AppUserRoles.Include(x => x.AppUser).Include(x => x.UserRole).FirstOrDefaultAsync(x => x.AppUserRoleId == id);
            if (item == null) return NotFound();
            return View(item);
        }

        // GET: /AppUserRoles/Create
        public IActionResult Create()
        {
            ViewBag.UserId = new SelectList(_context.AppUsers.OrderBy(x => x.FullName), "UserId", "FullName");
            ViewBag.RoleId = new SelectList(_context.UserRoles.OrderBy(x => x.RoleName), "RoleId", "RoleName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AppUserRoleId,UserId,RoleId")] AppUserRole model)
        {
            if (ModelState.IsValid)
            {
                model.CreatedAt = DateTime.UtcNow;
                _context.Add(model);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.UserId = new SelectList(_context.AppUsers.OrderBy(x => x.FullName), "UserId", "FullName");
            ViewBag.RoleId = new SelectList(_context.UserRoles.OrderBy(x => x.RoleName), "RoleId", "RoleName");
            return View(model);
        }

        // GET: /AppUserRoles/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var model = await _context.AppUserRoles.FindAsync(id);
            if (model == null) return NotFound();
            ViewBag.UserId = new SelectList(_context.AppUsers.OrderBy(x => x.FullName), "UserId", "FullName");
            ViewBag.RoleId = new SelectList(_context.UserRoles.OrderBy(x => x.RoleName), "RoleId", "RoleName");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AppUserRoleId,UserId,RoleId")] AppUserRole model)
        {
            if (id != model.AppUserRoleId) return NotFound();
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(model);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.AppUserRoles.Any(x => x.AppUserRoleId == id)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewBag.UserId = new SelectList(_context.AppUsers.OrderBy(x => x.FullName), "UserId", "FullName");
            ViewBag.RoleId = new SelectList(_context.UserRoles.OrderBy(x => x.RoleName), "RoleId", "RoleName");
            return View(model);
        }

        // GET: /AppUserRoles/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _context.AppUserRoles.Include(x => x.AppUser).Include(x => x.UserRole).FirstOrDefaultAsync(x => x.AppUserRoleId == id);
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var model = await _context.AppUserRoles.FindAsync(id);
            if (model != null)
            {
                _context.AppUserRoles.Remove(model);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}