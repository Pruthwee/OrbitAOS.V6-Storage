using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OrbitAOS.V6.Data;

// -----------------------------------------------------------------------
// OrbitAOS.V6 - Upgraded from ASP.NET MVC 6.0 to ASP.NET Core MVC on .NET 10
// This is the original single-project structure preserved for reference.
// The Clean Architecture version is in OrbitAOS.Web project.
// -----------------------------------------------------------------------

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

// EF Core 10: UseSqlServer replaces legacy Entity Framework 6 DbContext configuration
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// EF Core developer page exception filter (replaces legacy YSOD for EF errors)
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// ASP.NET Core Identity on .NET 10 (replaces legacy ASP.NET Identity 2.x / Forms Authentication)
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>();

// Add MVC with views (replaces legacy System.Web.Mvc registration in Global.asax)
builder.Services.AddControllersWithViews();

var app = builder.Build();

// -----------------------------------------------------------------------
// Middleware Pipeline
// Replaces legacy Global.asax Application_Start, HTTP Modules, web.config system.web
// -----------------------------------------------------------------------

if (app.Environment.IsDevelopment())
{
    // Development: show EF Core migration errors (replaces customErrors mode="Off")
    app.UseMigrationsEndPoint();
}
else
{
    // Production: custom error handler (replaces customErrors in web.config)
    app.UseExceptionHandler("/Home/Error");
    // HSTS (replaces requireSSL in web.config)
    app.UseHsts();
}

// HTTPS redirection (replaces requireSSL in web.config)
app.UseHttpsRedirection();

// Static files from wwwroot (replaces /Content, /Scripts served by IIS)
app.UseStaticFiles();

// Routing middleware (replaces RouteTable.Routes in App_Start/RouteConfig.cs)
app.UseRouting();

// Authentication middleware (replaces FormsAuthentication HTTP module)
app.UseAuthentication();

// Authorization middleware
app.UseAuthorization();

// -----------------------------------------------------------------------
// Route Configuration
// Replaces App_Start/RouteConfig.cs routes.MapRoute()
// -----------------------------------------------------------------------
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Razor Pages for Identity UI (replaces legacy Account controller scaffolding)
app.MapRazorPages();

app.Run();
