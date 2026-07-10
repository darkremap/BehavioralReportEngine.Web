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
    public class ParticipantController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ParticipantController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Participants
        public async Task<IActionResult> Index()
        {
            var data = _context.Participants.Include(x => x.Organization).Include(x => x.Department);
            return View(await data.ToListAsync());
        }

        // GET: /Participants/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var item = await _context.Participants.Include(x => x.Organization).Include(x => x.Department).FirstOrDefaultAsync(x => x.ParticipantId == id);
            if (item == null) return NotFound();
            return View(item);
        }

        // GET: /Participants/Create
        public IActionResult Create()
        {
            ViewBag.OrganizationId = new SelectList(_context.Organizations.OrderBy(x => x.Name), "OrganizationId", "Name");
            ViewBag.DepartmentId = new SelectList(_context.Departments.OrderBy(x => x.NameFa), "DepartmentId", "NameFa");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ParticipantId,OrganizationId,PersonnelCode,FirstNameFa,LastNameFa,FirstNameEn,LastNameEn,Email,DepartmentId,BirthDate,Gender,JobTitle,MetadataJson,IsActive")] Participant model)
        {
            if (ModelState.IsValid)
            {
                model.CreatedAt = DateTime.UtcNow;
                _context.Add(model);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.OrganizationId = new SelectList(_context.Organizations.OrderBy(x => x.Name), "OrganizationId", "Name");
            ViewBag.DepartmentId = new SelectList(_context.Departments.OrderBy(x => x.NameFa), "DepartmentId", "NameFa");
            return View(model);
        }

        // GET: /Participants/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var model = await _context.Participants.FindAsync(id);
            if (model == null) return NotFound();
            ViewBag.OrganizationId = new SelectList(_context.Organizations.OrderBy(x => x.Name), "OrganizationId", "Name");
            ViewBag.DepartmentId = new SelectList(_context.Departments.OrderBy(x => x.NameFa), "DepartmentId", "NameFa");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ParticipantId,OrganizationId,PersonnelCode,FirstNameFa,LastNameFa,FirstNameEn,LastNameEn,Email,DepartmentId,BirthDate,Gender,JobTitle,MetadataJson,IsActive")] Participant model)
        {
            if (id != model.ParticipantId) return NotFound();
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
                    if (!_context.Participants.Any(x => x.ParticipantId == id)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewBag.OrganizationId = new SelectList(_context.Organizations.OrderBy(x => x.Name), "OrganizationId", "Name");
            ViewBag.DepartmentId = new SelectList(_context.Departments.OrderBy(x => x.NameFa), "DepartmentId", "NameFa");
            return View(model);
        }

        // GET: /Participants/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _context.Participants.Include(x => x.Organization).Include(x => x.Department).FirstOrDefaultAsync(x => x.ParticipantId == id);
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var model = await _context.Participants.FindAsync(id);
            if (model != null)
            {
                _context.Participants.Remove(model);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}