using FreeCourse.Web.Models.Baskets; // Sepet model sınıflarını içe aktarma
using FreeCourse.Web.Models.Discounts; // İndirim model sınıflarını içe aktarma
using FreeCourse.Web.Services.Interfaces; // Servis arayüzlerini içe aktarma
using Microsoft.AspNetCore.Authorization; // Yetkilendirme için gerekli sınıflar
using Microsoft.AspNetCore.Mvc; // MVC yapısı için gerekli sınıflar
using System; // Temel sistem sınıfları
using System.Collections.Generic; // Koleksiyon sınıfları
using System.Linq; // LINQ işlevleri
using System.Threading.Tasks; // Asenkron programlama için

namespace FreeCourse.Web.Controllers
{
    [Authorize] // Bu denetleyiciyi sadece yetkilendirilmiş kullanıcıların erişimine kapat
    public class BasketController : Controller
    {
        private readonly ICatalogService _catalogService; // Kurs katalog servisi
        private readonly IBasketService _basketService; // Sepet servisi

        public BasketController(ICatalogService catalogService, IBasketService basketService)
        {
            _catalogService = catalogService; // Bağımlılığı atama
            _basketService = basketService; // Bağımlılığı atama
        }

        // Sepet sayfasını gösterir
        public async Task<IActionResult> Index()
        {
            var basket = await _basketService.Get(); // Sepetteki ürünleri al
            return View(basket); // Sepet verisini görüntüle
        }

        // Sepete kurs ekler
        public async Task<IActionResult> AddBasketItem(string courseId)
        {
            var course = await _catalogService.GetByCourseId(courseId); // Kursu al

            // Sepet ürünü oluştur
            var basketItem = new BasketItemViewModel
            {
                CourseId = course.Id,
                CourseName = course.Name,
                Price = course.Price
            };

            await _basketService.AddBasketItem(basketItem); // Sepet ürününü ekle

            return RedirectToAction(nameof(Index)); // Sepet sayfasına yönlendir
        }

        // Sepetten kursu çıkarır
        public async Task<IActionResult> RemoveBasketItem(string courseId)
        {
            var result = await _basketService.RemoveBasketItem(courseId); // Sepet ürününü çıkar

            return RedirectToAction(nameof(Index)); // Sepet sayfasına yönlendir
        }

        // İndirim uygular
        public async Task<IActionResult> ApplyDiscount(DiscountApplyInput discountApplyInput)
        {
            if (!ModelState.IsValid) // Model doğrulaması
            {
                // Hata mesajını geçici veriye ekle
                TempData["discountError"] = ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage).First();
                return RedirectToAction(nameof(Index)); // Sepet sayfasına yönlendir
            }

            var discountStatus = await _basketService.ApplyDiscount(discountApplyInput.Code); // İndirim uygula

            // İndirim durumunu geçici veriye ekle
            TempData["discountStatus"] = discountStatus;
            return RedirectToAction(nameof(Index)); // Sepet sayfasına yönlendir
        }

        // Uygulanan indirimi iptal eder
        public async Task<IActionResult> CancelApplyDiscount()
        {
            await _basketService.CancelApplyDiscount(); // İndirim iptali
            return RedirectToAction(nameof(Index)); // Sepet sayfasına yönlendir
        }
    }
}
