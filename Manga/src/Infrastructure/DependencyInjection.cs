using Manga.Application.Common.Interfaces;
using Manga.Domain.Constants;
using Manga.Infrastructure.BackgrounServices;
using Manga.Infrastructure.Data;
using Manga.Infrastructure.Data.Interceptors;
using Manga.Infrastructure.Identity;
using Manga.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.MSSqlServer;

namespace Microsoft.Extensions.DependencyInjection;
public static class DependencyInjection
{
    public static void AddInfrastructureServices(this IHostApplicationBuilder builder)
    {
        var connectionString = builder.Configuration.GetConnectionString(Env.MangaDb);
        Guard.Against.Null(connectionString, message: "Connection string 'MangaDb' not found.");

        builder.Services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();
        builder.Services.AddScoped<ISaveChangesInterceptor, DispatchDomainEventsInterceptor>();

        builder.Services.AddDbContext<ApplicationDbContext>((sp, options) =>
        {
            options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
            options.UseSqlServer(connectionString);
        });

        builder.EnrichSqlServerDbContext<ApplicationDbContext>();

        builder.Services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());

        builder.Services.AddScoped<ApplicationDbContextInitialiser>();

        builder.Services.AddAuthentication()
            .AddBearerToken(IdentityConstants.BearerScheme, opt =>
            {
                opt.Events.OnMessageReceived = context =>
                {
                    var accessToken = context.Request.Query["access_token"];
                    var path = context.HttpContext.Request.Path;

                    if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/system-usage"))
                    {
                        context.Token = accessToken;
                    }
                    return Task.CompletedTask;
                };
            });

        builder.Services.AddAuthorizationBuilder();

        builder.Services
            .AddIdentityCore<ApplicationUser>()
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddApiEndpoints();

        builder.Services.AddSingleton(TimeProvider.System);
        builder.Services.AddTransient<IIdentityService, IdentityService>();

        builder.Services.AddAuthorization(options =>
            options.AddPolicy(Policies.CanPurge, policy => policy.RequireRole(Roles.Administrator)));

        #region SignalR Configuration
        var urls = builder.Configuration.GetSection(Env.AccessURLs).Get<List<string>>();
        builder.Services.AddCors(o => o.AddPolicy(Env.CorsPolicy, p => p.WithOrigins([.. urls]).AllowAnyHeader().AllowAnyMethod().AllowCredentials()));
        builder.Services.AddSignalR();
        #endregion

        #region Application logs configuration
        var logOption = new MSSqlServerSinkOptions
        {
            AutoCreateSqlTable = true,
            TableName = "ApplicationLogs",
        };
        builder.Services.AddSerilog(provider => provider
        .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
        .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
        .MinimumLevel.Override("System", LogEventLevel.Warning)
        .MinimumLevel.Override("System", LogEventLevel.Warning)
        .Enrich.WithProperty("Manga", null, true)
        .WriteTo.Console());
        #endregion

        #region Services DI
        builder.Services.AddSingleton<ISystemProcessingService, SystemProcessingService>();
        builder.Services.AddSingleton<CollectSystemUsage>();
        builder.Services.AddSingleton<ICollectSystemUsage>(provider => provider.GetRequiredService<CollectSystemUsage>());
        builder.Services.AddHostedService(provider => provider.GetRequiredService<CollectSystemUsage>());
        #endregion
    }
}
