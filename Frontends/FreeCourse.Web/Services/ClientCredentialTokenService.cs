using FreeCourse.Web.Models; // Modeller
using FreeCourse.Web.Services.Interfaces; // Servis arayüzleri
using IdentityModel.AspNetCore.AccessTokenManagement; // Token yönetimi
using IdentityModel.Client; // IdentityModel istemci
using Microsoft.Extensions.Options; // Ayarları almak için
using System; // Temel sistem sınıfları
using System.Collections.Generic; // Koleksiyon sınıfları
using System.Linq; // LINQ işlevleri
using System.Net.Http; // HTTP istemcisi
using System.Threading.Tasks; // Asenkron programlama için

namespace FreeCourse.Web.Services
{
    // İstemci kimlik bilgileri ile token alma servisi
    public class ClientCredentialTokenService : IClientCredentialTokenService
    {
        private readonly ServiceApiSettings _serviceApiSettings; // Servis API ayarları
        private readonly ClientSettings _clientSettings; // İstemci ayarları
        private readonly IClientAccessTokenCache _clientAccessTokenCache; // Token önbelleği
        private readonly HttpClient _httpClient; // HTTP istemcisi

        // Yapıcı metot
        public ClientCredentialTokenService(IOptions<ServiceApiSettings> serviceApiSettings, IOptions<ClientSettings> clientSettings, IClientAccessTokenCache clientAccessTokenCache, HttpClient httpClient)
        {
            _serviceApiSettings = serviceApiSettings.Value; // Servis ayarlarını al
            _clientSettings = clientSettings.Value; // İstemci ayarlarını al
            _clientAccessTokenCache = clientAccessTokenCache; // Token önbelleğini al
            _httpClient = httpClient; // HTTP istemcisini al
        }

        // Token alma metodu
        public async Task<string> GetToken()
        {
            // Öncelikle mevcut token'ı al
            var currentToken = await _clientAccessTokenCache.GetAsync("WebClientToken", default);

            // Eğer mevcut bir token varsa, onu döndür
            if (currentToken != null)
            {
                return currentToken.AccessToken;
            }

            // Discovery belgesini al
            var disco = await _httpClient.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
            {
                Address = _serviceApiSettings.IdentityBaseUri, // Kimlik sağlayıcısının URI'si
                Policy = new DiscoveryPolicy { RequireHttps = false } // HTTPS zorunluluğu
            });

            // Discovery belgesinde bir hata varsa, hatayı fırlat
            if (disco.IsError)
            {
                throw disco.Exception;
            }

            // Yeni bir token istemek için kimlik bilgilerini ayarla
            var clientCredentialTokenRequest = new ClientCredentialsTokenRequest
            {
                ClientId = _clientSettings.WebClient.ClientId, // İstemci kimliği
                ClientSecret = _clientSettings.WebClient.ClientSecret, // İstemci gizli anahtarı
                Address = disco.TokenEndpoint // Token endpoint'i
            };

            // Token iste
            var newToken = await _httpClient.RequestClientCredentialsTokenAsync(clientCredentialTokenRequest);

            // Token isteği başarısızsa, hatayı fırlat
            if (newToken.IsError)
            {
                throw newToken.Exception;
            }

            // Yeni token'ı önbelleğe kaydet
            await _clientAccessTokenCache.SetAsync("WebClientToken", newToken.AccessToken, newToken.ExpiresIn, default);

            return newToken.AccessToken; // Yeni token'ı döndür
        }
    }
}
