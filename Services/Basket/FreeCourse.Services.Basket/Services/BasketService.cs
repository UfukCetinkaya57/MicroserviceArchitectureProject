using FreeCourse.Services.Basket.Dtos;
using FreeCourse.Shared.Dtos;
using System.Text.Json;

namespace FreeCourse.Services.Basket.Services
{
    // BasketService, sepet işlemlerini gerçekleştiren bir servis sınıfıdır.
    // Redis üzerinde sepet verilerini saklamak, güncellemek, getirmek ve silmek için gerekli metotları içerir.
    public class BasketService : IBasketService
    {
        private readonly RedisService _redisService; // Redis veritabanına erişim sağlamak için kullanılan servis.

        // Constructor, RedisService bağımlılığını alır ve sınıf içindeki alan değişkenine atar.
        public BasketService(RedisService redisService)
        {
            _redisService = redisService;
        }

        // Sepeti silmek için kullanılan metot.
        // Kullanıcı ID'sine göre Redis'ten sepet verisini siler.
        public async Task<Response<bool>> Delete(string userId)
        {
            // Redis'ten ilgili kullanıcıya ait sepet anahtarını siler.
            var status = await _redisService.GetDb().KeyDeleteAsync(userId);

            // Silme işlemi başarılıysa 204 (No Content) durumu döner, aksi takdirde 404 (Not Found) durumu döner.
            return status ? Response<bool>.Success(204) : Response<bool>.Fail("Basket not found", 404);
        }

        // Sepeti almak için kullanılan metot.
        // Kullanıcı ID'sine göre Redis'ten sepet verisini getirir.
        public async Task<Response<BasketDto>> GetBasket(string userId)
        {
            // Redis'ten kullanıcıya ait sepet verisini alır.
            var existBasket = await _redisService.GetDb().StringGetAsync(userId);

            // Eğer sepet bulunamazsa 404 (Not Found) durumu döner.
            if (String.IsNullOrEmpty(existBasket))
            {
                return Response<BasketDto>.Fail("Basket not found", 404);
            }

            // Sepet bulunursa, JSON formatındaki veriyi deserialize ederek döner.
            return Response<BasketDto>.Success(JsonSerializer.Deserialize<BasketDto>(existBasket), 200);
        }

        // Sepeti kaydetmek veya güncellemek için kullanılan metot.
        // Sepet verisini JSON formatında serialize ederek Redis'e kaydeder.
        public async Task<Response<bool>> SaveOrUpdate(BasketDto basketDto)
        {
            // Sepet verisini kullanıcı ID'siyle birlikte Redis'e kaydeder.
            var status = await _redisService.GetDb().StringSetAsync(basketDto.Userld, JsonSerializer.Serialize(basketDto));

            // İşlem başarılıysa 204 (No Content), başarısızsa 500 (Internal Server Error) durumu döner.
            return status ? Response<bool>.Success(204) : Response<bool>.Fail("Basket could not update or save", 500);
        }
    }
}
