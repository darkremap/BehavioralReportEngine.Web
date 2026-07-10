using Microsoft.AspNetCore.Components.Forms;

namespace BehavioralReportEngine.Web.Services
{
    // Port of the upload logic that used to live in OrganizationController.SaveLogoAsync.
    public class LogoUploadService
    {
        private const long MaxSizeBytes = 5 * 1024 * 1024;
        private readonly IWebHostEnvironment _env;

        public LogoUploadService(IWebHostEnvironment env)
        {
            _env = env;
        }

        public async Task<string> SaveAsync(IBrowserFile file)
        {
            var logosFolder = Path.Combine(_env.WebRootPath, "uploads", "logos");
            Directory.CreateDirectory(logosFolder);
            var fileName = Guid.NewGuid().ToString("N") + Path.GetExtension(file.Name);
            var filePath = Path.Combine(logosFolder, fileName);

            await using var stream = new FileStream(filePath, FileMode.Create);
            await file.OpenReadStream(MaxSizeBytes).CopyToAsync(stream);

            return "/uploads/logos/" + fileName;
        }
    }
}
