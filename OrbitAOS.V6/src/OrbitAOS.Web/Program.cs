using Microsoft.AspNetCore.Identity;
using OrbitAOS.Application;
using OrbitAOS.Infrastructure;
using OrbitAOS.Infrastructure.Data;

// ─── Application Builder ─────────────────────────────────────────────────────
var builder = WebApplication.CreateBuilder(args);

// ─── Configuration ────────────────────────────────────────────────────────────
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

// ─── Infrastructure Layer (EF Core, Repositories) ────────────────────────────
builder.Services.AddInfrastructure(connectionString);

// ─── Application Layer (Services) ────────────────────────────────────────────
builder.Services.AddApplication();

// ─── ASP.NET Core Identity (.NET 8) ──────────────────────────────────────────
// Migrated from ASP.NET Identity 2.x (MVC 5/6) to ASP.NET Core Identity on .NET 8
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    // Password policy - configure to match legacy application requirements
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;

    // Sign-in policy
    options.SignIn.RequireConfirmedAccount = true;

    // Lockout policy
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>();

// ─── MVC Controllers with Views ──────────────────────────────────────────────
builder.Services.AddControllersWithViews();

// ─── Cookie Authentication (replaces Forms Authentication from web.config) ───
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Account/Login";
    options.LogoutPath = "/Identity/Account/Logout";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
    options.SlidingExpiration = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
});

// ─── Session State (replaces System.Web.SessionState) ────────────────────────
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// ─── Response Caching (replaces [OutputCache] attribute) ─────────────────────
builder.Services.AddResponseCaching();

// ─── HTTP Context Accessor (replaces System.Web.HttpContext.Current) ─────────
builder.Services.AddHttpContextAccessor();

// ─── Build Application ────────────────────────────────────────────────────────
var app = builder.Build();

// ─── HTTP Request Pipeline ────────────────────────────────────────────────────
if (app.Environment.IsDevelopment())
{
    // EF Core migration endpoint for development (replaces YSOD)
    app.UseMigrationsEndPoint();
}
else
{
    // Global error handler (replaces customErrors in web.config)
    app.UseExceptionHandler("/Home/Error");
    // HSTS (HTTP Strict Transport Security) - not available in legacy MVC
    app.UseHsts();
}

// HTTPS redirection (replaces requireSSL in web.config)
app.UseHttpsRedirection();

// Static files from wwwroot (replaces /Content and /Scripts folders)
app.UseStaticFiles();

// Routing middleware (replaces RouteConfig.cs in App_Start)
app.UseRouting();

// Session middleware (must be after UseRouting, before UseAuthentication)
app.UseSession();

// Response caching middleware
app.UseResponseCaching();

// Authentication middleware (replaces FormsAuthentication module)
app.UseAuthentication();

// Authorization middleware
app.UseAuthorization();

// ─── Route Configuration ──────────────────────────────────────────────────────
// Replaces App_Start/RouteConfig.cs RegisterRoutes method
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Razor Pages for Identity UI (account management pages)
app.MapRazorPages();

app.Run();
