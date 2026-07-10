using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;
using BehavioralReportEngine.Web.Data;
using BehavioralReportEngine.Web.Components;
using BehavioralReportEngine.Web.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddMudServices();

// AddDbContextFactory (not AddDbContext): in Blazor Server the DI-scoped instance would live
// for the whole circuit (until the tab closes), causing stale-tracking bugs. Components create
// a short-lived context per operation via IDbContextFactory<ApplicationDbContext>.
builder.Services.AddDbContextFactory<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddSingleton<EntityMetadataService>();
builder.Services.AddScoped<LogoUploadService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
