using FreeCourse.Shared.Services; // Ortak servisleri içe aktarma
using FreeCourse.Web.Models.Catalogs; // Katalog model sınıflarını içe aktarma
using FreeCourse.Web.Services.Interfaces; // Servis arayüzlerini içe aktarma
using Microsoft.AspNetCore.Authorization; // Yetkilendirme için gerekli sınıflar
using Microsoft.AspNetCore.Mvc; // MVC yapısı için gerekli sınıflar
using Microsoft.AspNetCore.Mvc.Rendering; // Seçim listeleri için gerekli sınıflar
using System; // Temel sistem sınıfları
using System.Collections.Generic; // Koleksiyon sınıfları
using System.Linq; // LINQ işlevleri
using System.Threading.Tasks; // Asenkron programlama için

namespace FreeCourse.Web.Controllers
{
    [Authorize] // Bu denetleyiciye sadece yetkilendirilmiş kullanıcıların erişmesine izin ver
    public class CoursesController : Controller
    {
        private readonly ICatalogService _catalogService; // Katalog servisi
        private readonly ISharedIdentityService _sharedIdentityService; // Ortak kimlik servisi

        public CoursesController(ICatalogService catalogService, ISharedIdentityService sharedIdentityService)
        {
            _catalogService = catalogService; // Bağımlılığı atama
            _sharedIdentityService = sharedIdentityService; // Bağımlılığı atama
        }

        // Kullanıcının kurslarını gösterir
        public async Task<IActionResult> Index()
        {
            var userId = _sharedIdentityService.GetUserId; // Kullanıcı kimliğini al
            var courses = await _catalogService.GetAllCourseByUserIdAsync(userId); // Kullanıcıya ait tüm kursları al
            return View(courses); // Kurs listesini görüntüle
        }

        // Yeni kurs oluşturma sayfasını gösterir
        public async Task<IActionResult> Create()
        {
            var categories = await _catalogService.GetAllCategoryAsync(); // Tüm kategorileri al
            ViewBag.categoryList = new SelectList(categories, "Id", "Name"); // Kategorileri seçim listesi olarak ayarla

            return View(); // Kurs oluşturma görünümünü döndür
        }

        [HttpPost]
        public async Task<IActionResult> Create(CourseCreateInput courseCreateInput)
        {
            var categories = await _catalogService.GetAllCategoryAsync(); // Tüm kategorileri al
            ViewBag.categoryList = new SelectList(categories, "Id", "Name"); // Kategorileri seçim listesi olarak ayarla
            courseCreateInput.UserId = _sharedIdentityService.GetUserId; // Kullanıcı kimliğini ekle

            if (!ModelState.IsValid) // Model doğrulaması
            {
                return View(); // Hatalı ise görünümü döndür
            }

            await _catalogService.CreateCourseAsync(courseCreateInput); // Kursu oluştur

            return RedirectToAction(nameof(Index)); // Kurslar sayfasına yönlendir
        }

        // Kurs güncelleme sayfasını gösterir
        public async Task<IActionResult> Update(string id)
        {
            var course = await _catalogService.GetByCourseId(id); // Kursu al
            var categories = await _catalogService.GetAllCategoryAsync(); // Tüm kategorileri al

            if (course == null) // Eğer kurs bulunamazsa
            {
                // Mesaj göster (bu kısım eksik, bir mesaj mekanizması eklenebilir)
                return RedirectToAction(nameof(Index)); // Kurslar sayfasına yönlendir
            }

            ViewBag.categoryList = new SelectList(categories, "Id", "Name", course.CategoryId); // Seçim listesi oluştur
            var courseUpdateInput = new CourseUpdateInput // Güncelleme için gerekli bilgileri ayarla
            {
                Id = course.Id,
                Name = course.Name,
                Description = course.Description,
                Price = course.Price,
                Feature = course.Feature,
                CategoryId = course.CategoryId,
                UserId = course.UserId,
                Picture = course.Picture
            };

            return View(courseUpdateInput); // Güncelleme görünümünü döndür
        }

        [HttpPost]
        public async Task<IActionResult> Update(CourseUpdateInput courseUpdateInput)
        {
            var categories = await _catalogService.GetAllCategoryAsync(); // Tüm kategorileri al
            ViewBag.categoryList = new SelectList(categories, "Id", "Name", courseUpdateInput.CategoryId); // Seçim listesi oluştur
            if (!ModelState.IsValid) // Model doğrulaması
            {
                return View(); // Hatalı ise görünümü döndür
            }
            await _catalogService.UpdateCourseAsync(courseUpdateInput); // Kursu güncelle

            return RedirectToAction(nameof(Index)); // Kurslar sayfasına yönlendir
        }

        // Kursu siler
        public async Task<IActionResult> Delete(string id)
        {
            await _catalogService.DeleteCourseAsync(id); // Kursu sil

            return RedirectToAction(nameof(Index)); // Kurslar sayfasına yönlendir
        }
    }
}
