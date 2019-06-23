// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using IdentityServer4.Models;
using IdentityServer4.Test;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

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

        public static IEnumerable<ApiResource> GetApiResources()
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
                new Client
                {
                    ClientId = "spa",
                    ClientName = "SPA Client",
                    ClientUri = "https://localhost:44356",
                    RequireClientSecret = false,
                    AllowedGrantTypes = GrantTypes.Code,
                    AllowAccessTokensViaBrowser = true,                    
                    EnableLocalLogin = false, // Damit nur der Externe Provider als Login verwendet werden kann
                    RequireConsent =false,
                    RedirectUris =
                    {
                        "https://localhost:44356/index.html",
                        "https://localhost:44356/callback.html",
                        "https://localhost:44356/silent.html",
                        "https://localhost:44356/popup.html",
                    },

                    PostLogoutRedirectUris = {"https://localhost:44386/index.html"},
                    AllowedCorsOrigins = {"https://localhost:44356"},

                    AllowedScopes = {"openid", "profile", "api1"}
                },

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