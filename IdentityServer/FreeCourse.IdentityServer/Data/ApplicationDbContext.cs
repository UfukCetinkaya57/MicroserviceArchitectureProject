using Microsoft.AspNetCore.Identity.EntityFrameworkCore; // ASP.NET Identity için Entity Framework Core entegrasyonu
using Microsoft.EntityFrameworkCore; // Entity Framework Core için
using FreeCourse.IdentityServer.Models; // Uygulama kullanıcı modelini kullanmak için

namespace FreeCourse.IdentityServer.Data
{
    // ApplicationUser ile birlikte IdentityDbContext'i miras alır
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        // Constructor, DbContextOptions parametresini alır
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) // Taban sınıfın constructor'ına geçer
        {
        }

        // Model oluşturma işlemlerini özelleştirmek için
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder); // Taban sınıfın model oluşturma işlemlerini çağırır

            // ASP.NET Identity modelini özelleştirin ve varsayılanları geçersiz kılın.
            // Örneğin, ASP.NET Identity tablo adlarını yeniden adlandırabilir ve daha fazlasını yapabilirsiniz.
            // base.OnModelCreating(builder) çağrısından sonra özelleştirmelerinizi ekleyin.
        }
    }
}
