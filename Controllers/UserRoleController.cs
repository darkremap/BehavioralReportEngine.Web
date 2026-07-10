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
    public class UserRoleController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UserRoleController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /UserRoles
        public async Task<IActionResult> Index()
        {
            var data = _context.UserRoles;
            return View(await data.ToListAsync());
        }

        // GET: /UserRoles/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var item = await _context.UserRoles.FirstOrDefaultAsync(x => x.RoleId == id);
            if (item == null) return NotFound();
            return View(item);
        }

        // GET: /UserRoles/Create
        public IActionResult Create()
        {

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RoleId,RoleCode,RoleName,Description")] UserRole model)
        {
            if (ModelState.IsValid)
            {
                _context.Add(model);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        // GET: /UserRoles/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var model = await _context.UserRoles.FindAsync(id);
            if (model == null) return NotFound();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("RoleId,RoleCode,RoleName,Description")] UserRole model)
        {
            if (id != model.RoleId) return NotFound();
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(model);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.UserRoles.Any(x => x.RoleId == id)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        // GET: /UserRoles/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _context.UserRoles.FirstOrDefaultAsync(x => x.RoleId == id);
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var model = await _context.UserRoles.FindAsync(id);
            if (model != null)
            {
                _context.UserRoles.Remove(model);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}