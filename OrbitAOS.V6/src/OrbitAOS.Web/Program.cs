using Microsoft.AspNetCore.Identity;
using OrbitAOS.Application;
using OrbitAOS.Infrastructure;
using OrbitAOS.Infrastructure.Data;
using OrbitAOS.Web.Filters;
using OrbitAOS.Web.Middleware;

// ─── Application Builder ─────────────────────────────────────────────────────
var builder = WebApplication.CreateBuilder(args);

// ─── Configuration ────────────────────────────────────────────────────────────
// Null guard added for .NET 8 nullable reference type compliance (issue-6)
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

// ─── MVC Controllers with Views + Global Exception Filter ────────────────────
// GlobalExceptionFilter replaces legacy HandleErrorAttribute (issue-17)
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add<GlobalExceptionFilter>();
});

// ─── Cookie Authentication (replaces Forms Authentication from web.config) ───
// Explicitly configured for .NET 8 security best practices (issue-15)
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Account/Login";
    options.LogoutPath = "/Identity/Account/Logout";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
    options.SlidingExpiration = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
});

// ─── Session State (replaces System.Web.SessionState) ────────────────────────
// Required for HttpContext.Session usage (issue-9)
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// ─── Response Caching (replaces [OutputCache] attribute) ─────────────────────
// Required for [ResponseCache] attribute on HomeController.Error() (issue-14)
builder.Services.AddResponseCaching();

// ─── HTTP Context Accessor (replaces System.Web.HttpContext.Current) ─────────
// Required for services that inject IHttpContextAccessor (issue-10)
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

// Custom request logging middleware (replaces legacy IHttpModule) (issue-18)
app.UseRequestLogging();

// Routing middleware (replaces RouteConfig.cs in App_Start)
app.UseRouting();

// Session middleware (must be after UseRouting, before UseAuthentication)
app.UseSession();

// Response caching middleware (issue-14)
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
