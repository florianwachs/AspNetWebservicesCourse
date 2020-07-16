// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using IdentityServer4;
using IdentityServer4.Models;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace StsServerIdentity
{
    public class Config
    {
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email()
            };
        }

        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource("api")
                {
                    Scopes =
                    {
                        new Scope("university-api", "University API")
                    }
                }
            };
        }

        public static IEnumerable<Client> GetClients(IConfigurationSection stsConfig)
        {
            return new List<Client>
            {
                new Client
                {
                    ClientId="postman-client",
                    ClientName="Postman Client",
                    AllowedGrantTypes= GrantTypes.Code,
                    AllowAccessTokensViaBrowser = true,
                    RequireConsent=false,
                    RedirectUris={ "https://www.getpostman.com/oauth2/callback"},
                    PostLogoutRedirectUris={"https://www.getpostman.com"},
                    AllowedCorsOrigins={"https://www.getpostman.com"},
                    EnableLocalLogin = true,
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        IdentityServerConstants.StandardScopes.OfflineAccess,
                        "university-api"
                    },
                    RequireClientSecret = false,
                    AlwaysIncludeUserClaimsInIdToken = true
                },
            };
        }
    }
}
