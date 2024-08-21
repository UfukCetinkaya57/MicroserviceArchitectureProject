using IdentityServer4.Validation; // IdentityServer4 için doğrulama işlemleri
using System; // Temel sistem sınıfları
using System.Collections.Generic; // Koleksiyonlar için
using System.Linq; // LINQ sorguları için
using System.Threading.Tasks; // Asenkron görevler için

namespace FreeCourse.IdentityServer.Services
{
    // IExtensionGrantValidator arayüzünü uygulayan sınıf
    public class TokenExchangeExtensionGrantValidator : IExtensionGrantValidator
    {
        // GrantType özelliği, token exchange için tanımlanmıştır
        public string GrantType => "urn:ietf:params:oauth:grant-type:token-exchange";

        private readonly ITokenValidator _tokenValidator; // Token doğrulama arayüzü

        // Constructor, ITokenValidator'ı alır
        public TokenExchangeExtensionGrantValidator(ITokenValidator tokenValidator)
        {
            _tokenValidator = tokenValidator; // Token doğrulayıcıyı atar
        }

        // Token exchange işlemi doğrulama
        public async Task ValidateAsync(ExtensionGrantValidationContext context)
        {
            // Raw isteği alır
            var requestRaw = context.Request.Raw.ToString();

            // "subject_token" alanındaki token'ı alır
            var token = context.Request.Raw.Get("subject_token");

            // Token yoksa hata mesajı oluştur
            if (string.IsNullOrEmpty(token))
            {
                context.Result = new GrantValidationResult(IdentityServer4.Models.TokenRequestErrors.InvalidRequest, "token missing");
                return; // İşlemi sonlandırır
            }

            // Token'ı doğrula
            var tokenValidateResult = await _tokenValidator.ValidateAccessTokenAsync(token);

            // Token doğrulama hatası varsa hata mesajı oluştur
            if (tokenValidateResult.IsError)
            {
                context.Result = new GrantValidationResult(IdentityServer4.Models.TokenRequestErrors.InvalidGrant, "token invalid");
                return; // İşlemi sonlandırır
            }

            // Token'dan "sub" claim'ini al
            var subjectClaim = tokenValidateResult.Claims.FirstOrDefault(c => c.Type == "sub");

            // "sub" claim'i yoksa hata mesajı oluştur
            if (subjectClaim == null)
            {
                context.Result = new GrantValidationResult(IdentityServer4.Models.TokenRequestErrors.InvalidGrant, "token must contain sub value");
                return; // İşlemi sonlandırır
            }

            // Doğrulama başarılıysa GrantValidationResult ile sonucu ayarla
            context.Result = new GrantValidationResult(subjectClaim.Value, "access_token", tokenValidateResult.Claims);
            return; // İşlemi sonlandırır
        }
    }
}
