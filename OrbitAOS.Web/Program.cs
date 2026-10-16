using OrbitAOS.Application;
using OrbitAOS.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// -----------------------------------------------------------------------
// Service Registration
// -----------------------------------------------------------------------

// Add Application layer services (business logic, DTOs, service interfaces)
builder.Services.AddApplicationServices();

// Add Infrastructure layer services (EF Core, Identity, repositories)
builder.Services.AddInfrastructureServices(builder.Configuration);

// Add ASP.NET Core MVC with views
builder.Services.AddControllersWithViews();

// Add Razor Pages (required for ASP.NET Core Identity UI scaffolded pages)
builder.Services.AddRazorPages();

// Configure cookie authentication settings (replaces legacy Forms Authentication)
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.SlidingExpiration = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = Microsoft.AspNetCore.Http.CookieSecurePolicy.Always;
    options.Cookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Lax;
});

// Configure session state (replaces legacy ASP.NET Session)
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// -----------------------------------------------------------------------
// Build the application
// -----------------------------------------------------------------------
var app = builder.Build();

// -----------------------------------------------------------------------
// Middleware Pipeline Configuration
// Replaces legacy Global.asax Application_Start, HTTP Modules, and web.config system.web
// -----------------------------------------------------------------------

if (app.Environment.IsDevelopment())
{
    // Development: show detailed EF Core migration errors
    app.UseMigrationsEndPoint();
    app.UseDeveloperExceptionPage();
}
else
{
    // Production: use custom error handler (replaces customErrors in web.config)
    app.UseExceptionHandler("/Home/Error");
    // HSTS: replaces requireSSL in web.config
    app.UseHsts();
}

// Redirect HTTP to HTTPS (replaces requireSSL in web.config)
app.UseHttpsRedirection();

// Serve static files from wwwroot (replaces /Content, /Scripts, /Images)
app.UseStaticFiles();

// Enable routing middleware
app.UseRouting();

// Enable session middleware (must be after UseRouting, before UseAuthentication)
app.UseSession();

// Enable authentication middleware (replaces FormsAuthentication module)
app.UseAuthentication();

// Enable authorization middleware
app.UseAuthorization();

// -----------------------------------------------------------------------
// Route Configuration
// Replaces legacy App_Start/RouteConfig.cs
// -----------------------------------------------------------------------

// Map MVC controller routes (replaces routes.MapRoute in RouteConfig.cs)
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Map Razor Pages (required for Identity UI)
app.MapRazorPages();

app.Run();
