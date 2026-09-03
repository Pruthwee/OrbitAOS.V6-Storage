using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using OrbitAOS.V6.Data;
using StackExchange.Redis;
using WebOptimizer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();

// cr-dotnet-1000: Register AWS ElastiCache (Redis) as the distributed cache provider
// for async/non-blocking data retrieval in Razor view controllers.
var redisConnectionString = builder.Configuration.GetConnectionString("RedisConnection")
    ?? Environment.GetEnvironmentVariable("REDIS_CONNECTION_STRING")
    ?? "localhost:6379";

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = redisConnectionString;
    options.InstanceName = "OrbitAOS_";
});

// cr-dotnet-1011: Register LigerShark WebOptimizer to bundle and minify CSS/JS assets.
// Optimized bundles are delivered via AWS CloudFront to reduce cloud egress costs and
// improve page load performance. Fingerprinted bundle URLs enable long-lived CDN caching.
builder.Services.AddWebOptimizer(pipeline =>
{
    // Bundle and minify all application CSS into a single fingerprinted bundle.
    // ~/css/site.css is the application stylesheet; additional CSS files can be appended here.
    pipeline.AddCssBundle("/css/bundle.css",
        "css/site.css");

    // Bundle and minify all application JS into a single fingerprinted bundle.
    // ~/js/site.js is the application script; additional JS files can be appended here.
    pipeline.AddJavaScriptBundle("/js/bundle.js",
        "js/site.js");
}, minifyJavaScript: true, minifyCss: true, enableTagHelperBundling: true);

// cr-dotnet-1016: Enable ResponseCompression middleware with Gzip and Brotli providers
// to reduce egress bandwidth costs on AWS-hosted ASP.NET applications (Elastic Beanstalk/ECS).
// Brotli is preferred by modern browsers; Gzip serves as the universal fallback.
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<BrotliCompressionProvider>();
    options.Providers.Add<GzipCompressionProvider>();
    options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[]
    {
        "text/html",
        "text/css",
        "application/javascript",
        "application/json",
        "text/plain",
        "image/svg+xml"
    });
});
builder.Services.Configure<BrotliCompressionProviderOptions>(options =>
{
    options.Level = System.IO.Compression.CompressionLevel.Fastest;
});
builder.Services.Configure<GzipCompressionProviderOptions>(options =>
{
    options.Level = System.IO.Compression.CompressionLevel.SmallestSize;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

// cr-dotnet-1016: Apply ResponseCompression middleware early in the pipeline so all
// subsequent middleware responses (Razor HTML, JSON, static files) are compressed.
app.UseResponseCompression();

// cr-dotnet-1011: Enable WebOptimizer middleware BEFORE UseStaticFiles so that
// bundled/minified assets are served with fingerprinted URLs.
// cr-dotnet-1010: Configure StaticFileMiddleware with Cache-Control headers for AWS CloudFront CDN.
// Versioned assets (e.g. /js/app.v1.js) are cached immutably for 1 year; all other static files
// receive a 7-day max-age so CloudFront edge nodes can serve them without hitting the origin.
var staticFileContentTypeProvider = new FileExtensionContentTypeProvider();

app.UseWebOptimizer();

app.UseStaticFiles(new StaticFileOptions
{
    ContentTypeProvider = staticFileContentTypeProvider,
    OnPrepareResponse = ctx =>
    {
        var headers = ctx.Context.Response.Headers;
        var path    = ctx.File.Name;

        // Immutable cache for fingerprinted / versioned assets (filename contains a hash or version token).
        // Pattern: any file whose name contains a dot-separated segment that looks like a version/hash
        // e.g. site.min.abc123.css, app.v2.js, bundle.1a2b3c4d.js
        bool isVersioned = System.Text.RegularExpressions.Regex.IsMatch(
            path, @"\.[a-f0-9]{6,}\.|\.v\d+\.", System.Text.RegularExpressions.RegexOptions.IgnoreCase);

        if (isVersioned)
        {
            // Tell CloudFront (and browsers) this asset never changes — cache for 1 year.
            headers["Cache-Control"] = "public, max-age=31536000, immutable";
        }
        else
        {
            // Non-versioned assets: allow CloudFront to cache for 7 days; browsers revalidate.
            headers["Cache-Control"] = "public, max-age=604800, must-revalidate";
        }
    }
});

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
