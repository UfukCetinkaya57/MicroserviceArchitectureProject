using FreeCourse.Shared.Dtos; // Paylaşılan DTO sınıfları
using FreeCourse.Web.Helpers; // Yardımcı sınıflar
using FreeCourse.Web.Models; // Model sınıfları
using FreeCourse.Web.Models.Catalogs; // Katalog model sınıfları
using FreeCourse.Web.Services.Interfaces; // Servis arayüzleri
using System; // Temel sistem sınıfları
using System.Collections.Generic; // Koleksiyon sınıfları
using System.Linq; // LINQ işlevleri
using System.Net.Http; // HTTP istemcisi
using System.Net.Http.Json; // JSON ile HTTP istekleri
using System.Threading.Tasks; // Asenkron programlama için

namespace FreeCourse.Web.Services
{
    // Katalog servis sınıfı
    public class CatalogService : ICatalogService
    {
        private readonly HttpClient _client; // HTTP istemcisi
        private readonly IPhotoStockService _photoStockService; // Fotoğraf servisi
        private readonly PhotoHelper _photoHelper; // Fotoğraf yardımcı sınıfı

        // Yapıcı metot
        public CatalogService(HttpClient client, IPhotoStockService photoStockService, PhotoHelper photoHelper)
        {
            _client = client; // HTTP istemcisi
            _photoStockService = photoStockService; // Fotoğraf servisi
            _photoHelper = photoHelper; // Fotoğraf yardımcı sınıfı
        }

        // Kurs oluşturma metodu
        public async Task<bool> CreateCourseAsync(CourseCreateInput courseCreateInput)
        {
            // Fotoğraf yükle
            var resultPhotoService = await _photoStockService.UploadPhoto(courseCreateInput.PhotoFormFile);

            // Fotoğrafın URL'sini ayarla
            if (resultPhotoService != null)
            {
                courseCreateInput.Picture = resultPhotoService.Url;
            }

            // Kursu kaydet
            var response = await _client.PostAsJsonAsync<CourseCreateInput>("courses", courseCreateInput);

            return response.IsSuccessStatusCode; // Başarılı olup olmadığını döndür
        }

        // Kurs silme metodu
        public async Task<bool> DeleteCourseAsync(string courseId)
        {
            var response = await _client.DeleteAsync($"courses/{courseId}");

            return response.IsSuccessStatusCode; // Başarılı olup olmadığını döndür
        }

        // Tüm kategorileri alma metodu
        public async Task<List<CategoryViewModel>> GetAllCategoryAsync()
        {
            var response = await _client.GetAsync("categories");

            if (!response.IsSuccessStatusCode)
            {
                return null; // Başarısızsa null döndür
            }

            var responseSuccess = await response.Content.ReadFromJsonAsync<Response<List<CategoryViewModel>>>();

            return responseSuccess.Data; // Kategorileri döndür
        }

        // Tüm kursları alma metodu
        public async Task<List<CourseViewModel>> GetAllCourseAsync()
        {
            var response = await _client.GetAsync("courses");

            if (!response.IsSuccessStatusCode)
            {
                return null; // Başarısızsa null döndür
            }

            var responseSuccess = await response.Content.ReadFromJsonAsync<Response<List<CourseViewModel>>>();
            responseSuccess.Data.ForEach(x =>
            {
                x.StockPictureUrl = _photoHelper.GetPhotoStockUrl(x.Picture); // Fotoğraf URL'sini ayarla
            });

            return responseSuccess.Data; // Kursları döndür
        }

        // Kullanıcıya ait tüm kursları alma metodu
        public async Task<List<CourseViewModel>> GetAllCourseByUserIdAsync(string userId)
        {
            var response = await _client.GetAsync($"courses/GetAllByUserId/{userId}");

            if (!response.IsSuccessStatusCode)
            {
                return null; // Başarısızsa null döndür
            }

            var responseSuccess = await response.Content.ReadFromJsonAsync<Response<List<CourseViewModel>>>();
            responseSuccess.Data.ForEach(x =>
            {
                x.StockPictureUrl = _photoHelper.GetPhotoStockUrl(x.Picture); // Fotoğraf URL'sini ayarla
            });

            return responseSuccess.Data; // Kursları döndür
        }

        // Belirli bir kursu alma metodu
        public async Task<CourseViewModel> GetByCourseId(string courseId)
        {
            var response = await _client.GetAsync($"courses/{courseId}");

            if (!response.IsSuccessStatusCode)
            {
                return null; // Başarısızsa null döndür
            }

            var responseSuccess = await response.Content.ReadFromJsonAsync<Response<CourseViewModel>>();
            responseSuccess.Data.StockPictureUrl = _photoHelper.GetPhotoStockUrl(responseSuccess.Data.Picture); // Fotoğraf URL'sini ayarla

            return responseSuccess.Data; // Kursu döndür
        }

        // Kurs güncelleme metodu
        public async Task<bool> UpdateCourseAsync(CourseUpdateInput courseUpdateInput)
        {
            // Fotoğraf yükle
            var resultPhotoService = await _photoStockService.UploadPhoto(courseUpdateInput.PhotoFormFile);

            // Eski fotoğrafı sil ve yeni fotoğrafı ayarla
            if (resultPhotoService != null)
            {
                await _photoStockService.DeletePhoto(courseUpdateInput.Picture);
                courseUpdateInput.Picture = resultPhotoService.Url;
            }

            // Kursu güncelle
            var response = await _client.PutAsJsonAsync<CourseUpdateInput>("courses", courseUpdateInput);

            return response.IsSuccessStatusCode; // Başarılı olup olmadığını döndür
        }
    }
}
