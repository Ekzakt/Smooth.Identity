using Duende.IdentityServer;
using Duende.IdentityServer.Models;

namespace Smooth.Identity
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> IdentityResources =>
            new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email()
            };


        public static IEnumerable<ApiScope> ApiScopes =>
            new ApiScope[]
            {
                new ApiScope("flauntapi.read", "Read data from Flaunt API."),
                new ApiScope("flauntapi.write", "Write data to flaunt API.")
            };


        public static IEnumerable<ApiResource> ApiResources =>
            new ApiResource[]
            {
                new ApiResource("Flaunt.Api")
                {
                    Scopes = new List<string> { "flauntapi.read", "flauntapi.write" },
                    //ApiSecrets = new List<Secret> { new Secret("apiResourceSecret".Sha256()) }
                }
            };


        public static IEnumerable<Client> Clients(IConfiguration config) =>
            new Client[]
            {
                new Client
                {
                    Enabled = true,

                    ClientId = "Smooth.Shop",
                    ClientName = "Smooth Shop Client",
                    ClientSecrets = { new Secret(config.GetValue<string>("IdentityServer:Clients:0:ClientSecret").Sha256()) },
                    AllowedGrantTypes = GrantTypes.Code,

                    RedirectUris = { $"{config.GetValue<string>("IdentityServer:Clients:0:BaseUri")}/signin-oidc" },
                    PostLogoutRedirectUris = { $"{config.GetValue<string>("IdentityServer:Clients:0:BaseUri")}/signout-callback-oidc" },

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

                    ClientId = "Smooth.Web",
                    ClientName = "Smooth Web Client",
                    ClientSecrets = { new Secret(config.GetValue<string>("IdentityServer:Clients:1:ClientSecret").Sha256()) },
                    AllowedGrantTypes = GrantTypes.Code,

                    RedirectUris = { $"{config.GetValue<string>("IdentityServer:Clients:1:BaseUri")}/signin-oidc" },
                    PostLogoutRedirectUris = { $"{config.GetValue<string>("IdentityServer:Clients:1:BaseUri")}/signout-callback-oidc" },

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
                //},

                // interactive client using code flow + pkce
                // Flaunt.Shop
                //new Client
                //{
                //    ClientId = "interactive",
                //    ClientSecrets = { new Secret("secret_interactive".Sha256()) },

                //    AllowedGrantTypes = GrantTypes.Code,

                //    RedirectUris = { "https://localhost:7051/signin-oidc" },
                //    //FrontChannelLogoutUri = "https://localhost:7051/signout-oidc",
                //    PostLogoutRedirectUris = { "https://localhost:7051/signout-callback-oidc" },

                //    //AllowOfflineAccess = true,

                //    //AllowedScopes = 
                //    //{ 
                //    //    IdentityServerConstants.StandardScopes.OpenId,
                //    //    IdentityServerConstants.StandardScopes.Profile, 
                //    //    "flauntapi.read"
                //    //},
                //    //RequireConsent = true

                //    AllowOfflineAccess = true,
                //    AllowedScopes = {"openid", "profile", "flauntapi.read"},
                //    RequirePkce = true,
                //    RequireConsent = true,
                //    AllowPlainTextPkce = false
                // }
            };
    }
}
