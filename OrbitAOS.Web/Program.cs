using OrbitAOS.Application;
using OrbitAOS.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// -----------------------------------------------------------------------
// Register services from Clean Architecture layers
// -----------------------------------------------------------------------

// Infrastructure layer: EF Core, Identity, Repositories
builder.Services.AddInfrastructureServices(builder.Configuration);

// Application layer: Business logic services
builder.Services.AddApplicationServices();

// Web layer: MVC controllers with views
builder.Services.AddControllersWithViews();

var app = builder.Build();

// -----------------------------------------------------------------------
// Configure the HTTP request pipeline
// -----------------------------------------------------------------------
if (app.Environment.IsDevelopment())
{
    // Show EF Core migration errors in development
    app.UseMigrationsEndPoint();
}
else
{
    // Production error handling
    app.UseExceptionHandler("/Home/Error");
    // HSTS: enforce HTTPS for 30 days (adjust for production)
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Authentication must come before Authorization
app.UseAuthentication();
app.UseAuthorization();

// Default MVC route: {controller=Home}/{action=Index}/{id?}
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Map Razor Pages for ASP.NET Core Identity UI scaffolded pages
app.MapRazorPages();

app.Run();
