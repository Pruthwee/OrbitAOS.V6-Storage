using OrbitAOS.V8.Application;
using OrbitAOS.V8.Infrastructure;

// ============================================================================
// Program.cs — ASP.NET Core MVC .NET 8 Entry Point
// ============================================================================
// Migration notes from legacy ASP.NET MVC 6.0:
// - Replaces Global.asax + Global.asax.cs (Application_Start, Application_End, etc.)
// - Replaces App_Start/RouteConfig.cs (routing now configured here)
// - Replaces App_Start/FilterConfig.cs (global filters registered here)
// - Replaces App_Start/BundleConfig.cs (use LibMan or WebOptimizer instead)
// - Replaces web.config system.web section (configuration via appsettings.json)
// - No more HttpApplication lifecycle events — use middleware pipeline
// - No more DependencyResolver.SetResolver() — built-in DI container
// - No more FilterProviders.Providers — use AddMvc().AddFilters()
// ============================================================================

var builder = WebApplication.CreateBuilder(args);

// --------------------------------------------------------------------------
// Service Registration (replaces Global.asax Application_Start + DI setup)
// --------------------------------------------------------------------------

// Infrastructure services: EF Core, Identity, Repositories
// Replaces: Database.SetInitializer(), AreaRegistration.RegisterAllAreas()
builder.Services.AddInfrastructureServices(builder.Configuration);

// Application services: Business logic services
// Replaces: DependencyResolver.SetResolver() with custom IoC container
builder.Services.AddApplicationServices();

// MVC with Views — replaces System.Web.Mvc registration
// Migration note: AddControllersWithViews() replaces the implicit MVC pipeline
// in legacy MVC 6.0. No more FilterConfig.RegisterGlobalFilters().
builder.Services.AddControllersWithViews(options =>
{
    // Global filters — replaces App_Start/FilterConfig.cs
    // Example: options.Filters.Add<CustomAuthorizationFilter>();
});

// Razor Pages for Identity UI (Account management pages)
// Migration note: Replaces legacy AccountController scaffolded views
builder.Services.AddRazorPages();

// Session support — replaces System.Web.SessionState
// Migration note: In legacy MVC 6.0, session was available via HttpContext.Session
// without explicit configuration. In .NET 8, it must be explicitly registered.
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SecurePolicy = Microsoft.AspNetCore.Http.CookieSecurePolicy.Always;
});

// IHttpContextAccessor — replaces HttpContext.Current (System.Web)
// Migration note: HttpContext.Current is NOT available in .NET 8.
// Inject IHttpContextAccessor where HttpContext is needed outside controllers.
builder.Services.AddHttpContextAccessor();

// Response caching — replaces [OutputCache] attribute
// Migration note: [OutputCache] from System.Web.Mvc is not available in .NET 8.
// Use [ResponseCache] attribute + AddResponseCaching() middleware.
builder.Services.AddResponseCaching();

// --------------------------------------------------------------------------
// Build the application
// --------------------------------------------------------------------------
var app = builder.Build();

// --------------------------------------------------------------------------
// Middleware Pipeline (replaces HTTP Modules and HTTP Handlers in web.config)
// --------------------------------------------------------------------------
// Migration note: The order of middleware matters. This replaces the implicit
// pipeline ordering in legacy MVC 6.0 (HttpModules in web.config).

if (app.Environment.IsDevelopment())
{
    // Replaces customErrors mode="Off" in web.config
    app.UseDeveloperExceptionPage();
    app.UseMigrationsEndPoint();
}
else
{
    // Replaces customErrors mode="On" defaultRedirect in web.config
    app.UseExceptionHandler("/Home/Error");

    // HSTS — not available in legacy MVC 6.0 (IIS-only HTTPS redirect)
    // Migration note: Replaces IIS HTTPS redirect rules in web.config
    app.UseHsts();
}

// HTTPS Redirection — replaces IIS URL Rewrite rules for HTTP→HTTPS
app.UseHttpsRedirection();

// Static files — replaces IIS static file serving for /Content, /Scripts
// Migration note: Only files in wwwroot/ are served as static content.
// Legacy /Content → wwwroot/css, /Scripts → wwwroot/js
app.UseStaticFiles();

// Routing — replaces RouteTable.Routes.MapRoute() in App_Start/RouteConfig.cs
app.UseRouting();

// Response caching middleware — replaces [OutputCache] attribute behavior
app.UseResponseCaching();

// Session middleware — must be after UseRouting, before UseAuthorization
// Migration note: Replaces System.Web.SessionState module in web.config
app.UseSession();

// Authentication — replaces FormsAuthentication module in web.config
// Migration note: Replaces <authentication mode="Forms"> in web.config
app.UseAuthentication();

// Authorization — replaces [Authorize] attribute behavior in legacy MVC
app.UseAuthorization();

// --------------------------------------------------------------------------
// Route Configuration (replaces App_Start/RouteConfig.cs)
// --------------------------------------------------------------------------
// Migration note: MapControllerRoute() replaces RouteTable.Routes.MapRoute()
// The default route pattern is identical to legacy MVC 6.0 convention.

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Razor Pages for Identity UI (Account/Register, Account/Login, etc.)
app.MapRazorPages();

app.Run();
