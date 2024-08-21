using FreeCourse.Web.Exceptions; // Özel istisna sınıfı
using FreeCourse.Web.Services.Interfaces; // Arayüz tanımları
using System; // Temel sistem sınıfları
using System.Collections.Generic; // Koleksiyon sınıfları
using System.Linq; // LINQ işlevleri
using System.Net.Http; // HTTP istemcisi
using System.Net.Http.Headers; // HTTP başlıkları
using System.Threading; // İptal token'ları için
using System.Threading.Tasks; // Asenkron programlama için

namespace FreeCourse.Web.Handler
{
    // ClientCredentialTokenHandler, HTTP isteklerine token ekleyen bir delegating handler'dır
    public class ClientCredentialTokenHandler : DelegatingHandler
    {
        private readonly IClientCredentialTokenService _clientCredentialTokenService; // Token hizmeti arayüzü

        // Yapıcı
        public ClientCredentialTokenHandler(IClientCredentialTokenService clientCredentialTokenService)
        {
            _clientCredentialTokenService = clientCredentialTokenService;
        }

        // HTTP isteğini gönderir ve yanıtı alır
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // Authorization başlığına Bearer token ekler
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", await _clientCredentialTokenService.GetToken());

            // İsteği gönderir
            var response = await base.SendAsync(request, cancellationToken);

            // Yanıtın yetkisiz olup olmadığını kontrol eder
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                throw new UnAuthorizeException(); // Yetkisiz erişim durumunda özel istisna fırlatır
            }

            return response; // Yanıtı döner
        }
    }
}
