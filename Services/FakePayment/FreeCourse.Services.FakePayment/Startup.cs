using MassTransit; // Mesajlaþma altyapýsý için MassTransit kullanýmý
using Microsoft.AspNetCore.Authentication.JwtBearer; // JWT tabanlý kimlik doðrulama için
using Microsoft.AspNetCore.Authorization; // Yetkilendirme iþlemleri için
using Microsoft.AspNetCore.Builder; // Uygulama yapýlandýrmasý için
using Microsoft.AspNetCore.Hosting; // Web hosting iþlemleri için
using Microsoft.AspNetCore.Mvc; // MVC yapýlandýrmasý için
using Microsoft.AspNetCore.Mvc.Authorization; // MVC'de yetkilendirme filtreleri için
using Microsoft.Extensions.Configuration; // Uygulama yapýlandýrma ayarlarýna eriþim için
using Microsoft.Extensions.DependencyInjection; // Dependency Injection (Baðýmlýlýk Enjeksiyonu) yapýlandýrmasý için
using Microsoft.Extensions.Hosting; // Uygulamanýn çalýþma ortamýna eriþim için
using Microsoft.OpenApi.Models; // Swagger dokümantasyonu için
using System.IdentityModel.Tokens.Jwt; // JWT iþleme ve yönetimi için

namespace FreeCourse.Services.FakePayment
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // Uygulama servislerini yapýlandýrmak için kullanýlýr.
        public void ConfigureServices(IServiceCollection services)
        {
            // MassTransit'i RabbitMQ ile yapýlandýrma
            services.AddMassTransit(x =>
            {
                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(Configuration["RabbitMQUrl"], "/", host =>
                    {
                        host.Username("guest"); // RabbitMQ kullanýcý adý
                        host.Password("guest"); // RabbitMQ þifresi
                    });
                });
            });

            services.AddMassTransitHostedService(); // MassTransit'i hosted service olarak ekleme

            // Yetkilendirme politikasý: tüm kullanýcýlarýn kimlik doðrulamasý yapmasýný gerektirir
            var requireAuthorizePolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();

            // JWT tokenlarýndaki sub claim'ini varsayýlan olarak kaldýrýr
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Remove("sub");

            // JWT tabanlý kimlik doðrulamayý yapýlandýrma
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                options.Authority = Configuration["IdentityServerURL"]; // IdentityServer URL'si
                options.Audience = "resource_payment"; // API'nýn hedef kitlesi
                options.RequireHttpsMetadata = false; // HTTPS zorunluluðunu kaldýrýr (geliþtirme ortamý için)
            });

            // MVC'yi yapýlandýrma ve yetkilendirme filtresi ekleme
            services.AddControllers(opt =>
            {
                opt.Filters.Add(new AuthorizeFilter(requireAuthorizePolicy));
            });

            // Swagger dokümantasyonu yapýlandýrmasý
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "FreeCourse.Services.FakePayment", Version = "v1" });
            });
        }

        // HTTP istekleri için uygulama pipeline'ýný yapýlandýrmak için kullanýlýr.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Geliþtirme ortamý için yapýlandýrmalar
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage(); // Geliþtirme istisna sayfasý
                app.UseSwagger(); // Swagger dokümantasyonu
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "FreeCourse.Services.FakePayment v1"));
            }

            app.UseRouting(); // Yönlendirme middleware'ini kullanma
            app.UseAuthentication(); // Kimlik doðrulama middleware'ini kullanma
            app.UseAuthorization(); // Yetkilendirme middleware'ini kullanma

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers(); // MVC controller'larý için endpoint haritalama
            });
        }
    }
}
