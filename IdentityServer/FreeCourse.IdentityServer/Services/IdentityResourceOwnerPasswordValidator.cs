using FreeCourse.IdentityServer.Models; // ApplicationUser modelini kullanmak için
using IdentityModel; // IdentityModel kütüphanesini kullanmak için
using IdentityServer4.Validation; // IdentityServer4 için doğrulama işlemleri
using Microsoft.AspNetCore.Identity; // ASP.NET Core Identity için
using System.Collections.Generic; // Koleksiyonlar için
using System.Threading.Tasks; // Asenkron görevler için

namespace FreeCourse.IdentityServer.Services
{
    // IResourceOwnerPasswordValidator arayüzünü uygulayan sınıf
    public class IdentityResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
    {
        private readonly UserManager<ApplicationUser> _userManager; // UserManager, kullanıcı yönetimi için

        // Constructor, UserManager'ı alır
        public IdentityResourceOwnerPasswordValidator(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager; // UserManager'ı atar
        }

        // Kullanıcı adı ve şifre doğrulama işlemi
        public async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            // Kullanıcıyı e-posta ile bulur
            var existUser = await _userManager.FindByEmailAsync(context.UserName);

            // Kullanıcı bulunamazsa hata mesajı oluştur
            if (existUser == null)
            {
                var errors = new Dictionary<string, object>();
                errors.Add("errors", new List<string> { "Email ya da şifreniz yanlış." }); // Hata mesajı
                context.Result.CustomResponse = errors; // Hata yanıtını ayarlar
                return; // İşlemi sonlandırır
            }

            // Şifreyi kontrol et
            var passwordCheck = await _userManager.CheckPasswordAsync(existUser, context.Password);
            // Şifre yanlışsa hata mesajı oluştur
            if (passwordCheck == false)
            {
                var errors = new Dictionary<string, object>();
                errors.Add("errors", new List<string> { "Email ya da şifreniz yanlış." }); // Hata mesajı
                context.Result.CustomResponse = errors; // Hata yanıtını ayarlar
                return; // İşlemi sonlandırır
            }

            // Doğrulama başarılıysa GrantValidationResult ile kullanıcıyı doğrula
            context.Result = new GrantValidationResult(existUser.Id.ToString(), OidcConstants.AuthenticationMethods.Password);
        }
    }
}
