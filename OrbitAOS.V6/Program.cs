using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OrbitAOS.V6.Data;
using OrbitAOS.V6.Domain.Interfaces;
using OrbitAOS.V6.Infrastructure.Repositories;
using OrbitAOS.V6.Application.Services;
using OrbitAOS.V6.Application.Interfaces;
using OrbitAOS.V6.Filters;
using OrbitAOS.V6.Middleware;
using OrbitAOS.V6.Models;

// ============================================================
// OrbitAOS.V6 - ASP.NET Core MVC on .NET 8
// Migrated from ASP.NET MVC 6.0 (net6.0) to .NET 8 (net8.0)
// Architecture: MVC single-project with layered Services/Repositories
// ============================================================

var builder = WebApplication.CreateBuilder(args);

// ----------------------------------------------------------
// 1. STRONGLY-TYPED CONFIGURATION (IOptions<T> pattern)
//    Replaces direct builder.Configuration access throughout the app.
// ----------------------------------------------------------
builder.Services.Configure<AppSettings>(
    builder.Configuration.GetSection("AppSettings"));

// ----------------------------------------------------------
// 2. DATA LAYER (Infrastructure)
// ----------------------------------------------------------
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(connectionString);
    // Enable detailed EF Core diagnostics in development
    if (builder.Environment.IsDevelopment())
    {
        options.EnableSensitiveDataLogging();
        options.EnableDetailedErrors();
    }
});

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// ----------------------------------------------------------
// 3. IDENTITY (Infrastructure)
//    Migrated from ASP.NET Identity 2.x to ASP.NET Core Identity
// ----------------------------------------------------------
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = true;
    // Explicit password policy (matches legacy application requirements)
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<ApplicationDbContext>();

// ----------------------------------------------------------
// 4. APPLICATION SERVICES (Application Layer)
// ----------------------------------------------------------
builder.Services.AddScoped<IHomeService, HomeService>();

// ----------------------------------------------------------
// 5. INFRASTRUCTURE REPOSITORIES (Infrastructure Layer)
//    Register concrete repositories as the application grows:
//    builder.Services.AddScoped(typeof(IRepository<,>), typeof(Repository<,>));
//    builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
// ----------------------------------------------------------

// ----------------------------------------------------------
// 6. MVC + VIEWS + GLOBAL FILTERS
//    Replaces App_Start/FilterConfig.cs RegisterGlobalFilters
// ----------------------------------------------------------
builder.Services.AddControllersWithViews(options =>
{
    // Global action logging filter (replaces legacy FilterConfig.cs)
    options.Filters.Add<ActionLoggingFilter>();
});
builder.Services.AddRazorPages();

// ----------------------------------------------------------
// 7. RESPONSE CACHING (replaces legacy [OutputCache] attribute)
// ----------------------------------------------------------
builder.Services.AddResponseCaching();

// ----------------------------------------------------------
// 8. SESSION (replaces legacy System.Web.SessionState)
// ----------------------------------------------------------
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// ----------------------------------------------------------
// 9. HTTP CONTEXT ACCESSOR (replaces HttpContext.Current)
// ----------------------------------------------------------
builder.Services.AddHttpContextAccessor();

// ----------------------------------------------------------
// 10. HEALTH CHECKS (for container orchestration / load balancers)
// ----------------------------------------------------------
builder.Services.AddHealthChecks()
    .AddDbContextCheck<ApplicationDbContext>();

// ----------------------------------------------------------
// 11. CORS POLICY
// ----------------------------------------------------------
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin", policy =>
        policy.WithOrigins("https://example.com")
              .AllowAnyMethod()
              .AllowAnyHeader());
});

// ----------------------------------------------------------
// 12. ANTI-FORGERY CONFIGURATION (explicit CSRF settings)
// ----------------------------------------------------------
builder.Services.AddAntiforgery(options =>
{
    options.Cookie.Name = "X-CSRF-TOKEN";
    options.HeaderName = "X-CSRF-TOKEN";
});

// ============================================================
// BUILD THE APPLICATION
// ============================================================
var app = builder.Build();

// ----------------------------------------------------------
// MIDDLEWARE PIPELINE
// Order matters: Exception handling must be first.
// ----------------------------------------------------------

// Global exception middleware (replaces legacy Application_Error in Global.asax)
app.UseGlobalExceptionMiddleware();

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // HSTS: 30 days. Increase for production (max-age=31536000).
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// CORS must be between UseRouting and UseAuthorization
app.UseCors("AllowSpecificOrigin");

app.UseResponseCaching();

app.UseAuthentication();
app.UseAuthorization();

app.UseSession();

// ----------------------------------------------------------
// HEALTH CHECK ENDPOINT
// ----------------------------------------------------------
app.MapHealthChecks("/health");

// ----------------------------------------------------------
// ROUTING (replaces App_Start/RouteConfig.cs)
// Area route must be registered before the default route.
// ----------------------------------------------------------
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();
