using FreeCourse.Shared.Dtos; // Paylaşılan DTO sınıfları
using FreeCourse.Web.Models.Baskets; // Sepetle ilgili model sınıfları
using FreeCourse.Web.Services.Interfaces; // Servis arayüzleri
using System; // Temel sistem sınıfları
using System.Collections.Generic; // Koleksiyon sınıfları
using System.Linq; // LINQ işlevleri
using System.Net.Http; // HTTP istemcisi
using System.Net.Http.Json; // JSON ile HTTP istekleri
using System.Threading.Tasks; // Asenkron programlama için

namespace FreeCourse.Web.Services
{
    // Sepet servis sınıfı
    public class BasketService : IBasketService
    {
        private readonly HttpClient _httpClient; // HTTP istemcisi
        private readonly IDiscountService _discountService; // İndirim servisi

        public BasketService(HttpClient httpClient, IDiscountService discountService)
        {
            _httpClient = httpClient; // HTTP istemcisi
            _discountService = discountService; // İndirim servisi
        }

        // Sepete ürün ekleme metodu
        public async Task AddBasketItem(BasketItemViewModel basketItemViewModel)
        {
            var basket = await Get(); // Mevcut sepeti al

            if (basket != null)
            {
                // Sepette aynı ürün yoksa ekle
                if (!basket.BasketItems.Any(x => x.CourseId == basketItemViewModel.CourseId))
                {
                    basket.BasketItems.Add(basketItemViewModel);
                }
            }
            else
            {
                // Sepet yoksa yeni bir sepet oluştur ve ürünü ekle
                basket = new BasketViewModel();
                basket.BasketItems.Add(basketItemViewModel);
            }

            await SaveOrUpdate(basket); // Sepeti kaydet veya güncelle
        }

        // İndirim uygulama metodu
        public async Task<bool> ApplyDiscount(string discountCode)
        {
            await CancelApplyDiscount(); // Önceki indirim iptal ediliyor

            var basket = await Get(); // Mevcut sepeti al
            if (basket == null)
            {
                return false;
            }

            var hasDiscount = await _discountService.GetDiscount(discountCode); // İndirim kodunu kontrol et
            if (hasDiscount == null)
            {
                return false;
            }

            basket.ApplyDiscount(hasDiscount.Code, hasDiscount.Rate); // İndirimi uygula
            await SaveOrUpdate(basket); // Sepeti kaydet
            return true;
        }

        // İndirim iptal etme metodu
        public async Task<bool> CancelApplyDiscount()
        {
            var basket = await Get(); // Mevcut sepeti al

            if (basket == null || basket.DiscountCode == null)
            {
                return false;
            }

            basket.CancelDiscount(); // İndirim iptal et
            await SaveOrUpdate(basket); // Sepeti kaydet
            return true;
        }

        // Sepeti silme metodu
        public async Task<bool> Delete()
        {
            var result = await _httpClient.DeleteAsync("baskets"); // Sepeti sil

            return result.IsSuccessStatusCode; // Başarılı olup olmadığını döndür
        }

        // Sepeti alma metodu
        public async Task<BasketViewModel> Get()
        {
            var response = await _httpClient.GetAsync("baskets"); // Sepeti al

            if (!response.IsSuccessStatusCode)
            {
                return null; // Başarısızsa null döndür
            }

            var basketViewModel = await response.Content.ReadFromJsonAsync<Response<BasketViewModel>>(); // JSON'dan sepete dönüştür

            return basketViewModel.Data; // Sepet verisini döndür
        }

        // Sepet öğesini kaldırma metodu
        public async Task<bool> RemoveBasketItem(string courseId)
        {
            var basket = await Get(); // Mevcut sepeti al

            if (basket == null)
            {
                return false; // Sepet yoksa false döndür
            }

            var deleteBasketItem = basket.BasketItems.FirstOrDefault(x => x.CourseId == courseId); // Kaldırılacak ürünü bul

            if (deleteBasketItem == null)
            {
                return false; // Ürün yoksa false döndür
            }

            var deleteResult = basket.BasketItems.Remove(deleteBasketItem); // Ürünü kaldır

            if (!deleteResult)
            {
                return false; // Kaldırma başarısızsa false döndür
            }

            if (!basket.BasketItems.Any())
            {
                basket.DiscountCode = null; // Sepet boşsa indirim kodunu sıfırla
            }

            return await SaveOrUpdate(basket); // Sepeti kaydet
        }

        // Sepeti kaydetme veya güncelleme metodu
        public async Task<bool> SaveOrUpdate(BasketViewModel basketViewModel)
        {
            var response = await _httpClient.PostAsJsonAsync<BasketViewModel>("baskets", basketViewModel); // Sepeti kaydet
            var responseContent = await response.Content.ReadAsStringAsync(); // Yanıt içeriğini oku

            return response.IsSuccessStatusCode; // Başarılı olup olmadığını döndür
        }
    }
}
