using IdentityModel;
using Microsoft.AspNetCore.Identity;
using Serilog;
using Smooth.Identity.Models;
using System.Security.Claims;

namespace Smooth.Identity;

public class SeedData
{
    public static void EnsureSeedData(WebApplication app)
    {
        Log.Information("Seeding database...");

        using (var scope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
        {
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            var alice = userManager.FindByNameAsync("alice").Result;

            if (alice == null)
            {
                alice = new ApplicationUser
                {
                    UserName = "alice",
                    Email = "AliceSmith@email.com",
                    EmailConfirmed = true,
                };

                var result = userManager.CreateAsync(alice, "Pass123$").Result;

                if (!result.Succeeded)
                {
                    throw new Exception(result.Errors.First().Description);
                }

                result = userManager.AddClaimsAsync(alice, new Claim[]{
                            new Claim(JwtClaimTypes.Name, "Alice Smith"),
                            new Claim(JwtClaimTypes.GivenName, "Alice"),
                            new Claim(JwtClaimTypes.FamilyName, "Smith"),
                            new Claim(JwtClaimTypes.WebSite, "http://alice.com"),
                        }).Result;

                if (!result.Succeeded)
                {
                    throw new Exception(result.Errors.First().Description);
                }

                Log.Debug("Alice created.");
            }
            else
            {
                Log.Debug("Alice already exists.");
            }

            var bob = userManager.FindByNameAsync("bob").Result;

            if (bob == null)
            {
                bob = new ApplicationUser
                {
                    UserName = "bob",
                    Email = "BobSmith@email.com",
                    EmailConfirmed = true
                };

                var result = userManager.CreateAsync(bob, "Pass123$").Result;

                if (!result.Succeeded)
                {
                    throw new Exception(result.Errors.First().Description);
                }

                result = userManager.AddClaimsAsync(bob, new Claim[]{
                            new Claim(JwtClaimTypes.Name, "Bob Smith"),
                            new Claim(JwtClaimTypes.GivenName, "Bob"),
                            new Claim(JwtClaimTypes.FamilyName, "Smith"),
                            new Claim(JwtClaimTypes.WebSite, "http://bob.com"),
                            new Claim("location", "somewhere")
                        }).Result;

                if (!result.Succeeded)
                {
                    throw new Exception(result.Errors.First().Description);
                }
                Log.Debug("Bob created.");
            }
            else
            {
                Log.Debug("Bob already exists.");
            }
        }

        Log.Information("Done seeding database. Exiting.");
    }
}
