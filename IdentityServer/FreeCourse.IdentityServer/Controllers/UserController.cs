using FreeCourse.IdentityServer.Dtos; // DTO sınıflarını kullanmak için
using FreeCourse.IdentityServer.Models; // Uygulama kullanıcı modelini kullanmak için
using FreeCourse.Shared.Dtos; // Ortak DTO sınıflarını kullanmak için
using Microsoft.AspNet.Identity; // ASP.NET Identity için
using Microsoft.AspNetCore.Authorization; // Yetkilendirme için
using Microsoft.AspNetCore.Identity; // Identity sistemini kullanmak için
using Microsoft.AspNetCore.Mvc; // MVC yapısını kullanmak için
using System.IdentityModel.Tokens.Jwt; // JWT ile ilgili işlemler için
using System.Linq; // LINQ kullanmak için
using System.Threading.Tasks; // Asenkron programlama için
using static IdentityServer4.IdentityServerConstants; // IdentityServer sabitlerini kullanmak için

namespace FreeCourse.IdentityServer.Controllers
{
    // Local API politikası ile yetkilendirilmiş
    [Authorize(LocalApi.PolicyName)]
    // API yönlendirmelerini ayarlamak için
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        // UserManager<ApplicationUser> bağımlılığı, kullanıcı işlemleri için
        private readonly Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> _userManager;

        // Constructor, UserManager'ı alır ve ayarlar
        public UserController(Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        // Kullanıcı kaydı işlemi
        [HttpPost]
        public async Task<IActionResult> SignUp(SignupDto signupDto)
        {
            // Yeni bir ApplicationUser nesnesi oluştur
            var user = new ApplicationUser
            {
                UserName = signupDto.UserName, // Kullanıcı adını ata
                Email = signupDto.Email, // E-posta adresini ata
                City = signupDto.City // Şehir bilgisini ata
            };

            // Kullanıcıyı oluştur
            var result = await _userManager.CreateAsync(user, signupDto.Password);

            // Eğer kullanıcı oluşturulamadıysa hata döner
            if (!result.Succeeded)
            {
                return BadRequest(Response<NoContent>.Fail(result.Errors.Select(x => x.Description).ToList(), 400));
            }

            // Başarılıysa NoContent döner
            return NoContent();
        }

        // Kullanıcı bilgilerini alma işlemi
        [HttpGet]
        public async Task<IActionResult> GetUser()
        {
            // JWT token'ından kullanıcı ID'sini al
            var useridClaim = User.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Sub);
            if (useridClaim == null) return BadRequest();

            // Kullanıcıyı bul
            var user = await _userManager.FindByIdAsync(useridClaim.Value);
            if (user == null) return BadRequest();

            // Kullanıcı bilgilerini döner
            return Ok(new
            {
                Id = user.Id, // Kullanıcı ID'si
                UserName = user.UserName, // Kullanıcı adı
                Email = user.Email, // E-posta
                City = user.City // Şehir
            });
        }
    }
}
