using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OrbitAOS.V6.Data;
using OrbitAOS.V6.Filters;
using OrbitAOS.V6.Middleware;

// ─── Application Builder ─────────────────────────────────────────────────────
// .NET 8 minimal hosting model (replaces Global.asax + App_Start configuration)
var builder = WebApplication.CreateBuilder(args);

// ─── Database Configuration ───────────────────────────────────────────────────
// Connection string from appsettings.json (replaces web.config connectionStrings)
// Null guard added for .NET 8 nullable reference type compliance (issue-6)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

// EF Core 8 with SQL Server (replaces Entity Framework 6)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// EF Core developer page exception filter (development only)
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// ─── ASP.NET Core Identity (.NET 8) ──────────────────────────────────────────
// Replaces ASP.NET Identity 2.x / Forms Authentication from web.config
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = true;
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>();

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

// ─── MVC Controllers with Views + Global Exception Filter ────────────────────
// Replaces App_Start/FilterConfig.cs and RouteConfig.cs
// GlobalExceptionFilter replaces legacy HandleErrorAttribute (issue-17)
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add<GlobalExceptionFilter>();
});

// ─── Response Caching (replaces [OutputCache] attribute) ─────────────────────
// Required for [ResponseCache] attribute on HomeController.Error() (issue-14)
builder.Services.AddResponseCaching();

// ─── Session State ────────────────────────────────────────────────────────────
// Replaces System.Web.SessionState (issue-9)
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// ─── HTTP Context Accessor ────────────────────────────────────────────────────
// Replaces System.Web.HttpContext.Current (issue-10)
builder.Services.AddHttpContextAccessor();

// ─── Build Application ────────────────────────────────────────────────────────
var app = builder.Build();

// ─── HTTP Request Pipeline ────────────────────────────────────────────────────
// Replaces Global.asax Application_Start and HTTP modules
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    // Replaces customErrors in web.config
    app.UseExceptionHandler("/Home/Error");
    // HSTS header (not available in legacy MVC)
    app.UseHsts();
}

// HTTPS redirection (replaces requireSSL in web.config)
app.UseHttpsRedirection();

// Static files from wwwroot (replaces /Content and /Scripts)
app.UseStaticFiles();

// Custom request logging middleware (replaces legacy IHttpModule) (issue-18)
app.UseRequestLogging();

// Routing (replaces App_Start/RouteConfig.cs)
app.UseRouting();

// Session middleware (must be after UseRouting, before UseAuthentication)
app.UseSession();

// Response caching middleware (issue-14)
app.UseResponseCaching();

// Authentication (replaces FormsAuthentication HTTP module)
app.UseAuthentication();

// Authorization
app.UseAuthorization();

// ─── Route Configuration ──────────────────────────────────────────────────────
// Replaces App_Start/RouteConfig.cs RegisterRoutes method
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Razor Pages for Identity UI
app.MapRazorPages();

app.Run();
