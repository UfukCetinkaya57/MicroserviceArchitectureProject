using FreeCourse.Web.Exceptions; // Özel istisna sınıfı
using FreeCourse.Web.Services.Interfaces; // Arayüz tanımları
using Microsoft.AspNetCore.Authentication; // Kimlik doğrulama ile ilgili sınıflar
using Microsoft.AspNetCore.Http; // HTTP bağlamı ile ilgili sınıflar
using Microsoft.Extensions.Logging; // Günlük kaydı ile ilgili sınıflar
using Microsoft.IdentityModel.Protocols.OpenIdConnect; // OpenID Connect ile ilgili sabitler
using System; // Temel sistem sınıfları
using System.Collections.Generic; // Koleksiyon sınıfları
using System.Linq; // LINQ işlevleri
using System.Net.Http; // HTTP istemcisi
using System.Threading; // İptal token'ları için
using System.Threading.Tasks; // Asenkron programlama için

namespace FreeCourse.Web.Handler
{
    // ResourceOwnerPasswordTokenHandler, yetkilendirilmiş HTTP isteklerini işlemek için kullanılan bir delegating handler'dır
    public class ResourceOwnerPasswordTokenHandler : DelegatingHandler
    {
        private readonly IHttpContextAccessor _httpContextAccessor; // HTTP bağlamı erişimi
        private readonly IIdentityService _identityService; // Kimlik hizmeti arayüzü
        private readonly ILogger<ResourceOwnerPasswordTokenHandler> _logger; // Günlük kaydı için logger

        // Yapıcı
        public ResourceOwnerPasswordTokenHandler(IHttpContextAccessor httpContextAccessor, IIdentityService identityService, ILogger<ResourceOwnerPasswordTokenHandler> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _identityService = identityService;
            _logger = logger;
        }

        // HTTP isteğini gönderir ve yanıtı alır
        protected async override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // Mevcut erişim token'ını alır
            var accessToken = await _httpContextAccessor.HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);

            // İstek başlığına Bearer token ekler
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            // İsteği gönderir
            var response = await base.SendAsync(request, cancellationToken);

            // Yanıt yetkisiz ise, yeni token almak için yeniden dener
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                var responseContent = await response.Content.ReadAsStringAsync(); // Yanıt içeriğini oku

                // Yenileme token'ı ile yeni erişim token'ı al
                var tokenResponse = await _identityService.GetAccessTokenByRefreshToken();

                if (tokenResponse != null)
                {
                    // Yeni token'ı isteğe ekle
                    request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", tokenResponse.AccessToken);

                    // İsteği tekrar gönder
                    response = await base.SendAsync(request, cancellationToken);
                }
            }

            // Eğer hala yetkisiz ise, özel bir istisna fırlat
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                throw new UnAuthorizeException(); // Yetkisiz erişim durumunda özel istisna fırlatır
            }

            return response; // Yanıtı döner
        }
    }
}
