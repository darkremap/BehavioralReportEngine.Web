using Microsoft.EntityFrameworkCore;
using BehavioralReportEngine.Web.Data;
using BehavioralReportEngine.Web.Helpers;

var builder = WebApplication.CreateBuilder(args);

// MVC with views
builder.Services.AddControllersWithViews();

// EF Core - SQL Server, pointing at the database created by the schema script
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// Data-validation errors (CHECK/UNIQUE/FK constraint violations) are expected user input
// mistakes, not bugs - show a friendly message instead of the raw SQL exception. Placed after
// UseRouting/UseAuthorization so it wraps MVC action execution directly, catching the exception
// before it reaches the dev exception page / 500 handler registered above.
app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (DbUpdateException ex)
    {
        var message = DbExceptionHelper.GetFriendlyMessage(ex);
        context.Response.Redirect($"/Home/Error?message={Uri.EscapeDataString(message)}");
    }
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
