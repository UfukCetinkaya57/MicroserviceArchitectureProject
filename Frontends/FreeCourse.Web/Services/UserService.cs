using FreeCourse.Web.Models; // Kullanıcı modeli sınıfları
using FreeCourse.Web.Services.Interfaces; // Servis arayüzleri
using System; // Temel sistem sınıfları
using System.Collections.Generic; // Koleksiyon sınıfları
using System.Linq; // LINQ işlevleri
using System.Net.Http; // HTTP istemcisi
using System.Net.Http.Json; // JSON ile çalışmak için genişletmeler
using System.Threading.Tasks; // Asenkron programlama için

namespace FreeCourse.Web.Services
{
    // Kullanıcı hizmeti
    public class UserService : IUserService
    {
        // HTTP istemcisi
        private readonly HttpClient _client;

        // Yapıcı metot
        public UserService(HttpClient client)
        {
            _client = client; // HTTP istemcisini al
        }

        // Kullanıcı bilgilerini getiren metot
        public async Task<UserViewModel> GetUser()
        {
            // API'den kullanıcı bilgilerini al ve UserViewModel döndür
            return await _client.GetFromJsonAsync<UserViewModel>("/api/user/getuser");
        }
    }
}
