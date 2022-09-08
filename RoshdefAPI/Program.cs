using Serilog;
using RoshdefAPI.Services;
using RoshdefAPI.Middleware;
using RoshdefAPI.Entity.Repositories.Core;
using RoshdefAPI.Entity.Repositories;
using System.Net;
using RoshdefAPI.Entity.Services;
using RoshdefAPI.Entity.Services.Core;
using RoshdefAPI.AutoMapper;
using RoshdefAPI.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.HttpOverrides;
using RoshdefAPI.Shared.Models.Configuration;
using RoshdefAPI.Shared.Services.Core;
using RoshdefAPI.Shared.Services;

var builder = WebApplication.CreateBuilder(args);

// Setup logger
var logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();
builder.Logging.ClearProviders();
builder.Host.UseSerilog(logger);

var env = builder.Environment;

// Serilog debug
//Serilog.Debugging.SelfLog.Enable(msg => throw new Exception(msg));

// Add services to the container.
var services = builder.Services;
// Load settings
services.Configure<ApplicationSettings>(builder.Configuration.GetSection(nameof(ApplicationSettings)));
var settings = builder.Configuration.GetSection(nameof(ApplicationSettings)).Get<ApplicationSettings>();

// To make ModelBindingValidationFilter work
services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

services.AddControllers(config =>
{
    //Custom filters
    config.Filters.Add<ModelBindingValidationFilter>();
});


// Setup auto mapper
services.AddAutoMapper(mc =>
    {
        mc.AddProfile(new AutoMapperProfile());
    },
    typeof(Program).Assembly
);

if (env.IsDevelopment())
{
    services.AddSwaggerGen();
}
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

// Custom services
services.AddTransient<IAPIKeyValidator, DedicatedServerKeyValidator>();
services.AddTransient<APIKeyValidatorMiddleware>();
services.AddTransient<ErrorHandlerMiddleware>();
services.AddTransient<RequestResponseLoggingMiddleware>();
services.AddSingleton<IShopItemsService, ShopItemsService>();
services.AddSingleton<IQuestsService, QuestsService>();
services.AddHostedService<LoadKeyValuesService>();
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

var app = builder.Build();

if (settings.UseForwardedHeaders)
{
    app.UseForwardedHeaders();
}

// Custom middlewares
app.UseMiddleware<RequestResponseLoggingMiddleware>(); // must be first
app.UseMiddleware<APIKeyValidatorMiddleware>();
app.UseMiddleware<ErrorHandlerMiddleware>(); // must be last?

// Configure the HTTP request pipeline.
if (env.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (settings.UseHTTPS)
{
    app.UseHttpsRedirection();
}

// Save all logs
app.Lifetime.ApplicationStopped.Register(() =>
{
    Log.CloseAndFlush();
});

app.MapControllers();
app.Run();