// ============================================================
// Program.cs - Application Entry Point
// Migrated from net6.0 to net10.0 with Clean Architecture
// Replaces: Global.asax, App_Start/RouteConfig.cs,
//           App_Start/FilterConfig.cs, App_Start/BundleConfig.cs
// ============================================================

using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OrbitAOS.V6.Application.Common.Interfaces;
using OrbitAOS.V6.Application.Common.Mappings;
using OrbitAOS.V6.Domain.Entities;
using OrbitAOS.V6.Infrastructure.Data;
using OrbitAOS.V6.Infrastructure.Identity;
using OrbitAOS.V6.Infrastructure.Services;
using OrbitAOS.V6.Web.Middleware;
using Serilog;
using System.Reflection;

// -------------------------------------------------------
// Configure Serilog early (replaces legacy log4net/ELMAH)
// -------------------------------------------------------
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/orbitaos-.log", rollingInterval: RollingInterval.Day)
    .CreateBootstrapLogger();

try
{
    Log.Information("Starting OrbitAOS.V6 application on .NET 10");

    var builder = WebApplication.CreateBuilder(args);

    // -------------------------------------------------------
    // Serilog integration (replaces ILogger<T> default provider)
    // -------------------------------------------------------
    builder.Host.UseSerilog((context, services, configuration) =>
        configuration
            .ReadFrom.Configuration(context.Configuration)
            .ReadFrom.Services(services)
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .WriteTo.File("logs/orbitaos-.log", rollingInterval: RollingInterval.Day));

    // -------------------------------------------------------
    // DATABASE - EF Core 10 (replaces EF6 / web.config connectionStrings)
    // -------------------------------------------------------
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

    builder.Services.AddDbContext<ApplicationDbContext>(options =>
    {
        options.UseSqlServer(connectionString, sqlOptions =>
        {
            sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(30),
                errorNumbersToAdd: null);
        });
        // Enable sensitive data logging only in Development
        if (builder.Environment.IsDevelopment())
        {
            options.EnableSensitiveDataLogging();
            options.EnableDetailedErrors();
        }
    });

    // Developer exception page for EF Core migrations
    builder.Services.AddDatabaseDeveloperPageExceptionFilter();

    // -------------------------------------------------------
    // IDENTITY - ASP.NET Core Identity on .NET 10
    // Replaces: Forms Authentication / ASP.NET Identity 2.x
    // -------------------------------------------------------
    builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
    {
        // Password policy (replaces web.config membership settings)
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequiredLength = 8;

        // Lockout settings
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
        options.Lockout.MaxFailedAccessAttempts = 5;
        options.Lockout.AllowedForNewUsers = true;

        // User settings
        options.User.RequireUniqueEmail = true;
        options.SignIn.RequireConfirmedAccount = true;
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

    // Cookie authentication (replaces Forms Authentication)
    builder.Services.ConfigureApplicationCookie(options =>
    {
        options.LoginPath = "/Identity/Account/Login";
        options.LogoutPath = "/Identity/Account/Logout";
        options.AccessDeniedPath = "/Identity/Account/AccessDenied";
        options.SlidingExpiration = true;
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.SameSite = SameSiteMode.Lax;
    });

    // -------------------------------------------------------
    // CLEAN ARCHITECTURE - Application Layer Services
    // -------------------------------------------------------
    // MediatR for CQRS (Commands/Queries)
    builder.Services.AddMediatR(cfg =>
        cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

    // AutoMapper profiles
    builder.Services.AddAutoMapper(typeof(MappingProfile));

    // FluentValidation (replaces DataAnnotations-only validation)
    builder.Services.AddFluentValidationAutoValidation();
    builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

    // -------------------------------------------------------
    // INFRASTRUCTURE - Repository and Service registrations
    // -------------------------------------------------------
    builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
    builder.Services.AddScoped<IDateTimeService, DateTimeService>();
    builder.Services.AddHttpContextAccessor(); // Replaces HttpContext.Current

    // -------------------------------------------------------
    // MVC - Controllers with Views (replaces System.Web.Mvc)
    // -------------------------------------------------------
    builder.Services.AddControllersWithViews(options =>
    {
        // Global exception filter (replaces App_Start/FilterConfig.cs HandleErrorAttribute)
        options.Filters.Add<GlobalExceptionFilter>();
    });

    // Razor Pages for Identity UI scaffolding
    builder.Services.AddRazorPages();

    // Response caching (replaces [OutputCache] attribute)
    builder.Services.AddResponseCaching();

    // Session state (replaces System.Web.SessionState)
    builder.Services.AddDistributedMemoryCache();
    builder.Services.AddSession(options =>
    {
        options.IdleTimeout = TimeSpan.FromMinutes(30);
        options.Cookie.HttpOnly = true;
        options.Cookie.IsEssential = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    });

    // -------------------------------------------------------
    // BUILD APPLICATION
    // -------------------------------------------------------
    var app = builder.Build();

    // -------------------------------------------------------
    // MIDDLEWARE PIPELINE
    // Replaces: HTTP Modules (IHttpModule) and HTTP Handlers
    // -------------------------------------------------------
    if (app.Environment.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
        app.UseMigrationsEndPoint();
    }
    else
    {
        // Replaces: CustomErrors in web.config
        app.UseExceptionHandler("/Home/Error");
        // HSTS (replaces manual HTTPS redirect in web.config)
        app.UseHsts();
    }

    // Custom request logging middleware (replaces HTTP Module)
    app.UseMiddleware<RequestLoggingMiddleware>();

    // HTTPS redirection (replaces IIS HTTPS redirect)
    app.UseHttpsRedirection();

    // Static files from wwwroot (replaces /Content, /Scripts folders)
    app.UseStaticFiles();

    // Response caching middleware
    app.UseResponseCaching();

    // Routing (replaces App_Start/RouteConfig.cs)
    app.UseRouting();

    // Session middleware
    app.UseSession();

    // Authentication & Authorization (replaces FormsAuthentication)
    app.UseAuthentication();
    app.UseAuthorization();

    // -------------------------------------------------------
    // ROUTING - Convention-based (replaces RouteConfig.cs)
    // -------------------------------------------------------
    app.MapControllerRoute(
        name: "areas",
        pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");

    // Razor Pages for Identity UI
    app.MapRazorPages();

    // -------------------------------------------------------
    // AUTO-MIGRATE DATABASE ON STARTUP (Development only)
    // -------------------------------------------------------
    if (app.Environment.IsDevelopment())
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await dbContext.Database.MigrateAsync();
    }

    await app.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    await Log.CloseAndFlushAsync();
}
