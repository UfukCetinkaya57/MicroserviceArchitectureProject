// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Apache Lisansı, Sürüm 2.0 altında lisanslanmıştır. Lisans bilgisi için projedeki LICENSE dosyasına bakın.

using IdentityServer4; // IdentityServer4 kullanımı
using FreeCourse.IdentityServer.Data; // Uygulama veritabanı bağlamı
using FreeCourse.IdentityServer.Models; // Uygulama model sınıfları
using Microsoft.AspNetCore.Builder; // ASP.NET Core yapılandırması
using Microsoft.AspNetCore.Hosting; // ASP.NET Core web barındırma
using Microsoft.AspNetCore.Identity; // ASP.NET Core kimlik yönetimi
using Microsoft.EntityFrameworkCore; // Entity Framework Core
using Microsoft.Extensions.Configuration; // Yapılandırma ayarları
using Microsoft.Extensions.DependencyInjection; // Servis bağımlılıkları
using Microsoft.Extensions.Hosting; // Barındırma ortamı
using FreeCourse.IdentityServer.Services; // Servis sınıfları

namespace FreeCourse.IdentityServer
{
    public class Startup
    {
        public IWebHostEnvironment Environment { get; } // Web barındırma ortamı
        public IConfiguration Configuration { get; } // Yapılandırma ayarları

        // Yapılandırıcı
        public Startup(IWebHostEnvironment environment, IConfiguration configuration)
        {
            Environment = environment; // Ortamı ayarla
            Configuration = configuration; // Yapılandırmayı ayarla
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // Kontrolcü ve görünüm hizmetlerini ekle
            services.AddControllersWithViews();
            services.AddLocalApiAuthentication(); // Yerel API kimlik doğrulamasını ekle

            // Veritabanı bağlamını ekle
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"))); // Bağlantı dizesini kullan

            // Kimlik hizmetlerini ekle
            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>() // EF çekirdek deposunu kullan
                .AddDefaultTokenProviders(); // Varsayılan jeton sağlayıcılarını ekle

            // IdentityServer yapılandırması
            var builder = services.AddIdentityServer(options =>
            {
                options.Events.RaiseErrorEvents = true; // Hata olaylarını yükselt
                options.Events.RaiseInformationEvents = true; // Bilgi olaylarını yükselt
                options.Events.RaiseFailureEvents = true; // Başarısızlık olaylarını yükselt
                options.Events.RaiseSuccessEvents = true; // Başarı olaylarını yükselt

                // Statik izleyici iddialarını yay
                options.EmitStaticAudienceClaim = true;
            })
                .AddInMemoryIdentityResources(Config.IdentityResources) // Bellekte kimlik kaynaklarını ekle
                .AddInMemoryApiResources(Config.ApiResources) // Bellekte API kaynaklarını ekle
                .AddInMemoryApiScopes(Config.ApiScopes) // Bellekte API kapsamlarını ekle
                .AddInMemoryClients(Config.Clients) // Bellekte istemcileri ekle
                .AddAspNetIdentity<ApplicationUser>(); // ASP.NET Identity ile entegre et

            // Özel doğrulayıcılar ekle
            builder.AddResourceOwnerValidator<IdentityResourceOwnerPasswordValidator>();
            builder.AddExtensionGrantValidator<TokenExchangeExtensionGrantValidator>();

            // Geliştirici imzalama anahtarını ekle (üretim için önerilmez)
            builder.AddDeveloperSigningCredential();

            // Google kimlik doğrulamasını ekle
            services.AddAuthentication()
                .AddGoogle(options =>
                {
                    options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme; // Giriş şemasını ayarla

                    // Google ile kaydol ve ayarları yapılandır
                    // https://console.developers.google.com adresine kaydol
                    // Google+ API'sini etkinleştir
                    // Yönlendirme URI'sını https://localhost:5001/signin-google olarak ayarla
                    options.ClientId = "copy client ID from Google here"; // Google'dan istemci kimliğini kopyala
                    options.ClientSecret = "copy client secret from Google here"; // Google'dan istemci gizlisini kopyala
                });
        }

        public void Configure(IApplicationBuilder app)
        {
            // Geliştirme ortamı ayarları
            if (Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage(); // Geliştirici hata sayfasını kullan
                app.UseDatabaseErrorPage(); // Veritabanı hata sayfasını kullan
            }

            app.UseStaticFiles(); // Statik dosyaları kullan

            app.UseRouting(); // Yönlendirmeyi kullan
            app.UseIdentityServer(); // IdentityServer'ı kullan
            app.UseAuthentication(); // Kimlik doğrulamayı kullan
            app.UseAuthorization(); // Yetkilendirmeyi kullan
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute(); // Varsayılan kontrolcü yolunu haritalandır
            });
        }
    }
}
