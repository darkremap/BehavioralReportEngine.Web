using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BehavioralReportEngine.Web.Data;
using BehavioralReportEngine.Web.Models;

namespace BehavioralReportEngine.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var counts = new Dictionary<string, int>
            {
                ["Organizations"] = await _context.Organizations.CountAsync(),
                ["Participants"] = await _context.Participants.CountAsync(),
                ["Games"] = await _context.Games.CountAsync(),
                ["Game Sessions"] = await _context.GameSessions.CountAsync(),
                ["Reports"] = await _context.Reports.CountAsync(),
                ["Key Moments"] = await _context.KeyMoments.CountAsync(),
            };

            ViewBag.Counts = counts;
            return View();
        }

        public IActionResult Error(string message)
        {
            ViewBag.Message = message;
            return View();
        }
    }
}
