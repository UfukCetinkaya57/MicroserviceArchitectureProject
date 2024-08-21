// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Apache Lisansı, Sürüm 2.0 altında lisanslanmıştır. Lisans bilgisi için projedeki LICENSE dosyasına bakın.

using IdentityServer4; // IdentityServer4 kütüphanesi
using IdentityServer4.Models; // IdentityServer4 model sınıfları
using System; // Temel sistem sınıfları
using System.Collections.Generic; // Koleksiyonlar için

namespace FreeCourse.IdentityServer
{
    public static class Config
    {
        // API kaynaklarını tanımlar
        public static IEnumerable<ApiResource> ApiResources => new ApiResource[]
        {
            new ApiResource("resource_catalog"){Scopes={"catalog_fullpermission"}},
            new ApiResource("resource_photo_stock"){Scopes={"photo_stock_fullpermission"}},
            new ApiResource("resource_basket"){Scopes={"basket_fullpermission"}},
            new ApiResource("resource_discount"){Scopes={"discount_fullpermission"}},
            new ApiResource("resource_order"){Scopes={"order_fullpermission"}},
            new ApiResource("resource_payment"){Scopes={"payment_fullpermission"}},
            new ApiResource("resource_gateway"){Scopes={"gateway_fullpermission"}},
            new ApiResource(IdentityServerConstants.LocalApi.ScopeName) // Yerel API için varsayılan kaynak
        };

        // Kimlik kaynaklarını tanımlar
        public static IEnumerable<IdentityResource> IdentityResources =>
            new IdentityResource[]
            {
                new IdentityResources.Email(), // Email kaynağı
                new IdentityResources.OpenId(), // OpenID kaynağı
                new IdentityResources.Profile(), // Profil kaynağı
                new IdentityResource() // Kullanıcı rolleri
                {
                    Name = "roles",
                    DisplayName = "Roles",
                    Description = "Kullanıcı rolleri",
                    UserClaims = new[] { "role" } // Kullanıcı iddiaları
                }
            };

        // API kapsamlarını tanımlar
        public static IEnumerable<ApiScope> ApiScopes =>
            new ApiScope[]
            {
                new ApiScope("catalog_fullpermission", "Catalog API için full erişim"),
                new ApiScope("photo_stock_fullpermission", "Photo Stock API için full erişim"),
                new ApiScope("basket_fullpermission", "Basket API için full erişim"),
                new ApiScope("discount_fullpermission", "Discount API için full erişim"),
                new ApiScope("order_fullpermission", "Order API için full erişim"),
                new ApiScope("payment_fullpermission", "Payment API için full erişim"),
                new ApiScope("gateway_fullpermission", "Gateway API için full erişim"),
                new ApiScope(IdentityServerConstants.LocalApi.ScopeName) // Yerel API kapsamı
            };

        // İstemci ayarlarını tanımlar
        public static IEnumerable<Client> Clients =>
            new Client[]
            {
                new Client
                {
                    ClientName = "Asp.Net Core MVC", // İstemci adı
                    ClientId = "WebMvcClient", // İstemci kimliği
                    ClientSecrets = { new Secret("secret".Sha256()) }, // İstemci sırrı
                    AllowedGrantTypes = GrantTypes.ClientCredentials, // İzin verilen grant türleri
                    AllowedScopes = { "catalog_fullpermission", "photo_stock_fullpermission", "gateway_fullpermission", IdentityServerConstants.LocalApi.ScopeName } // İzin verilen kapsamlar
                },
                new Client
                {
                    ClientName = "Asp.Net Core MVC", // İstemci adı
                    ClientId = "WebMvcClientForUser", // İstemci kimliği
                    AllowOfflineAccess = true, // Offline erişime izin ver
                    ClientSecrets = { new Secret("secret".Sha256()) }, // İstemci sırrı
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword, // İzin verilen grant türü
                    AllowedScopes = { "basket_fullpermission", "order_fullpermission", "gateway_fullpermission", IdentityServerConstants.StandardScopes.Email, IdentityServerConstants.StandardScopes.OpenId, IdentityServerConstants.StandardScopes.Profile, IdentityServerConstants.StandardScopes.OfflineAccess, IdentityServerConstants.LocalApi.ScopeName, "roles" }, // İzin verilen kapsamlar
                    AccessTokenLifetime = 1 * 60 * 60, // Erişim token süresi (1 saat)
                    RefreshTokenExpiration = TokenExpiration.Absolute, // Refresh token süresi
                    AbsoluteRefreshTokenLifetime = (int)(DateTime.Now.AddDays(60) - DateTime.Now).TotalSeconds, // Refresh token mutlak süresi
                    RefreshTokenUsage = TokenUsage.ReUse // Refresh token kullanımı
                },
                new Client
                {
                    ClientName = "Token Exchange Client", // İstemci adı
                    ClientId = "TokenExhangeClient", // İstemci kimliği
                    ClientSecrets = { new Secret("secret".Sha256()) }, // İstemci sırrı
                    AllowedGrantTypes = new[] { "urn:ietf:params:oauth:grant-type:token-exchange" }, // İzin verilen grant türü
                    AllowedScopes = { "discount_fullpermission", "payment_fullpermission", IdentityServerConstants.StandardScopes.OpenId } // İzin verilen kapsamlar
                },
            };
    }
}
