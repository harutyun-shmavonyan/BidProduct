using System;
using IdentityServer4.Models;
using System.Collections.Generic;

namespace BidProduct.IdentityManagement
{
    public static class Config
    {
        public static IEnumerable<ApiResource> ApiResources => new[]
        {
            new ApiResource("BidProductApi", "Bid Product Api")
            {
                ApiSecrets = new List<Secret>
                {
                    new Secret("8AC859FF-C649-4E1E-AFE5-25CDF22E3745".Sha256())
                },
                Scopes = new List<string> {"appScope"}
            }
        };

        public static IEnumerable<IdentityResource> IdentityResources =>
                           new IdentityResource[]
                           {
                        new IdentityResources.OpenId(),
                        new IdentityResources.Profile(),
                           };

        public static IEnumerable<ApiScope> ApiScopes =>
            new[]
            {
                new ApiScope("appScope"),
            };

        public static IEnumerable<Client> Clients =>
            new[]
            {
                new Client
                {
                    ClientId = "bidProduct",
                    ClientSecrets = { new Secret("49C1A7E1-0C79-4A89-A3D6-A37998FB86B0".Sha256()) },

                    AllowedGrantTypes = GrantTypes.Code,

                    RedirectUris = { "https://localhost:7143/bff/exchange-token" },
                    FrontChannelLogoutUri = "https://localhost:7143/signout-oidc",
                    PostLogoutRedirectUris = { "https://localhost:7143/signout-callback-oidc" },

                    AccessTokenType = AccessTokenType.Reference,
                    RequireClientSecret = true,
                    AccessTokenLifetime = (int)TimeSpan.FromDays(1).TotalSeconds,

                    AllowOfflineAccess = true,
                    AllowedScopes = { "openid", "profile", "appScope" },

                    RefreshTokenUsage = TokenUsage.OneTimeOnly,
                    IdentityTokenLifetime = (int)TimeSpan.FromDays(10).TotalSeconds,

                    RequirePkce = false
                },
            };
    }
}