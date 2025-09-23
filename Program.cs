using Deploying_Test.Data;
using Deploying_Test.Helper;
using Deploying_Test.Models.Entities;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ------- Connecting to Database --------

var cs = builder.Configuration.GetConnectionString("DefaultConnection")
         ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is missing.");

if (builder.Environment.IsDevelopment())
{
    // make local startup fast if DB is down/slow
    if (!cs.Contains("Connection Timeout", StringComparison.OrdinalIgnoreCase)) cs += ";Connection Timeout=3";
    if (!cs.Contains("Default Command Timeout", StringComparison.OrdinalIgnoreCase)) cs += ";Default Command Timeout=5";
}

builder.Services.AddDbContextPool<ApplicationDbContext>(options =>
{
    var ver = new MySqlServerVersion(new Version(8, 0, 36));
    options.UseMySql(cs, ver, mySql =>
    {
        //mySql.SchemaBehavior(MySqlSchemaBehavior.Ignore);
        if (builder.Environment.IsProduction())
            mySql.EnableRetryOnFailure(5, TimeSpan.FromSeconds(3), null);
        else
            mySql.EnableRetryOnFailure(1, TimeSpan.FromMilliseconds(200), null);
    });
});


// ----------------- Identity / Cookie -----------------
builder.Services
    .AddIdentity<Owner, IdentityRole>(o =>
    {
        o.Password.RequiredLength = 6;
        o.Password.RequireDigit = true;
        o.Password.RequireNonAlphanumeric = false;
        o.User.RequireUniqueEmail = true;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(opt =>
{
    opt.Cookie.Name = "Deploy_cookie";
    opt.Cookie.HttpOnly = true;
    opt.SlidingExpiration = true;

    if (builder.Environment.IsDevelopment())
    {
        opt.Cookie.SameSite = SameSiteMode.Lax;
        opt.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    }
    else
    {
        // cross-site from your FE (required for cookies across domains)
        opt.Cookie.SameSite = SameSiteMode.None;
        opt.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    }

    opt.Events.OnRedirectToLogin = ctx => { ctx.Response.StatusCode = StatusCodes.Status401Unauthorized; return Task.CompletedTask; };
    opt.Events.OnRedirectToAccessDenied = ctx => { ctx.Response.StatusCode = StatusCodes.Status403Forbidden; return Task.CompletedTask; };
});


// ----------------- CORS -----------------
var allowed = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ??
    new[]
    {
        "http://localhost:5173",
        "https://localhost:5173",
        // add your custom domain here if you later use one, e.g. "https://app.example.com"
    };

builder.Services.AddCors(o =>
{
    o.AddPolicy("api", p =>
    {
        p.SetIsOriginAllowed(origin =>
        {
            // keep exact allowlist
            if (allowed.Contains(origin)) return true;

            // allow any HTTPS Railway subdomain (helps with preview URLs)
            // e.g. https://<anything>.up.railway.app
            if (Uri.TryCreate(origin, UriKind.Absolute, out var uri))
                return uri.Scheme == Uri.UriSchemeHttps && uri.Host.EndsWith(".railway.app");

            return false;
        })
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials();
    });
});






///
var app = builder.Build();

if (app.Environment.IsProduction())
{
    var port = Environment.GetEnvironmentVariable("PORT") ?? "5000";
    app.Urls.Add($"http://0.0.0.0:{port}");

    var fh = new ForwardedHeadersOptions
    {
        ForwardedHeaders = ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedFor
    };
    fh.KnownNetworks.Clear();
    fh.KnownProxies.Clear();
    app.UseForwardedHeaders(fh);

    // endpoint routing
    app.UseRouting();

    // CORS must be after routing and before auth
    app.UseCors("api");

    app.UseAuthentication();
    app.UseAuthorization();

    // Swagger in prod
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Deployment app  API v1");
        c.RoutePrefix = "swagger";
    });

    // Controllers
    app.MapControllers();

    // ensure ALL preflights return CORS headers
    app.MapMethods("{*path}", new[] { "OPTIONS" }, () => Results.Ok()).RequireCors("api");
}
else // DEV
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Deploying_Test API v1");
        c.RoutePrefix = "swagger";
    });

    app.UseHttpsRedirection();

    // endpoint routing
    app.UseRouting();

    // CORS after routing and before auth
    app.UseCors("api");

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    // helpful in dev too
    app.MapMethods("{*path}", new[] { "OPTIONS" }, () => Results.Ok()).RequireCors("api");
}

// Diagnostics
app.MapGet("/", () => "API is running, Test3");
app.MapGet("/health/db", async (ApplicationDbContext db) =>
    await db.Database.CanConnectAsync() ? Results.Ok("DB OK") : Results.StatusCode(500));

// -------------- Migrations + seed --------------
var runMigrationsOnStart = builder.Configuration.GetValue("Db:MigrateOnStart", app.Environment.IsProduction());
if (runMigrationsOnStart)
{
    using var scope = app.Services.CreateScope();
    var svcs = scope.ServiceProvider;
    var db = svcs.GetRequiredService<ApplicationDbContext>();
    await db.Database.MigrateAsync();
    await IdentitySeed.SeedAsync(svcs, app.Configuration);
}

await app.RunAsync();     


