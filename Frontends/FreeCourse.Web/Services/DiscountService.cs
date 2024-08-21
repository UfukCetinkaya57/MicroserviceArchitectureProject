using FreeCourse.Shared.Dtos; // Ortak DTO'lar
using FreeCourse.Web.Models.Discounts; // İndirim modelleri
using FreeCourse.Web.Services.Interfaces; // Servis arayüzleri
using System; // Temel sistem sınıfları
using System.Collections.Generic; // Koleksiyon sınıfları
using System.Linq; // LINQ işlevleri
using System.Net.Http; // HTTP istemcisi
using System.Net.Http.Json; // JSON desteği
using System.Threading.Tasks; // Asenkron programlama için

namespace FreeCourse.Web.Services
{
    // İndirim servisi
    public class DiscountService : IDiscountService
    {
        private readonly HttpClient _httpClient; // HTTP istemcisi

        // Yapıcı metot
        public DiscountService(HttpClient httpClient)
        {
            _httpClient = httpClient; // HTTP istemcisini al
        }

        // İndirim kodu ile indirim bilgisi alma metodu
        public async Task<DiscountViewModel> GetDiscount(string discountCode)
        {
            // İndirim koduna göre indirim bilgisi almak için istek yap
            var response = await _httpClient.GetAsync($"discounts/GetByCode/{discountCode}");

            // Eğer istek başarısızsa, null döndür
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            // Başarılı bir yanıt aldıysak, içeriği oku ve indirim bilgilerini al
            var discount = await response.Content.ReadFromJsonAsync<Response<DiscountViewModel>>();

            return discount.Data; // İndirim verisini döndür
        }
    }
}
