// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;
using System.Collections.Generic;

namespace StsServerIdentity
{
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
                new ApiScope("api1", "Api 1"),
                new ApiScope(IdentityServerConstants.LocalApi.ScopeName),
            };

        public static IEnumerable<ApiResource> ApiResources =>
            new ApiResource[]
            {
                new ApiResource("api1", "Api 1")
                {
                    Scopes={"api1"}
                },
                new ApiResource(IdentityServerConstants.LocalApi.ScopeName),
            };



        public static IEnumerable<Client> Clients =>
            new Client[]
            {
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

                    AllowedScopes = {"openid", "profile", "api1", IdentityServerConstants.LocalApi.ScopeName }
                },
            };

        public static List<TestUser> TestUsers =>
             new List<TestUser>
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
