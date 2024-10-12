using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Smooth.Identity.Data;
using Smooth.Identity.Models;

namespace Smooth.Identity;

internal static class HostingExtensions
{
    public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
    {
        var configuration = builder.Configuration;
        var sqlConnectionString = configuration.GetConnectionString("DefaultConnectionString");
        var migrationsAssembly = typeof(Config).Assembly.GetName().Name;

        builder.Services.Configure<RouteOptions>(routeOptions =>
        {
            routeOptions.LowercaseUrls = true;
        });

        builder.Services.AddRazorPages();
        builder.Services.AddControllersWithViews();

        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(sqlConnectionString));

        builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        builder.Services.AddCors(options =>
        {
            var corsOrigins = configuration["CorsOrigins"]!.Split(',');

            if (corsOrigins != null)
            {
                options.AddPolicy("IdentityServerCorsPolicy", policy =>
                {
                    policy.WithOrigins(corsOrigins)
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
            }
        });

        var clients = Config.Clients(builder.Configuration);

        builder.Services
            .AddIdentityServer(options =>
            {
                options.UserInteraction.LoginUrl = "/Account/Login";
                options.UserInteraction.LogoutUrl = "/Account/Logout";
                options.UserInteraction.ErrorUrl = "/Home/Error";
                options.UserInteraction.ConsentUrl = "/Consent";

                options.IssuerUri = configuration["IdentityServer:IssuerUri"];
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;
                options.ServerSideSessions.UserDisplayNameClaimType = "name";
                options.EmitStaticAudienceClaim = true;
            })
            .AddOperationalStore(options =>
            {
                options.ConfigureDbContext = builder =>
                    builder.UseSqlServer(sqlConnectionString, sqlServerOptionsAction =>
                        sqlServerOptionsAction.MigrationsAssembly(migrationsAssembly));

                // this enables automatic token cleanup. this is optional.
                options.EnableTokenCleanup = true;
                options.TokenCleanupInterval = 3600; // interval in seconds (default is 3600)
            })
            .AddServerSideSessions()
            .AddInMemoryIdentityResources(Config.IdentityResources)
            .AddInMemoryApiScopes(Config.ApiScopes)
            .AddInMemoryClients(Config.Clients(builder.Configuration))
            .AddInMemoryApiResources(Config.ApiResources)
            .AddAspNetIdentity<ApplicationUser>();

        builder.Services.AddAuthentication();

        return builder.Build();
    }

    public static WebApplication ConfigurePipeline(this WebApplication app)
    {
        app.UseSerilogRequestLogging();

        app.UseCors("IdentityServerCorsPolicy");

        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseHttpsRedirection();

        app.UseStaticFiles();
        app.UseRouting();

        app.UseIdentityServer();
        app.UseAuthorization();

        app.MapRazorPages().RequireAuthorization();

        return app;
    }
}