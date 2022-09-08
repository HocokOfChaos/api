using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using RoshdefAPI.Admin.AutoMapper;
using RoshdefAPI.Admin.Middleware;
using RoshdefAPI.Admin.Models;
using RoshdefAPI.Admin.Repositories;
using RoshdefAPI.Admin.Repositories.Core;
using RoshdefAPI.Admin.Services;
using RoshdefAPI.Admin.Services.Core;
using RoshdefAPI.Areas.Identify.Stores;
using RoshdefAPI.Entity.Repositories;
using RoshdefAPI.Entity.Repositories.Core;
using RoshdefAPI.Entity.Services;
using RoshdefAPI.Entity.Services.Core;
using RoshdefAPI.Shared.Models.Configuration;
using RoshdefAPI.Shared.Services;
using RoshdefAPI.Shared.Services.Core;
using Serilog;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

// Setup logger
var logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();
builder.Logging.ClearProviders();
builder.Host.UseSerilog(logger);

// Add services to the container.
var services = builder.Services;
// Load settings
services.Configure<ApplicationSettings>(builder.Configuration.GetSection(nameof(ApplicationSettings)));
var settings = builder.Configuration.GetSection(nameof(ApplicationSettings)).Get<ApplicationSettings>();

services.AddControllersWithViews();

if (settings.UseHTTPS)
{
    services.AddHttpsRedirection(options =>
    {
        options.RedirectStatusCode = (int)HttpStatusCode.TemporaryRedirect;
        options.HttpsPort = settings.HTTPSPort;
    });
}

if (settings.UseForwardedHeaders)
{
    services.Configure<ForwardedHeadersOptions>(options =>
    {
        options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    });
}

// Setup auto mapper
services.AddAutoMapper(mc =>
{
    mc.AddProfile(new AutoMapperProfile());
},
    typeof(Program).Assembly
);

services.AddRazorPages();

// Custom services
services.AddSingleton<IShopItemsService, ShopItemsService>();
services.AddSingleton<IDOTALocalizationService, DOTALocalizationReaderService>();
services.AddTransient<IAPIKeyValidator, DedicatedServerKeyValidator>();
services.AddTransient<ErrorHandlerMiddleware>();
services.AddScoped<IViewRenderService, ViewRenderService>();
services.AddHostedService<PreloadSingletonsService>();

// Database related stuff
services.AddSingleton<IQueryBuilder, QueryBuilderMySQL>();
services.AddScoped<IDatabaseConnectionProvider, DatabaseConnectionProviderMySQL>();
// Unit of work & repositories
services.AddScoped<IUnitOfWork, UnitOfWork>();
services.AddScoped<PlayersRepositoryBase, PlayersRepository>();
services.AddScoped<PlayersItemsRepositoryBase, PlayersItemRepository>();
services.AddScoped<PlayersLogsRepositoryBase, PlayersLogsRepository>();
services.AddScoped<DailyRewardsRepositoryBase, DailyRewardsRepository>();
services.AddScoped<DailyRewardsItemsRepositoryBase, DailyRewardsItemsRepository>();
services.AddScoped<MatchesRepositoryBase, MatchesRepository>();
services.AddScoped<MatchesPlayersRepositoryBase, MatchPlayerRepository>();
services.AddScoped<ConfigRepositoryBase, ConfigRepository>();
services.AddScoped<LeaderboardPlayersRepositoryBase, LeaderboardPlayersRepository>();
services.AddScoped<UsersRepositoryBase, UsersRepository>();
services.AddScoped<UsersRolesRepositoryBase, UsersRolesRepository>();

// Identity things
services.AddIdentity<User, UserRole>()
    .AddDefaultTokenProviders();

services.Configure<IdentityOptions>(options =>
{
    // Password settings.
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;

    // Lockout settings.
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    // User settings.
    options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = false;
});

services.ConfigureApplicationCookie(options =>
{
    // Cookie settings
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(480);

    options.LoginPath = "/Identity/Account/Login";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
    options.SlidingExpiration = true;
});

// Identity Services
services.AddTransient<IUserStore<User>, UserStore>();
services.AddTransient<IRoleStore<UserRole>, RoleStore>();

// Localization
services.Configure<LocalizationSettings>(builder.Configuration.GetSection(nameof(LocalizationSettings)));
var localizationSettings = builder.Configuration.GetSection(nameof(LocalizationSettings)).Get<LocalizationSettings>();
var localizationSupportedCultures = localizationSettings.SupportedCultureInfos.ToList();
var localizationDefaultCulture = new RequestCulture(
    localizationSettings.DefaultCulture,
    localizationSettings.DefaultCulture
);

services.AddSingleton<IJsonStringLocalizer, JsonStringLocalizer>();
services.AddSingleton<IJsonViewLocalizer, JsonViewLocalizer>();

services.Configure<RequestLocalizationOptions>(options =>
{
    options.DefaultRequestCulture = localizationDefaultCulture;
    // Formatting numbers, dates, etc.
    options.SupportedCultures = localizationSupportedCultures;
    // UI strings that we have localized.
    options.SupportedUICultures = localizationSupportedCultures;

    /*
    options.RequestCultureProviders.Insert(0, new CustomRequestCultureProvider(context =>
    {
        //...
        var userLangs = context.Request.Headers["Accept-Language"].ToString();
        var firstLang = userLangs.Split(',').FirstOrDefault();
        var defaultLang = string.IsNullOrEmpty(firstLang) ? options.DefaultRequestCulture.UICulture.Name : firstLang;
        return Task.FromResult(new ProviderCultureResult(defaultLang, defaultLang));
    }));  */
});

// Application
var app = builder.Build();

// global error handler
app.UseMiddleware<ErrorHandlerMiddleware>();

if (!app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error/HandleStatusCode/500");
}

app.UseStatusCodePagesWithReExecute("/Error/HandleStatusCode/{0}");

app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = localizationDefaultCulture,
    // Formatting numbers, dates, etc.
    SupportedCultures = localizationSupportedCultures,
    // UI strings that we have localized.
    SupportedUICultures = localizationSupportedCultures
});

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
