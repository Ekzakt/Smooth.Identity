using Duende.IdentityServer;
using Duende.IdentityServer.Models;

namespace Smooth.Identity
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> IdentityResources =>
            [
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email()
            ];


        public static IEnumerable<ApiScope> ApiScopes =>
            [
                new ApiScope("flauntapi.read", "Read data from Flaunt API."),
                new ApiScope("flauntapi.write", "Write data to flaunt API.")
            ];


        public static IEnumerable<ApiResource> ApiResources =>
            [
                new ApiResource("Flaunt.Api")
                {
                    Scopes = new List<string> { "flauntapi.read", "flauntapi.write" },
                    //ApiSecrets = new List<Secret> { new Secret("apiResourceSecret".Sha256()) }
                }
            ];


        public static IEnumerable<Client> Clients(IConfiguration configuration) =>
            [
                new Client
                {
                    Enabled = true,
                    ClientId = "Smooth.Web",
                    ClientName = "Smooth Web Client",
                    ClientSecrets = { new Secret(configuration["IdentityServer:Clients:0:ClientSecret"].Sha256()) },
                    AllowedGrantTypes = GrantTypes.Code,
                    RedirectUris = { configuration["IdentityServer:Clients:0:BaseUri"]! + "/signin-oidc" },
                    PostLogoutRedirectUris = { configuration["IdentityServer:Clients:0:BaseUri"]! + "/signout-callback-oidc" },
                    //FrontChannelLogoutUri = configuration["IdentityServer:Clients:0:BaseUri"]! + "/signout-oidc",
                    //BackChannelLogoutUri = configuration["IdentityServer:BackChannelLogoutBaseUri"]! + "/signout-oidc",
                    AllowOfflineAccess = true,
                    RequireConsent = false,
                    RequirePkce = true,

                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        "flauntapi.read"
                    }
                },
                new Client
                {
                    Enabled = true,
                    ClientId = "Smooth.Shop",
                    ClientName = "Smooth Shop Client",
                    ClientSecrets = { new Secret(configuration["IdentityServer:Clients:1:ClientSecret"].Sha256()) },
                    AllowedGrantTypes = GrantTypes.Code,
                    RedirectUris = { configuration["IdentityServer:Clients:1:BaseUri"]! + "/signin-oidc" },
                    PostLogoutRedirectUris = { configuration["IdentityServer:Clients:1:BaseUri"]! + "/signout-callback-oidc" },
                    //FrontChannelLogoutUri = configuration["IdentityServer:Clients:1:BaseUri"]! + "/signout-oidc",
                    //BackChannelLogoutUri = configuration["IdentityServer:BackChannelLogoutBaseUri"]! + "/signout-oidc",
                    AllowOfflineAccess = true,
                    RequireConsent = false,
                    RequirePkce = true,

                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        "flauntapi.read"
                    }
                }
                // m2m client credentials flow client
                // Flaunt.Api
                //new Client
                //{
                //    ClientId = "m2m.client",
                //    ClientName = "Client Credentials Client",

                //    AllowedGrantTypes = GrantTypes.ClientCredentials,
                //    ClientSecrets = { new Secret("secret_m2m.client".Sha256()) },

                //    AllowedScopes = { "flauntapi.read", "flauntapi.write" }
                //}
            ];
    }
}
