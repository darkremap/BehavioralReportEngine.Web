using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BehavioralReportEngine.Web.Data;
using BehavioralReportEngine.Web.Models;

namespace BehavioralReportEngine.Web.Controllers
{
    public class OrganizationController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public OrganizationController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        private async Task<string> SaveLogoAsync(IFormFile logoFile)
        {
            var logosFolder = Path.Combine(_env.WebRootPath, "uploads", "logos");
            Directory.CreateDirectory(logosFolder);
            var fileName = Guid.NewGuid().ToString("N") + Path.GetExtension(logoFile.FileName);
            var filePath = Path.Combine(logosFolder, fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await logoFile.CopyToAsync(stream);
            }
            return "/uploads/logos/" + fileName;
        }

        // GET: /Organizations
        public async Task<IActionResult> Index()
        {
            var data = _context.Organizations;
            return View(await data.ToListAsync());
        }

        // GET: /Organizations/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var item = await _context.Organizations.FirstOrDefaultAsync(x => x.OrganizationId == id);
            if (item == null) return NotFound();
            return View(item);
        }

        // GET: /Organizations/Create
        public IActionResult Create()
        {

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("OrganizationId,OrganizationCode,Name,Industry,Country,IsActive")] Organization model, IFormFile logoFile)
        {
            if (ModelState.IsValid)
            {
                if (logoFile != null && logoFile.Length > 0)
                {
                    model.LogoPath = await SaveLogoAsync(logoFile);
                }
                model.CreatedAt = DateTime.UtcNow;
                _context.Add(model);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        // GET: /Organizations/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var model = await _context.Organizations.FindAsync(id);
            if (model == null) return NotFound();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("OrganizationId,OrganizationCode,Name,Industry,Country,IsActive")] Organization model, IFormFile logoFile)
        {
            if (id != model.OrganizationId) return NotFound();
            if (ModelState.IsValid)
            {
                try
                {
                    if (logoFile != null && logoFile.Length > 0)
                    {
                        model.LogoPath = await SaveLogoAsync(logoFile);
                    }
                    else
                    {
                        var existingLogoPath = await _context.Organizations
                            .Where(x => x.OrganizationId == id)
                            .Select(x => x.LogoPath)
                            .FirstOrDefaultAsync();
                        model.LogoPath = existingLogoPath;
                    }
                    model.UpdatedAt = DateTime.UtcNow;
                    _context.Update(model);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Organizations.Any(x => x.OrganizationId == id)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        // GET: /Organizations/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _context.Organizations.FirstOrDefaultAsync(x => x.OrganizationId == id);
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var model = await _context.Organizations.FindAsync(id);
            if (model != null)
            {
                _context.Organizations.Remove(model);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}