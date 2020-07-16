// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4.Models;
using System.Collections.Generic;
using IdentityServer4.Test;

namespace StsServerIdentity
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
            };
        }

        public static IEnumerable<ApiResource> GetApis()
        {
            return new ApiResource[]
            {
                new ApiResource("api1", "My API #1")
            };
        }

        public static IEnumerable<Client> GetClients()
        {
            return new[]
            {
                // client credentials flow client
                new Client
                {
                    ClientId = "client",
                    ClientName = "Client Credentials Client",

                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    ClientSecrets = {new Secret("511536EF-F270-4058-80CA-1C89C192F69A".Sha256())},

                    AllowedScopes = {"api1"}
                },

                // MVC client using hybrid flow
                new Client
                {
                    ClientId = "mvc",
                    ClientName = "MVC Client",

                    AllowedGrantTypes = GrantTypes.HybridAndClientCredentials,
                    ClientSecrets = {new Secret("49C1A7E1-0C79-4A89-A3D6-A37998FB86B0".Sha256())},

                    RedirectUris = {"https://localhost:44386/signin-oidc"},
                    FrontChannelLogoutUri = "https://localhost:44386/signout-oidc",
                    PostLogoutRedirectUris = {"https://localhost:44386/signout-callback-oidc"},

                    AllowOfflineAccess = true,
                    AllowedScopes = {"openid", "profile", "api1"}
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
                        "https://localhost:44387/index.html",
                        "https://localhost:44387/callback.html",
                        "https://localhost:44387/silent.html",
                        "https://localhost:44387/popup.html",
                    },

                    PostLogoutRedirectUris = {"https://localhost:44386/index.html"},
                    AllowedCorsOrigins = {"https://localhost:44387"},

                    AllowedScopes = {"openid", "profile", "api1"}
                },

                // legacy client mit ressource owner password flow
                new Client
                {
                    ClientId = "legacy-js",
                    ClientName = "Legacy JS Client",
                    ClientUri = "https://localhost:44387",

                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                    AllowAccessTokensViaBrowser = true,
                    AllowedCorsOrigins = {"https://localhost:44387"},
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

        public static List<TestUser> GetTestUsers()
        {
            return new List<TestUser>
            {
                new TestUser
                {
                    Username = "Katie",
                    Password = "Pass123$",
                    SubjectId = "1"
                },
                new TestUser
                {
                    Username = "Jason",
                    Password = "Pass123$",
                    SubjectId = "2"
                },
            };
        }
    }
}