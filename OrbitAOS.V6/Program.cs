using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OrbitAOS.V6.Data;

// ─── Application Builder ─────────────────────────────────────────────────────
// .NET 8 minimal hosting model (replaces Global.asax + App_Start configuration)
var builder = WebApplication.CreateBuilder(args);

// ─── Database Configuration ───────────────────────────────────────────────────
// Connection string from appsettings.json (replaces web.config connectionStrings)
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
    options.Password.RequiredLength = 6;
})
.AddEntityFrameworkStores<ApplicationDbContext>();

// ─── MVC Controllers with Views ──────────────────────────────────────────────
// Replaces App_Start/FilterConfig.cs and RouteConfig.cs
builder.Services.AddControllersWithViews();

// ─── Session State ────────────────────────────────────────────────────────────
// Replaces System.Web.SessionState
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// ─── HTTP Context Accessor ────────────────────────────────────────────────────
// Replaces System.Web.HttpContext.Current
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

// Routing (replaces App_Start/RouteConfig.cs)
app.UseRouting();

// Session middleware
app.UseSession();

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
