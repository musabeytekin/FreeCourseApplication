// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using IdentityServer4.Models;
using System.Collections.Generic;
using IdentityServer4;

namespace FreeCourse.IdentityServer
{
    public static class Config
    {
        public static IEnumerable<ApiResource> ApiResources =>
            new ApiResource[]
            {
                new ApiResource("resource_catalog"){Scopes={"catalog_fullpermission"}},
                new ApiResource("resource_photo_stock"){Scopes={"photo_stock_fullpermission"}},
                new ApiResource("resource_basket"){Scopes={"basket_fullpermission"}},
                new ApiResource("resource_discount"){Scopes={"discount_fullpermission"}},
                new ApiResource(IdentityServerConstants.LocalApi.ScopeName)
            };
        public static IEnumerable<IdentityResource> IdentityResources =>
            new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Email(),
                new IdentityResources.Profile(),
                new IdentityResource(){Name="roles",DisplayName="Roles",Description="User roles",UserClaims=new []{"role"}}
            };

        public static IEnumerable<ApiScope> ApiScopes =>
            new ApiScope[]
            {
                new ApiScope("catalog_fullpermission","full access permission for Catalog API "),
                new ApiScope("photo_stock_fullpermission","full access permission for PhotoStock API "),
                new ApiScope("basket_fullpermission","full access permission for Basket API "),
                new ApiScope("discount_fullpermission","full access permission for Discount API "),
                
                new ApiScope(IdentityServerConstants.LocalApi.ScopeName)
            };

        public static IEnumerable<Client> Clients =>
            new Client[]
            {
                new Client
                {
                    ClientName = "Asp.Net Core MVC",
                    ClientId = "WebMvcClient",
                    ClientSecrets = {new Secret("secret".Sha256())},
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    AllowedScopes = {"catalog_fullpermission","photo_stock_fullpermission",IdentityServerConstants.LocalApi.ScopeName}
                },
                
                new Client
                {
                    ClientName = "Asp.Net Core MVC",
                    ClientId = "WebMvcClientForUser",
                    AllowOfflineAccess = true,
                    ClientSecrets = {new Secret("secret".Sha256())},
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                    AllowedScopes =
                    {
                        "basket_fullpermission",
                        "discount_fullpermission",
                        IdentityServerConstants.StandardScopes.Email, 
                        IdentityServerConstants.StandardScopes.OpenId, 
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.OfflineAccess,  // for refresh token, kullanici login olmasa dahi refresh token ile token alabilir.
                        IdentityServerConstants.LocalApi.ScopeName,
                        "roles"
                    },
                    AccessTokenLifetime = 1*60*60, // 1 saat
                    RefreshTokenExpiration = TokenExpiration.Absolute,
                    AbsoluteRefreshTokenLifetime = (int)(DateTime.Now.AddDays(60)-DateTime.Now).TotalSeconds, // 60 gun
                    RefreshTokenUsage = TokenUsage.ReUse, // refresh token kullanildiktan sonra tekrar kullanilabilir
                }
                
            };
    }
}