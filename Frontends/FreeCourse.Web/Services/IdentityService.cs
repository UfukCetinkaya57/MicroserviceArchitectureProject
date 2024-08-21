using FreeCourse.Shared.Dtos; // Ortak DTO'lar
using FreeCourse.Web.Models; // Uygulama modelleri
using FreeCourse.Web.Services.Interfaces; // Servis arayüzleri
using IdentityModel.Client; // IdentityModel kütüphanesi
using Microsoft.AspNetCore.Authentication; // ASP.NET Core kimlik doğrulama
using Microsoft.AspNetCore.Authentication.Cookies; // Cookie tabanlı kimlik doğrulama
using Microsoft.AspNetCore.Http; // HTTP bağlamı
using Microsoft.Extensions.Options; // Ayar yönetimi
using Microsoft.IdentityModel.Protocols.OpenIdConnect; // OpenID Connect protokolleri
using System; // Temel sistem sınıfları
using System.Collections.Generic; // Koleksiyon sınıfları
using System.Globalization; // Kültürel bilgileri yönetme
using System.Linq; // LINQ işlevleri
using System.Net.Http; // HTTP istemcisi
using System.Security.Claims; // İlgili iddialar
using System.Text.Json; // JSON serileştirme
using System.Threading.Tasks; // Asenkron programlama için

namespace FreeCourse.Web.Services
{
    // Kimlik hizmeti
    public class IdentityService : IIdentityService
    {
        // Mikroservisler için HTTP istemcisi
        private readonly HttpClient _httpClient;
        // Cookie erişimi için bağlam
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ClientSettings _clientSettings;
        private readonly ServiceApiSettings _serviceApiSettings;

        // Yapıcı metot
        public IdentityService(HttpClient client, IHttpContextAccessor httpContextAccessor, IOptions<ClientSettings> clientSettings, IOptions<ServiceApiSettings> serviceApiSettings)
        {
            _httpClient = client; // HTTP istemcisini al
            _httpContextAccessor = httpContextAccessor; // HTTP bağlamını al
            _clientSettings = clientSettings.Value; // İstemci ayarlarını al
            _serviceApiSettings = serviceApiSettings.Value; // Servis API ayarlarını al
        }

        // Refresh token ile erişim token'ı alma
        public async Task<TokenResponse> GetAccessTokenByRefreshToken()
        {
            // Keşif belgesini al
            var disco = await _httpClient.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
            {
                Address = _serviceApiSettings.IdentityBaseUri,
                Policy = new DiscoveryPolicy { RequireHttps = false }
            });

            if (disco.IsError)
            {
                throw disco.Exception; // Hata varsa, istisna fırlat
            }

            var refreshToken = await _httpContextAccessor.HttpContext.GetTokenAsync(OpenIdConnectParameterNames.RefreshToken); // Refresh token'ı al

            // Refresh token isteği oluştur
            RefreshTokenRequest refreshTokenRequest = new()
            {
                ClientId = _clientSettings.WebClientForUser.ClientId,
                ClientSecret = _clientSettings.WebClientForUser.ClientSecret,
                RefreshToken = refreshToken,
                Address = disco.TokenEndpoint
            };

            var token = await _httpClient.RequestRefreshTokenAsync(refreshTokenRequest); // Yeni token al

            if (token.IsError)
            {
                return null; // Hata varsa null döndür
            }

            // Kimlik doğrulama token'larını depola
            var authenticationTokens = new List<AuthenticationToken>()
            {
                new AuthenticationToken{ Name=OpenIdConnectParameterNames.AccessToken,Value=token.AccessToken},
                new AuthenticationToken{ Name=OpenIdConnectParameterNames.RefreshToken,Value=token.RefreshToken},
                new AuthenticationToken{ Name=OpenIdConnectParameterNames.ExpiresIn,Value= DateTime.Now.AddSeconds(token.ExpiresIn).ToString("o",CultureInfo.InvariantCulture)}
            };

            var authenticationResult = await _httpContextAccessor.HttpContext.AuthenticateAsync(); // Mevcut kimlik doğrulama sonucunu al

            var properties = authenticationResult.Properties; // Özellikleri al
            properties.StoreTokens(authenticationTokens); // Token'ları depola

            // Kullanıcıyı yeniden oturum açtır
            await _httpContextAccessor.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, authenticationResult.Principal, properties);

            return token; // Yeni token'ı döndür
        }

        // Refresh token'ı iptal et
        public async Task RevokeRefreshToken()
        {
            var disco = await _httpClient.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
            {
                Address = _serviceApiSettings.IdentityBaseUri,
                Policy = new DiscoveryPolicy { RequireHttps = false }
            });

            if (disco.IsError)
            {
                throw disco.Exception; // Hata varsa, istisna fırlat
            }

            var refreshToken = await _httpContextAccessor.HttpContext.GetTokenAsync(OpenIdConnectParameterNames.RefreshToken); // Refresh token'ı al

            // Token iptal isteği oluştur
            TokenRevocationRequest tokenRevocationRequest = new()
            {
                ClientId = _clientSettings.WebClientForUser.ClientId,
                ClientSecret = _clientSettings.WebClientForUser.ClientSecret,
                Address = disco.RevocationEndpoint,
                Token = refreshToken,
                TokenTypeHint = "refresh_token"
            };

            await _httpClient.RevokeTokenAsync(tokenRevocationRequest); // Token'ı iptal et
        }

        // Kullanıcı girişi
        public async Task<Response<bool>> SignIn(SigninInput signinInput)
        {
            // Token endpoint'leri için keşif belgesi al
            var disco = await _httpClient.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
            {
                Address = _serviceApiSettings.IdentityBaseUri,
                Policy = new DiscoveryPolicy { RequireHttps = false }
            });

            if (disco.IsError)
            {
                throw disco.Exception; // Hata varsa, istisna fırlat
            }

            // Kullanıcı adı ve şifre ile token isteği oluştur
            var passwordTokenRequest = new PasswordTokenRequest
            {
                ClientId = _clientSettings.WebClientForUser.ClientId,
                ClientSecret = _clientSettings.WebClientForUser.ClientSecret,
                UserName = signinInput.Email,
                Password = signinInput.Password,
                Address = disco.TokenEndpoint
            };

            var token = await _httpClient.RequestPasswordTokenAsync(passwordTokenRequest); // Token isteği gönder

            if (token.IsError)
            {
                var responseContent = await token.HttpResponse.Content.ReadAsStringAsync(); // Hata içeriğini oku

                var errorDto = JsonSerializer.Deserialize<ErrorDto>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                return Response<bool>.Fail(errorDto.Errors, 400); // Hata varsa yanıt döndür
            }

            // Kullanıcı bilgilerini al
            var userInfoRequest = new UserInfoRequest
            {
                Token = token.AccessToken,
                Address = disco.UserInfoEndpoint
            };

            var userInfo = await _httpClient.GetUserInfoAsync(userInfoRequest); // Kullanıcı bilgilerini al

            if (userInfo.IsError)
            {
                throw userInfo.Exception; // Hata varsa, istisna fırlat
            }

            // İddiaları oluştur
            ClaimsIdentity claimsIdentity = new ClaimsIdentity(userInfo.Claims, CookieAuthenticationDefaults.AuthenticationScheme, "name", "role");
            ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            var authenticationProperties = new AuthenticationProperties();

            // Token'ları depola
            authenticationProperties.StoreTokens(new List<AuthenticationToken>()
            {
                new AuthenticationToken{ Name=OpenIdConnectParameterNames.AccessToken,Value=token.AccessToken},
                new AuthenticationToken{ Name=OpenIdConnectParameterNames.RefreshToken,Value=token.RefreshToken},
                new AuthenticationToken{ Name=OpenIdConnectParameterNames.ExpiresIn,Value= DateTime.Now.AddSeconds(token.ExpiresIn).ToString("o",CultureInfo.InvariantCulture)}
            });

            authenticationProperties.IsPersistent = signinInput.IsRemember; // Kullanıcıyı hatırlama özelliği

            // Kullanıcıyı oturum açtır
            await _httpContextAccessor.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal, authenticationProperties);

            return Response<bool>.Success(200); // Başarılı yanıt döndür
        }
    }
}
