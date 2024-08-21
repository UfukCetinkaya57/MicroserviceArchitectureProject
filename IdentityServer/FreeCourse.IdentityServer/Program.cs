// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Apache Lisansı, Sürüm 2.0 altında lisanslanmıştır. Lisans bilgisi için projedeki LICENSE dosyasına bakın.

using FreeCourse.IdentityServer.Data; // Uygulama veritabanı bağlamı
using FreeCourse.IdentityServer.Models; // Uygulama model sınıfları
using Microsoft.AspNetCore.Hosting; // ASP.NET Core web barındırma
using Microsoft.AspNetCore.Identity; // ASP.NET Core kimlik yönetimi
using Microsoft.EntityFrameworkCore; // Entity Framework Core
using Microsoft.Extensions.Configuration; // Yapılandırma ayarları
using Microsoft.Extensions.DependencyInjection; // Servis bağımlılıkları
using Microsoft.Extensions.Hosting; // Barındırma ortamı
using Serilog; // Serilog için günlüğe kaydetme
using Serilog.Events; // Günlük olayları
using Serilog.Sinks.SystemConsole.Themes; // Konsol temaları
using System; // Temel sistem sınıfları
using System.Linq; // LINQ

namespace FreeCourse.IdentityServer
{
    public class Program
    {
        // Ana giriş noktası
        public static int Main(string[] args)
        {
            // Serilog günlüğü yapılandırması
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug() // Minimum günlük seviyesi
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning) // Microsoft için günlük seviyesini geçersiz kıl
                .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information) // Barındırma sürekliliği için
                .MinimumLevel.Override("System", LogEventLevel.Warning) // Sistem için günlük seviyesi
                .MinimumLevel.Override("Microsoft.AspNetCore.Authentication", LogEventLevel.Information) // Kimlik doğrulama için
                .Enrich.FromLogContext() // Günlük bağlamından bilgi ekle
                                         // Azure'a yazmak için yorumdan çıkar
                                         //.WriteTo.File(
                                         //    @"D:\home\LogFiles\Application\identityserver.txt",
                                         //    fileSizeLimitBytes: 1_000_000,
                                         //    rollOnFileSizeLimit: true,
                                         //    shared: true,
                                         //    flushToDiskInterval: TimeSpan.FromSeconds(1))
                .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}", theme: AnsiConsoleTheme.Code) // Konsola yazdır
                .CreateLogger(); // Logger'ı oluştur

            try
            {
                // Uygulama ana bilgisayarını oluştur
                var host = CreateHostBuilder(args).Build();

                // Kapsama al ve veritabanı güncellemelerini uygula
                using (var scope = host.Services.CreateScope())
                {
                    var serviceProvider = scope.ServiceProvider;
                    var applicationDbContext = serviceProvider.GetRequiredService<ApplicationDbContext>();
                    applicationDbContext.Database.Migrate(); // Veritabanı göçlerini uygula

                    var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                    // Eğer kullanıcı yoksa, yeni bir kullanıcı oluştur
                    if (!userManager.Users.Any())
                    {
                        userManager.CreateAsync(new ApplicationUser
                        {
                            UserName = "ufukcetinkaya57", // Kullanıcı adı
                            Email = "ufukcetinkaya57@outlook.com", // Email
                            City = "İstanbul" // Şehir
                        }, "Password12*").Wait(); // Varsayılan şifre
                    }
                }

                Log.Information("Starting host..."); // Sunucu başlatılıyor
                host.Run(); // Ana bilgisayarı çalıştır
                return 0; // Başarı durumu
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly."); // Hata durumu
                return 1; // Hata durumu
            }
            finally
            {
                Log.CloseAndFlush(); // Günlük kaydını kapat
            }
        }

        // Ana bilgisayar oluşturucu
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args) // Varsayılan ayarları kullan
                .UseSerilog() // Serilog kullan
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>(); // Startup sınıfını kullan
                });
    }
}
