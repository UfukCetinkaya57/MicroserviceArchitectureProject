using FreeCourse.Web.Exceptions; // Özel istisna sınıflarını içe aktarma
using FreeCourse.Web.Models; // Modelleri içe aktarma
using FreeCourse.Web.Services.Interfaces; // Servis arayüzlerini içe aktarma
using Microsoft.AspNetCore.Diagnostics; // Hata işleme için gerekli sınıflar
using Microsoft.AspNetCore.Mvc; // MVC yapısı için gerekli sınıflar
using Microsoft.Extensions.Logging; // Loglama için gerekli sınıflar
using System; // Temel sistem sınıfları
using System.Collections.Generic; // Koleksiyon sınıfları
using System.Diagnostics; // Performans izleme için gerekli sınıflar
using System.Linq; // LINQ işlevleri
using System.Threading.Tasks; // Asenkron programlama için

namespace FreeCourse.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger; // Loglama için ILogger

        private readonly ICatalogService _catalogService; // Katalog servisi

        public HomeController(ILogger<HomeController> logger, ICatalogService catalogService)
        {
            _logger = logger; // Loglama servisini atama
            _catalogService = catalogService; // Katalog servisini atama
        }

        // Ana sayfa, tüm kursları görüntüler
        public async Task<IActionResult> Index()
        {
            var courses = await _catalogService.GetAllCourseAsync(); // Tüm kursları al
            return View(courses); // Kurs listesini görüntüle
        }

        // Belirtilen ID'ye göre kurs detayını görüntüler
        public async Task<IActionResult> Detail(string id)
        {
            var course = await _catalogService.GetByCourseId(id); // Kursu al
            return View(course); // Kurs detayını görüntüle
        }

        // Hata yönetimi için kullanılan işlem
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            var errorFeature = HttpContext.Features.Get<IExceptionHandlerFeature>(); // Hata özelliğini al

            // Eğer hata özel bir istisna ise, kullanıcıyı oturumu kapatma sayfasına yönlendir
            if (errorFeature != null && errorFeature.Error is UnAuthorizeException)
            {
                return RedirectToAction(nameof(AuthController.Logout), "Auth"); // Oturumu kapat
            }

            // Diğer hatalar için hata görünümünü döndür
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier }); // Hata görünümünü döndür
        }
    }
}
