using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OrbitAOS.V6.Data;
using OrbitAOS.V6.Domain.Interfaces;
using OrbitAOS.V6.Infrastructure.Repositories;
using OrbitAOS.V6.Application.Services;
using OrbitAOS.V6.Application.Interfaces;

// ============================================================
// OrbitAOS.V6 - ASP.NET Core MVC on .NET 8
// Clean Architecture: Domain / Application / Infrastructure / Web
// ============================================================

var builder = WebApplication.CreateBuilder(args);

// ----------------------------------------------------------
// 1. DATA LAYER (Infrastructure)
// ----------------------------------------------------------
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// ----------------------------------------------------------
// 2. IDENTITY (Infrastructure)
// ----------------------------------------------------------
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = true;
    // Password policy (adjust to match legacy requirements)
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<ApplicationDbContext>();

// ----------------------------------------------------------
// 3. APPLICATION SERVICES (Application Layer)
// ----------------------------------------------------------
builder.Services.AddScoped<IHomeService, HomeService>();

// ----------------------------------------------------------
// 4. INFRASTRUCTURE REPOSITORIES (Infrastructure Layer)
// ----------------------------------------------------------
// Register repositories here as the application grows
// builder.Services.AddScoped<IUserRepository, UserRepository>();

// ----------------------------------------------------------
// 5. MVC + VIEWS
// ----------------------------------------------------------
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// ----------------------------------------------------------
// 6. RESPONSE CACHING (replaces legacy OutputCache)
// ----------------------------------------------------------
builder.Services.AddResponseCaching();

// ----------------------------------------------------------
// 7. SESSION (if needed)
// ----------------------------------------------------------
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// ----------------------------------------------------------
// 8. HTTP CONTEXT ACCESSOR (replaces HttpContext.Current)
// ----------------------------------------------------------
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// ----------------------------------------------------------
// MIDDLEWARE PIPELINE
// ----------------------------------------------------------
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // HSTS: 30 days default. Adjust for production.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseResponseCaching();

app.UseAuthentication();
app.UseAuthorization();

app.UseSession();

// ----------------------------------------------------------
// ROUTING (replaces App_Start/RouteConfig.cs)
// ----------------------------------------------------------
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();
