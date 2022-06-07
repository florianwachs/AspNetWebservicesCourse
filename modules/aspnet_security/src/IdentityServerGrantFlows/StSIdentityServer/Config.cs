using Duende.IdentityServer.Models;

namespace StSIdentityServer;

public static class Config
{
    public static IEnumerable<IdentityResource> IdentityResources =>
        new IdentityResource[]
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
            new IdentityResources.Email(),
        };

    public static IEnumerable<ApiScope> ApiScopes =>
        new ApiScope[]
        {
            new ApiScope("api1"),
        };

    public static IEnumerable<Client> Clients =>
        new Client[]
        {
            // m2m client credentials flow client
            new Client
            {
                ClientId = "client",
                ClientName = "Client Credentials Client",

                AllowedGrantTypes = GrantTypes.ClientCredentials,
                ClientSecrets = { new Secret("511536EF-F270-4058-80CA-1C89C192F69A".Sha256()) },

                AllowedScopes = { "api1" }
            },           
            // SPA client mit implicit flow
                new Client
                {
                    ClientId = "spa",
                    ClientName = "SPA Client",
                    ClientUri = "https://localhost:44387",
                    RequireClientSecret = false,
                    AllowedGrantTypes = GrantTypes.Code,
                    AllowAccessTokensViaBrowser = true,

                    RedirectUris =
                    {
                        "https://localhost:5002/index.html",
                        "https://localhost:5002/callback.html",
                        "https://localhost:5002/silent.html",
                        "https://localhost:5002/popup.html",
                    },

                    PostLogoutRedirectUris = {"https://localhost:5002/index.html"},
                    AllowedCorsOrigins = {"https://localhost:5002"},

                    AllowedScopes = {"openid", "profile", "api1"}
                },
            // legacy client mit ressource owner password flow
                new Client
                {
                    ClientId = "legacy-js",
                    ClientName = "Legacy JS Client",
                    ClientUri = "https://localhost:5002",

                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                    AllowAccessTokensViaBrowser = true,
                    AllowedCorsOrigins = {"https://localhost:5002"},
                    AllowedScopes = {"openid", "profile", "api1"},
                    RequireClientSecret = false
                },

                new Client
                {
                    ClientId = "trusted-client",
                    ClientName = "Trusted C# Client",
                    ClientSecrets = {new Secret("secret".Sha256())},
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                    AllowedScopes = {"openid", "profile", "api1"}
                }
        };
}
