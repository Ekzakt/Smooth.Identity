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
                new IdentityResources.Profile()
            };


        public static IEnumerable<ApiScope> ApiScopes =>
            new ApiScope[]
            {
                new ApiScope("flauntapi.read"),
                new ApiScope("flauntapi.write")
            };


        public static IEnumerable<ApiResource> ApiResources =>
            new ApiResource[]
            {
                new ApiResource("flauntapi")
                {
                    Scopes = new List<string> { "flauntapi.read", "flauntapi.write" },
                    ApiSecrets = new List<Secret> { new Secret("apiResourceSecret".Sha256()) },
                    UserClaims = new List<string> { "role" }
                }
            };


        public static IEnumerable<Client> Clients =>
            new Client[]
            {
                // m2m client credentials flow client
                // Flaunt.Api
                new Client
                {
                    ClientId = "m2m.client",
                    ClientName = "Client Credentials Client",

                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    ClientSecrets = { new Secret("secret_m2m.client".Sha256()) },

                    AllowedScopes = { "flauntapi.read", "flauntapi.write" }
                },

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
                // },

                new Client
                {
                    ClientId = "RazorDemo",
                    ClientSecrets = { new Secret("secret".Sha256()) },

                    AllowedGrantTypes = GrantTypes.Code,
            
                    // where to redirect to after login
                    RedirectUris = { "https://localhost:7150/signin-oidc" },

                    // where to redirect to after logout
                    PostLogoutRedirectUris = { "https://localhost:7150/signout-callback-oidc" },

                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile
                    }
                },

                new Client
                {
                    ClientId = "Flaunt.Shop", // m2m.client
                    ClientSecrets = { new Secret("ecb76798-1d21-4278-bb83-3d1d07ad186e".Sha256()) },

                    AllowedGrantTypes = GrantTypes.Code,
            
                    // where to redirect to after login
                    RedirectUris = { "https://localhost:7051/signin-oidc" },

                    // where to redirect to after logout
                    PostLogoutRedirectUris = { "https://localhost:7051/signout-callback-oidc" },

                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "flauntapi.read"
                    }
                }
            };
    }
}
