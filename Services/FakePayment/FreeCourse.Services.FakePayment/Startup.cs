using MassTransit; // Mesajla�ma altyap�s� i�in MassTransit kullan�m�
using Microsoft.AspNetCore.Authentication.JwtBearer; // JWT tabanl� kimlik do�rulama i�in
using Microsoft.AspNetCore.Authorization; // Yetkilendirme i�lemleri i�in
using Microsoft.AspNetCore.Builder; // Uygulama yap�land�rmas� i�in
using Microsoft.AspNetCore.Hosting; // Web hosting i�lemleri i�in
using Microsoft.AspNetCore.Mvc; // MVC yap�land�rmas� i�in
using Microsoft.AspNetCore.Mvc.Authorization; // MVC'de yetkilendirme filtreleri i�in
using Microsoft.Extensions.Configuration; // Uygulama yap�land�rma ayarlar�na eri�im i�in
using Microsoft.Extensions.DependencyInjection; // Dependency Injection (Ba��ml�l�k Enjeksiyonu) yap�land�rmas� i�in
using Microsoft.Extensions.Hosting; // Uygulaman�n �al��ma ortam�na eri�im i�in
using Microsoft.OpenApi.Models; // Swagger dok�mantasyonu i�in
using System.IdentityModel.Tokens.Jwt; // JWT i�leme ve y�netimi i�in

namespace FreeCourse.Services.FakePayment
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // Uygulama servislerini yap�land�rmak i�in kullan�l�r.
        public void ConfigureServices(IServiceCollection services)
        {
            // MassTransit'i RabbitMQ ile yap�land�rma
            services.AddMassTransit(x =>
            {
                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(Configuration["RabbitMQUrl"], "/", host =>
                    {
                        host.Username("guest"); // RabbitMQ kullan�c� ad�
                        host.Password("guest"); // RabbitMQ �ifresi
                    });
                });
            });

            services.AddMassTransitHostedService(); // MassTransit'i hosted service olarak ekleme

            // Yetkilendirme politikas�: t�m kullan�c�lar�n kimlik do�rulamas� yapmas�n� gerektirir
            var requireAuthorizePolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();

            // JWT tokenlar�ndaki sub claim'ini varsay�lan olarak kald�r�r
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Remove("sub");

            // JWT tabanl� kimlik do�rulamay� yap�land�rma
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                options.Authority = Configuration["IdentityServerURL"]; // IdentityServer URL'si
                options.Audience = "resource_payment"; // API'n�n hedef kitlesi
                options.RequireHttpsMetadata = false; // HTTPS zorunlulu�unu kald�r�r (geli�tirme ortam� i�in)
            });

            // MVC'yi yap�land�rma ve yetkilendirme filtresi ekleme
            services.AddControllers(opt =>
            {
                opt.Filters.Add(new AuthorizeFilter(requireAuthorizePolicy));
            });

            // Swagger dok�mantasyonu yap�land�rmas�
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "FreeCourse.Services.FakePayment", Version = "v1" });
            });
        }

        // HTTP istekleri i�in uygulama pipeline'�n� yap�land�rmak i�in kullan�l�r.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Geli�tirme ortam� i�in yap�land�rmalar
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage(); // Geli�tirme istisna sayfas�
                app.UseSwagger(); // Swagger dok�mantasyonu
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "FreeCourse.Services.FakePayment v1"));
            }

            app.UseRouting(); // Y�nlendirme middleware'ini kullanma
            app.UseAuthentication(); // Kimlik do�rulama middleware'ini kullanma
            app.UseAuthorization(); // Yetkilendirme middleware'ini kullanma

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers(); // MVC controller'lar� i�in endpoint haritalama
            });
        }
    }
}
