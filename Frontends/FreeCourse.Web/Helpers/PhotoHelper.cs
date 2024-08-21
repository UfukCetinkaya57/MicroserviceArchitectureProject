using FreeCourse.Web.Models; // ServiceApiSettings modelini içe aktarır
using Microsoft.Extensions.Options; // Seçenekleri yönetmek için gerekli
using System; // Temel sistem sınıfları
using System.Collections.Generic; // Koleksiyon sınıfları
using System.Linq; // LINQ işlevleri
using System.Threading.Tasks; // Asenkron programlama için

namespace FreeCourse.Web.Helpers
{
    // Fotoğraf URL'lerini oluşturmak için yardımcı sınıf
    public class PhotoHelper
    {
        private readonly ServiceApiSettings _serviceApiSettings; // Servis ayarlarını saklamak için değişken

        // Yapıcı
        public PhotoHelper(IOptions<ServiceApiSettings> serviceApiSettings)
        {
            _serviceApiSettings = serviceApiSettings.Value; // ServiceApiSettings değerini alır
        }

        // Fotoğraf URL'sini oluşturur
        public string GetPhotoStockUrl(string photoUrl)
        {
            // Ayarların PhotoStockUri'sini ve verilen photoUrl'yi birleştirerek tam URL oluşturur
            return $"{_serviceApiSettings.PhotoStockUri}/photos/{photoUrl}";
        }
    }
}
