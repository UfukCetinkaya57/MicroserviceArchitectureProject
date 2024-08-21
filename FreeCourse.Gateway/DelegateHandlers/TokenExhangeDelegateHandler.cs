using IdentityModel.Client;
using Microsoft.Extensions.Configuration;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace FreeCourse.Gateway.DelegateHandlers
{
    public class TokenExhangeDelegateHandler : DelegatingHandler
    {
        private readonly HttpClient _httpClient; // HttpClient örneği
        private readonly IConfiguration _configuration; // Uygulama yapılandırma ayarları

        private string _accessToken; // Elde edilen erişim token'ı

        public TokenExhangeDelegateHandler(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient; // HttpClient örneğini al
            _configuration = configuration; // Yapılandırmayı al
        }

        // Erişim token'ını almak için asenkron yöntem
        private async Task<string> GetToken(string requestToken)
        {
            // Eğer daha önce erişim token'ı alındıysa, onu döndür
            if (!string.IsNullOrEmpty(_accessToken))
            {
                return _accessToken;
            }

            // Discovery document'ı al
            var disco = await _httpClient.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
            {
                Address = _configuration["IdentityServerURL"], // IdentityServer URL'si
                Policy = new DiscoveryPolicy { RequireHttps = false } // HTTPS gereksinimini devre dışı bırak
            });

            // Eğer discovery işlemi hatalıysa, hata fırlat
            if (disco.IsError)
            {
                throw disco.Exception;
            }

            // Token exchange isteğini oluştur
            TokenExchangeTokenRequest tokenExchangeTokenRequest = new TokenExchangeTokenRequest()
            {
                Address = disco.TokenEndpoint, // Token endpoint adresi
                ClientId = _configuration["ClientId"], // Client ID
                ClientSecret = _configuration["ClientSecret"], // Client Secret
                GrantType = _configuration["TokenGrantType"], // Grant type
                SubjectToken = requestToken, // İstemci tarafından sağlanan token
                SubjectTokenType = "urn:ietf:params:oauth:token-type:access-token", // Token türü
                Scope = "openid discount_fullpermission payment_fullpermission" // İzinler
            };

            // Token exchange işlemini gerçekleştir
            var tokenResponse = await _httpClient.RequestTokenExchangeTokenAsync(tokenExchangeTokenRequest);

            // Eğer token isteği hatalıysa, hata fırlat
            if (tokenResponse.IsError)
            {
                throw tokenResponse.Exception;
            }

            // Elde edilen erişim token'ını sakla
            _accessToken = tokenResponse.AccessToken;

            return _accessToken; // Elde edilen token'ı döndür
        }

        // HTTP isteğini gönderirken token'ı ayarlamak için override edilen yöntem
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // İstek başlığından token'ı al
            var requestToken = request.Headers.Authorization.Parameter;

            // Yeni erişim token'ını al
            var newToken = await GetToken(requestToken);

            // İstek başlığına Bearer token'ı ekle
            request.SetBearerToken(newToken);

            // İsteği gönder
            return await base.SendAsync(request, cancellationToken);
        }
    }
}
