using System;
using System.Net;
using System.Net.Sockets;
using Hangfire;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Net.Http.Headers;
using MrCMS.Apps;
using MrCMS.Entities.People;
using MrCMS.Helpers;
using MrCMS.Installation;
using MrCMS.Logging;
using MrCMS.Services;
using MrCMS.Services.Auth;
using MrCMS.Settings;
using MrCMS.Web.Admin;
using MrCMS.Web.Apps.Articles;
using MrCMS.Web.Apps.Core;
using MrCMS.Web.Hangfire;
using MrCMS.Website;
using MrCMS.Website.CMS;
using NHibernate;
using ILoggerFactory = Microsoft.Extensions.Logging.ILoggerFactory;

const int DefaultStartPort = 5000;
const int MaxPortAttempts = 100;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseDefaultServiceProvider(options =>
{
    options.ValidateScopes = false;
    options.ValidateOnBuild = false;
});

builder.Configuration.AddJsonFile("connectionStrings.json", optional: true, reloadOnChange: true);
builder.Configuration.AddEnvironmentVariables();

builder.WebHost.ConfigureKestrel(options =>
{
    options.ConfigureEndpointDefaults(x => x.UseConnectionLogging());
});

if (builder.Environment.IsDevelopment())
{
    var httpPort = FindAvailablePort(DefaultStartPort);
    var httpsPort = FindAvailablePort(httpPort + 1);

    builder.WebHost.UseUrls($"http://localhost:{httpPort}", $"https://localhost:{httpsPort}");

    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.WriteLine();
    Console.WriteLine($"  -> HTTP:  http://localhost:{httpPort}");
    Console.WriteLine($"  -> HTTPS: https://localhost:{httpsPort}");
    Console.WriteLine();
    Console.ResetColor();
}

TypeHelper.Initialize(typeof(Program).Assembly);

var configuration = builder.Configuration;
var environment = builder.Environment;
var services = builder.Services;

bool IsInstalled() => configuration.GetConnectionString("mrcms")?.Length > 0;
bool IsMiniProfileEnabled() => configuration.GetValue<bool>("EnableMiniProfiler");

// --- Services ---

if (environment.IsProduction())
{
    var connectionString = configuration["DataProtectionConnectionString"];
    var keyName = configuration["DataProtectionKeyName"];
    if (!string.IsNullOrWhiteSpace(connectionString))
    {
        services
            .AddDataProtection()
            .SetApplicationName("MrCMS")
            .PersistKeysToAzureBlobStorage(connectionString, "datakeys", keyName);
    }
}

var isInstalled = IsInstalled();
if (isInstalled)
{
    services.AddHangfire(cfg => cfg
        .UseSimpleAssemblyNameTypeSerializer()
        .UseRecommendedSerializerSettings()
        .UseSqlServerStorage(configuration.GetConnectionString("mrcms"))
    );
    services.AddHangfireServer(options => options.WorkerCount = 10);
}

services.AddDefaultIdentity<User>(options =>
    {
        options.User.RequireUniqueEmail = true;
        options.ClaimsIdentity.UserNameClaimType = nameof(User.Email);
        options.ClaimsIdentity.SecurityStampClaimType = nameof(User.SecurityStamp);
    })
    .AddRoles<UserRole>()
    .AddUserStore<UserStore>()
    .AddRoleStore<RoleStore>()
    .AddUserManager<UserManager>()
    .AddSignInManager<SignInManager>()
    .AddDefaultTokenProviders()
    .Services
    .AddScoped<IPasswordHasher<User>, MrCMSPasswordHasher>()
    .AddScoped<IClaimsTransformation, ImpersonationClaimsTransformation>()
    .ConfigureApplicationCookie(options =>
    {
        options.SlidingExpiration = true;
        options.ExpireTimeSpan = TimeSpan.FromDays(30);
    });

services.AddRequiredServices();
services.Configure<SystemConfig>(configuration.GetSection(SystemConfig.SectionName));
services.AddCultureInfo(configuration);
configuration.SetDefaultPageSize();
configuration.SetMaxFileSize();

var appContext = services.AddMrCMSApps(configuration, context =>
{
    context.RegisterApp<MrCMSAdmin>();
    context.RegisterApp<MrCMSCoreApp>();
    context.RegisterApp<MrCMSArticlesApp>();
});

services.AddMrCMSData(isInstalled, configuration, environment);
services.AddSiteProvider();
services.AddMrCMSFileSystem();
services.AddSignalR();
services.AddAutoMapper(expression =>
{
    expression.AllowNullDestinationValues = true;
    appContext.ConfigureAutomapper(expression);
}, typeof(Program).Assembly);

if (!isInstalled)
{
    services.AddInstallationServices();
}
else
{
    services.RegisterSiteLocator();
    services.RegisterAllDiscoveredServices();
    services.AddSingleton<IWebpageMetadataService, WebpageMetadataService>();

    services.AddMvcForMrCMS(appContext);
    services.Configure<FormOptions>(x => { x.MultipartBodyLengthLimit = SessionHelper.MaxFileSize; });
    services.AddSingleton<ICmsMethodTester, CmsMethodTester>();
    services.AddSingleton<IGetMrCMSParts, GetMrCMSParts>();
    services.AddSingleton<IAssignPageDataToRouteValues, AssignPageDataToRouteValues>();
    services.AddSingleton<IQuerySerializer, QuerySerializer>();

    services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();

    services.AddHttpsRedirection(options => { options.HttpsPort = 443; });

    services.AddScoped(x =>
    {
        var actionContext = x.GetRequiredService<IActionContextAccessor>().ActionContext;
        var factory = x.GetRequiredService<IUrlHelperFactory>();
        return factory.GetUrlHelper(actionContext);
    });

    services.Configure<SecurityStampValidatorOptions>(options =>
    {
        options.ValidationInterval = TimeSpan.FromMinutes(5);
    });

    services.AddAuthorization(options =>
    {
        options.AddPolicy("admin", policy => policy.RequireRole(UserRole.Administrator));
        appContext.ConfigureAuthorization(options);
    });

    services.AddHsts(options =>
    {
        options.Preload = true;
        options.IncludeSubDomains = true;
        options.MaxAge = TimeSpan.FromDays(60);
    });

    services.AddHostedService<StartupService>();

    configuration.SetMiniProfilerEnableStatus();
    if (IsMiniProfileEnabled())
    {
        services.AddMiniProfiler(options =>
        {
            options.RouteBasePath = "/profiler";
            options.PopupRenderPosition = StackExchange.Profiling.RenderPosition.BottomRight;
            options.PopupShowTimeWithChildren = true;
            options.ShouldProfile = x =>
            {
                if (!x.Path.HasValue)
                    return true;

                return !x.Path.Value!.EndsWith(".js", StringComparison.OrdinalIgnoreCase) &&
                       !x.Path.Value.EndsWith(".css", StringComparison.OrdinalIgnoreCase) &&
                       !x.Path.Value.EndsWith(".svg", StringComparison.OrdinalIgnoreCase) &&
                       !x.Path.Value.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) &&
                       !x.Path.Value.EndsWith(".png", StringComparison.OrdinalIgnoreCase) &&
                       !x.Path.Value.EndsWith(".gif", StringComparison.OrdinalIgnoreCase) &&
                       !x.Path.Value.EndsWith(".map", StringComparison.OrdinalIgnoreCase) &&
                       !x.Path.Value.EndsWith(".woff", StringComparison.OrdinalIgnoreCase) &&
                       !x.Path.Value.EndsWith(".woff2", StringComparison.OrdinalIgnoreCase) &&
                       !x.Path.Value.EndsWith(".ttf", StringComparison.OrdinalIgnoreCase) &&
                       !x.Path.Value.EndsWith(".eot", StringComparison.OrdinalIgnoreCase) &&
                       !x.Path.Value.EndsWith(".ico", StringComparison.OrdinalIgnoreCase);
            };
        });

        services.AddAntiforgery(options =>
        {
            options.SuppressXFrameOptionsHeader = false;
        });
    }
}

// --- Build ---

var app = builder.Build();

// --- Middleware ---

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
    app.UseHttpsRedirection();
}

app.UseForwardedHeaders();

app.UseCors("AllowAll");

app.Use(async (context, next) =>
{
    context.Response.Headers["X-Frame-Options"] = "SAMEORIGIN";
    context.Response.Headers["X-Xss-Protection"] = "1; mode=block";
    context.Response.Headers["X-Content-Type-Options"] = "nosniff";
    context.Response.Headers["Referrer-Policy"] = "same-origin";
    context.Response.Headers["X-Permitted-Cross-Domain-Policies"] = "none";
    await next();
});

app.UseStatusCodePagesWithReExecute("/HandleStatusCode/{0}");

app.UseSession();

if (!IsInstalled())
{
    app.ShowInstallation();
}
else
{
    if (IsMiniProfileEnabled())
    {
        app.UseMiniProfiler();
    }

    var loggerFactory = app.Services.GetRequiredService<ILoggerFactory>();
    var httpContextAccessor = app.Services.GetRequiredService<IHttpContextAccessor>();
    loggerFactory.AddProvider(
        new MrCMSDatabaseLoggerProvider(
            app.Services.GetRequiredService<ISessionFactory>(),
            httpContextAccessor));

    app.UseMrCMS(mrcms =>
    {
        mrcms.UseRequestLocalization();
        mrcms.UseStaticFiles(new StaticFileOptions
        {
            OnPrepareResponse = context =>
            {
                var headers = context.Context.Response.GetTypedHeaders();
                headers.CacheControl = new CacheControlHeaderValue
                {
                    Public = true,
                    MaxAge = app.Environment.IsDevelopment() ? TimeSpan.FromDays(0) : TimeSpan.FromDays(30)
                };
            }
        });
        mrcms.UseAuthentication();

        if (IsInstalled())
        {
            mrcms.RegisterJobs(app.Services);
        }

        mrcms.Use(async (context, next) =>
        {
            var maxAllowedFileSize = SessionHelper.MaxFileSize;
            var contentLength = context.Request.ContentLength ?? 0;
            if (contentLength > maxAllowedFileSize)
            {
                context.Response.StatusCode = StatusCodes.Status413PayloadTooLarge;
                await context.Response.WriteAsync(
                    $"File is too big ({contentLength / 1024 / 1024}MiB). Max filesize: {maxAllowedFileSize / 1024 / 1024}MiB.");
                return;
            }

            await next.Invoke();
        });

        if (IsMiniProfileEnabled())
        {
            app.UseMiniProfiler();
        }
    }, endpoints => { endpoints.MapRazorPages(); });
}

app.Run();

// --- Port helpers ---

static int FindAvailablePort(int startPort)
{
    for (var port = startPort; port < startPort + MaxPortAttempts; port++)
    {
        if (IsPortAvailable(port))
            return port;
    }

    throw new InvalidOperationException(
        $"No available port found in range {startPort}-{startPort + MaxPortAttempts - 1}");
}

static bool IsPortAvailable(int port)
{
    try
    {
        using var listener = new TcpListener(IPAddress.Loopback, port);
        listener.Start();
        listener.Stop();
        return true;
    }
    catch (SocketException)
    {
        return false;
    }
}
