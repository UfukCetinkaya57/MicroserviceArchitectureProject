using FreeCourse.Shared.Dtos; // Ortak DTO sınıfları
using FreeCourse.Web.Models.PhotoStock; // Fotoğraf modelleri
using FreeCourse.Web.Services.Interfaces; // Servis arayüzleri
using Microsoft.AspNetCore.Http; // HTTP dosya işlemleri
using System; // Temel sistem sınıfları
using System.Collections.Generic; // Koleksiyon sınıfları
using System.IO; // Dosya akış işlemleri
using System.Linq; // LINQ işlevleri
using System.Net.Http; // HTTP istemcisi
using System.Net.Http.Json; // JSON ile çalışmak için genişletmeler
using System.Threading.Tasks; // Asenkron programlama için

namespace FreeCourse.Web.Services
{
    // Fotoğraf hizmeti
    public class PhotoStockService : IPhotoStockService
    {
        // HTTP istemcisi
        private readonly HttpClient _httpClient;

        // Yapıcı metot
        public PhotoStockService(HttpClient httpClient)
        {
            _httpClient = httpClient; // HTTP istemcisini al
        }

        // Fotoğraf silme metodu
        public async Task<bool> DeletePhoto(string photoUrl)
        {
            // Belirtilen fotoğraf URL'sine göre HTTP DELETE isteği gönder
            var response = await _httpClient.DeleteAsync($"photos?photoUrl={photoUrl}");
            // İsteğin başarısını döndür
            return response.IsSuccessStatusCode; // Başarılı ise true, değilse false döner
        }

        // Fotoğraf yükleme metodu
        public async Task<PhotoViewModel> UploadPhoto(IFormFile photo)
        {
            // Fotoğrafın null veya boyutunun 0'dan küçük olup olmadığını kontrol et
            if (photo == null || photo.Length <= 0)
            {
                return null; // Geçersiz fotoğraf durumu
            }

            // Rastgele dosya ismi oluştur (örnek: 203802340234.jpg)
            var randomFilename = $"{Guid.NewGuid().ToString()}{Path.GetExtension(photo.FileName)}";

            using var ms = new MemoryStream(); // Bellekte yeni bir akış oluştur
            await photo.CopyToAsync(ms); // Fotoğrafı bellek akışına kopyala

            var multipartContent = new MultipartFormDataContent(); // Çok parçalı form verisi içeriği oluştur

            // Fotoğraf içeriğini ekle
            multipartContent.Add(new ByteArrayContent(ms.ToArray()), "photo", randomFilename);

            // HTTP POST isteği gönder
            var response = await _httpClient.PostAsync("photos", multipartContent);

            if (!response.IsSuccessStatusCode)
            {
                return null; // Eğer istek başarısızsa null döner
            }

            // Başarılı bir yanıt aldıysak, yanıt içeriğini oku
            var responseSuccess = await response.Content.ReadFromJsonAsync<Response<PhotoViewModel>>();

            return responseSuccess.Data; // Fotoğraf bilgilerini döndür
        }
    }
}
