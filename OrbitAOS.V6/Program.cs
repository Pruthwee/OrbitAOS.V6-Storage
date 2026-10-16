using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OrbitAOS.V6.Data;

// ── OrbitAOS.V6 - Upgraded from net6.0 to net8.0 ─────────────────────────
// This is the original single-project MVC application upgraded in-place.
// For the full Clean Architecture version, see OrbitAOS.Web project.
//
// Key changes from net6.0 to net8.0:
//   1. TargetFramework: net6.0 → net8.0
//   2. All NuGet packages: 6.0.x → 8.0.x
//   3. EF Core migrations: Updated from EF Core 3.0 syntax to EF Core 8.0
//      - Removed deprecated 'nullable: false' parameter
//      - Replaced HasName() with HasDatabaseName() for index naming
//      - Replaced SqlServerValueGenerationStrategy.IdentityColumn annotation
//   4. Program.cs: Minimal hosting model preserved (already compatible)
//   5. Added ResponseCaching and Session middleware (best practices for .NET 8)

var builder = WebApplication.CreateBuilder(args);

// ── Data layer (EF Core 8.0 - upgraded from EF Core 6.0) ─────────────────
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// ── ASP.NET Core Identity (upgraded from Identity 6.0 to Identity 8.0) ───
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = true;
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>();

// ── MVC ───────────────────────────────────────────────────────────────────
builder.Services.AddControllersWithViews();

// ── Response Caching (replaces legacy [OutputCache] attribute) ────────────
builder.Services.AddResponseCaching();

// ── Session (replaces legacy System.Web.SessionState) ────────────────────
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// ── HTTP request pipeline ─────────────────────────────────────────────────
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseResponseCaching();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
