using Dapper; // Dapper ORM kütüphanesini kullanmak için
using FreeCourse.Services.Discount.Models; // İndirim modeli sınıfına erişim sağlamak için
using FreeCourse.Shared.Dtos; // Paylaşılan DTO'lara (Data Transfer Object) erişim sağlamak için
using Npgsql; // PostgreSQL veri tabanına bağlantı kurmak için Npgsql kütüphanesini kullanmak için
using System.Data; // Veri tabanı işlemleri için gerekli olan IDbConnection arayüzünü kullanmak için

namespace FreeCourse.Services.Discount.Services
{
    public class DiscountService : IDiscountService
    {
        private readonly IConfiguration _configuration; // Uygulama yapılandırma ayarlarına erişmek için
        private readonly IDbConnection _dbConnection; // Veri tabanı bağlantısı için

        // Yapılandırıcı: Yapılandırma ayarlarını enjekte eder ve PostgreSQL veri tabanına bir bağlantı kurar
        public DiscountService(IConfiguration configuration)
        {
            _configuration = configuration;
            _dbConnection = new NpgsqlConnection(_configuration.GetConnectionString("PostgreSql"));
        }

        // Belirtilen ID'ye sahip indirimi veri tabanından siler
        public async Task<Response<NoContent>> Delete(int id)
        {
            var status = await _dbConnection.ExecuteAsync("delete from discount where id=@Id", new { Id = id });
            return status > 0
                ? Response<NoContent>.Success(204)
                : Response<NoContent>.Fail("Discount not found", 404); // Silme işlemi başarılıysa 204, değilse 404 döndürür
        }

        // Tüm indirimleri veri tabanından alır
        public async Task<Response<List<Models.Discount>>> GetAll()
        {
            var discounts = await _dbConnection.QueryAsync<Models.Discount>("Select * from discount");
            return Response<List<Models.Discount>>.Success(discounts.ToList(), 200); // İndirimleri başarılı bir şekilde döndürür
        }

        // Kullanıcı ID'si ve kodu eşleşen indirimi veri tabanından alır
        public async Task<Response<Models.Discount>> GetByCodeAndUserId(string code, string userId)
        {
            var discounts = await _dbConnection.QueryAsync<Models.Discount>(
                "Select * from discount where userid=@UserId and code=@Code",
                new { UserId = userId, Code = code });

            var hasDiscount = discounts.FirstOrDefault(); // İndirim varsa alır

            if (hasDiscount == null)
            {
                return Response<Models.Discount>.Fail("Discount not found", 404); // İndirim bulunamazsa 404 döndürür
            }
            return Response<Models.Discount>.Success(hasDiscount, 200); // Başarılı bir şekilde indirim döndürür
        }

        // Belirtilen ID'ye sahip indirimi veri tabanından alır
        public async Task<Response<Models.Discount>> GetById(int id)
        {
            var discount = (await _dbConnection.QueryAsync<Models.Discount>(
                "Select * from discount where id=@Id",
                new { id })).SingleOrDefault(); // Tek bir indirim döndürür

            if (discount == null)
            {
                return Response<Models.Discount>.Fail("Discount not found", 404); // İndirim bulunamazsa 404 döndürür
            }
            return Response<Models.Discount>.Success(discount, 200); // Başarılı bir şekilde indirim döndürür
        }

        // Yeni bir indirim kaydeder
        public async Task<Response<NoContent>> Save(Models.Discount discount)
        {
            var saveStatus = await _dbConnection.ExecuteAsync(
                "INSERT INTO discount(userid, rate, code) VALUES(@UserId, @Rate, @Code)",
                discount);
            if (saveStatus > 0)
            {
                return Response<NoContent>.Success(204); // Kayıt başarılıysa 204 döndürür
            }
            else
            {
                return Response<NoContent>.Fail("an error occurred while adding", 500); // Hata durumunda 500 döndürür
            }
        }

        // Mevcut bir indirimi günceller
        public async Task<Response<NoContent>> Update(Models.Discount discount)
        {
            var status = await _dbConnection.ExecuteAsync(
                "update discount set userid=@UserId, code=@Code, rate=@Rate where id=@Id",
                new
                {
                    Id = discount.Id,
                    UserId = discount.UserId,
                    Code = discount.Code,
                    Rate = discount.Rate
                });

            if (status > 0)
            {
                return Response<NoContent>.Success(204); // Güncelleme başarılıysa 204 döndürür
            }
            else
            {
                return Response<NoContent>.Fail("Discount not found", 404); // İndirim bulunamazsa 404 döndürür
            }
        }
    }
}
