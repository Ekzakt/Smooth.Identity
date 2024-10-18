using Azure.Identity;
using Azure.Security.KeyVault.Certificates;
using Azure.Security.KeyVault.Secrets;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;
using Serilog;
using Smooth.Identity.Data;
using Smooth.Identity.Models;
using System.Security.Cryptography.X509Certificates;

namespace Smooth.Identity;

internal static class HostingExtensions
{
    public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
    {
        var configuration = builder.Configuration;
        var sqlConnectionString = configuration.GetConnectionString("DefaultConnectionString");
        var migrationsAssembly = typeof(Config).Assembly.GetName().Name;

        Log.Information("*** Configuring RouteOptions ***");
        builder.Services.Configure<RouteOptions>(routeOptions =>
        {
            routeOptions.LowercaseUrls = true;
        });

        Log.Information("*** Adding Razor Pages ***");
        builder.Services.AddRazorPages();

        Log.Information("*** Adding Controllers ***");
        builder.Services.AddControllersWithViews();

        Log.Information("*** Configuring Database Context ***");
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(sqlConnectionString));

        Log.Information("*** Adding Identity ***");
        builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        Log.Information("*** Adding CORS ***");
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

        Log.Information("*** Configuring Forwarded Headers ***");
        builder.Services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            options.KnownNetworks.Clear();
            options.KnownProxies.Clear();
        });

        Log.Information("*** Adding IdentityServer ***");
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

                options.EnableTokenCleanup = true;
                options.TokenCleanupInterval = 3600;
            })
            .AddServerSideSessions()
            .AddInMemoryIdentityResources(Config.IdentityResources)
            .AddInMemoryApiScopes(Config.ApiScopes)
            .AddInMemoryClients(Config.Clients(builder.Configuration))
            .AddInMemoryApiResources(Config.ApiResources)
            .AddAspNetIdentity<ApplicationUser>()
            .AddDeveloperSigningCredential();
        //    .AddSigningCredential(GetSigningCertificate(
        //        builder.Configuration["Azure:KeyVault:VaultUri"]!,
        //        builder.Configuration["Azure:KeyVault:CertificateName"]!)
        //      );

        Log.Information("*** Adding Authentication ***");
        builder.Services.AddAuthentication();

        return builder.Build();
    }

    public static WebApplication ConfigurePipeline(this WebApplication app)
    {
        Log.Information("*** UseFordedHeaders ***");
        app.UseForwardedHeaders();
        app.UseSerilogRequestLogging();
        app.UseCors("IdentityServerCorsPolicy");

        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();

        app.UseStaticFiles();
        app.UseRouting();

        app.UseCookiePolicy(new CookiePolicyOptions
        {
            Secure = CookieSecurePolicy.Always,
            MinimumSameSitePolicy = SameSiteMode.None
        });

        app.UseIdentityServer();
        app.UseAuthorization();

        app.MapRazorPages().RequireAuthorization();

        return app;
    }

    #region Helpers

    private static DefaultAzureCredentialOptions GetDefaultAzureCredentialOptions()
    {
        var credentials = new DefaultAzureCredentialOptions
        {
            ExcludeEnvironmentCredential = true,
            ExcludeInteractiveBrowserCredential = true,
            ExcludeAzurePowerShellCredential = true,
            ExcludeSharedTokenCacheCredential = true,
            ExcludeVisualStudioCodeCredential = true,
            ExcludeVisualStudioCredential = true,
            ExcludeAzureCliCredential = false,
            ExcludeManagedIdentityCredential = false
        };

        return credentials;
    }


    private static X509Certificate2 GetSigningCertificate(string keyVaultUri, string certificateName)
    {
        Log.Information("*** Retrieving signing certificate from Key Vault...");
        var client = new CertificateClient(new Uri(keyVaultUri), new DefaultAzureCredential(GetDefaultAzureCredentialOptions()));

        Log.Information("*** Retrieving certificate...");
        var certificateWithPrivateKey = client.GetCertificate(certificateName);
        Log.Information("*** " + certificateName + " retrieved with value " + certificateWithPrivateKey.Value);
        var certificate = certificateWithPrivateKey.Value;

        // Download the secret associated with the certificate to get the private key
        Log.Information("*** Retrieving secret...");
        var secretClient = new SecretClient(new Uri(keyVaultUri), new DefaultAzureCredential(GetDefaultAzureCredentialOptions()));
        var secret = secretClient.GetSecret(certificateName);
        Log.Information("*** " + certificateName + " secret retrieved with value " + secret.Value.Value);

        // Create X509Certificate2 with private key
        return new X509Certificate2(Convert.FromBase64String(secret.Value.Value));
    }

    #endregion Helpers
}