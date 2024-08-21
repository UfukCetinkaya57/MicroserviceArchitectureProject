using FreeCourse.Web.Models; // Model sınıflarını içe aktarma
using FreeCourse.Web.Services; // Servis sınıflarını içe aktarma
using FreeCourse.Web.Services.Interfaces; // Servis arayüzlerini içe aktarma
using Microsoft.AspNetCore.Authentication; // Kimlik doğrulama için gerekli sınıflar
using Microsoft.AspNetCore.Authentication.Cookies; // Cookie tabanlı kimlik doğrulama
using Microsoft.AspNetCore.Mvc; // MVC yapısı için gerekli sınıflar
using System; // Temel sistem sınıfları
using System.Collections.Generic; // Koleksiyon sınıfları
using System.Linq; // LINQ işlevleri
using System.Threading.Tasks; // Asenkron programlama için

namespace FreeCourse.Web.Controllers
{
    public class AuthController : Controller
    {
        private readonly IIdentityService _identityService; // Kimlik servisi için bağımlılık

        public AuthController(IIdentityService identityService)
        {
            _identityService = identityService; // Bağımlılığı atama
        }

        // Kullanıcı giriş sayfasını döndürür
        public IActionResult SignIn()
        {
            return View();
        }

        // Giriş işlemi için POST isteği
        [HttpPost]
        public async Task<IActionResult> SignIn(SigninInput signinInput)
        {
            if (!ModelState.IsValid) // Model doğrulaması
            {
                return View(); // Geçersizse tekrar giriş sayfasını göster
            }

            var response = await _identityService.SignIn(signinInput); // Giriş işlemi

            if (!response.IsSuccessful) // Giriş başarısızsa
            {
                response.Errors.ForEach(x =>
                {
                    ModelState.AddModelError(String.Empty, x); // Hata mesajlarını modele ekle
                });

                return View(); // Giriş sayfasını tekrar göster
            }

            return RedirectToAction(nameof(Index), "Home"); // Başarılı girişte ana sayfaya yönlendir
        }

        // Çıkış işlemi
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme); // Kullanıcıyı çıkış yap
            await _identityService.RevokeRefreshToken(); // Refresh token'ı iptal et
            return RedirectToAction(nameof(HomeController.Index), "Home"); // Ana sayfaya yönlendir
        }
    }
}
