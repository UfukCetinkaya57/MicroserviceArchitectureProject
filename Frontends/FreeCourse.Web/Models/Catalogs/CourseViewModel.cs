using FreeCourse.Web.Models.Catalogs; // İlgili model sınıflarını içe aktar
using System; // Temel sistem sınıfları
using System.Collections.Generic; // Koleksiyon sınıfları
using System.Linq; // LINQ işlevleri
using System.Threading.Tasks; // Asenkron programlama için

namespace FreeCourse.Web.Models.Catalogs
{
    // Kursu temsil eden model
    public class CourseViewModel
    {
        // Kursun benzersiz kimliği
        public string Id { get; set; }

        // Kursun adı
        public string Name { get; set; }

        // Kursun açıklaması
        public string Description { get; set; }

        // Kısa açıklama, açıklamanın ilk 100 karakterini alır
        public string ShortDescription
        {
            get => Description.Length > 100 ? Description.Substring(0, 100) + "..." : Description;
        }

        // Kursun fiyatı
        public decimal Price { get; set; }

        // Kursu oluşturan kullanıcının kimliği
        public string UserId { get; set; }

        // Kursun resmi
        public string Picture { get; set; }

        // Stokta bulunan resmin URL'si
        public string StockPictureUrl { get; set; }

        // Kursun oluşturulma tarihi
        public DateTime CreatedTime { get; set; }

        // Kursun özelliklerini tutan model
        public FeatureViewModel Feature { get; set; }

        // Kursun ait olduğu kategori kimliği
        public string CategoryId { get; set; }

        // Kursun ait olduğu kategori bilgisi
        public CategoryViewModel Category { get; set; }
    }
}
