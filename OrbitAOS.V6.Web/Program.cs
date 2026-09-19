using Microsoft.AspNetCore.Identity;
using OrbitAOS.V6.Application;
using OrbitAOS.V6.Infrastructure;
using OrbitAOS.V6.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

// -------------------------------------------------------------------------
// Register services from Clean Architecture layers
// -------------------------------------------------------------------------

// Infrastructure layer: EF Core, repositories (connection string configured here)
builder.Services.AddInfrastructureServices(builder.Configuration);

// Application layer: business services
builder.Services.AddApplicationServices();

// ASP.NET Core Identity with UI (AddDefaultIdentity requires Microsoft.AspNetCore.Identity.UI)
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = true;
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 8;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<ApplicationDbContext>();

// Web layer: MVC controllers with views
builder.Services.AddControllersWithViews();

// Response caching support for OutputCache attribute usage
builder.Services.AddResponseCaching();

// Health checks for monitoring
builder.Services.AddHealthChecks();

// -------------------------------------------------------------------------
// Build the application pipeline
// -------------------------------------------------------------------------
var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    // Show detailed EF Core migration errors in development
    app.UseMigrationsEndPoint();
}
else
{
    // Production error handling
    app.UseExceptionHandler("/Home/Error");
    // HSTS: enforce HTTPS for 30 days (adjust for production)
    app.UseHsts();
}

// Redirect HTTP to HTTPS
app.UseHttpsRedirection();

// Serve static files from wwwroot
app.UseStaticFiles();

// Enable response caching middleware
app.UseResponseCaching();

// Enable routing
app.UseRouting();

// Enable authentication (must come before authorization)
app.UseAuthentication();

// Enable authorization
app.UseAuthorization();

// Health check endpoint
app.MapHealthChecks("/health");

// Map default MVC controller route: {controller=Home}/{action=Index}/{id?}
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Map Razor Pages (required for ASP.NET Core Identity UI scaffolded pages)
app.MapRazorPages();

app.Run();
