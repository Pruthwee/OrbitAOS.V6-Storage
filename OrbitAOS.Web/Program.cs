using Microsoft.AspNetCore.Identity;
using OrbitAOS.Application;
using OrbitAOS.Infrastructure;
using OrbitAOS.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

// ── Infrastructure layer (EF Core 8, Identity, repositories) ─────────────
builder.Services.AddInfrastructureServices(builder.Configuration);

// ── Application layer (business services) ────────────────────────────────
builder.Services.AddApplicationServices();

// ── ASP.NET Core Identity (migrated from ASP.NET Identity 2.x) ───────────
// Uses ASP.NET Core Identity on .NET 8 with EF Core 8 stores.
// Replaces legacy Forms Authentication / ASP.NET Identity 2.x from MVC 6.0.
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = true;
    // Password policy (equivalent to legacy PasswordValidator settings)
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>();

// ── MVC with Views ────────────────────────────────────────────────────────
// Replaces legacy System.Web.Mvc registration in Global.asax / FilterConfig.
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
// Replaces legacy HTTP Modules and Global.asax Application_BeginRequest etc.
if (app.Environment.IsDevelopment())
{
    // Replaces legacy customErrors in web.config
    app.UseMigrationsEndPoint();
}
else
{
    // Replaces legacy customErrors mode="On" in web.config
    app.UseExceptionHandler("/Home/Error");
    // HSTS replaces legacy requireSSL in web.config
    app.UseHsts();
}

// Replaces legacy requireSSL / httpsRedirect in web.config
app.UseHttpsRedirection();

// Serves files from wwwroot (replaces legacy /Content and /Scripts folders)
app.UseStaticFiles();

// Replaces legacy RouteConfig.RegisterRoutes in App_Start
app.UseRouting();

// Response caching middleware (replaces legacy [OutputCache] attribute)
app.UseResponseCaching();

// Session middleware (replaces legacy System.Web.SessionState)
app.UseSession();

// Authentication/Authorization middleware
// Replaces legacy FormsAuthentication and legacy [Authorize] attribute wiring
app.UseAuthentication();
app.UseAuthorization();

// Default MVC route (replaces App_Start/RouteConfig.cs MapRoute)
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Razor Pages for Identity UI scaffolded pages
app.MapRazorPages();

app.Run();
