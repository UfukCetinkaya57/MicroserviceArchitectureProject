using System; // Temel sistem sınıfları
using System.Collections.Generic; // Koleksiyon sınıfları
using System.Linq; // LINQ işlevleri
using System.Threading.Tasks; // Asenkron programlama için

namespace FreeCourse.Web.Models.Baskets
{
    // Alışveriş sepetindeki bir kurs öğesini temsil eden model
    public class BasketItemViewModel
    {
        // Sepetteki ürün miktarı
        public int Quantity { get; set; } = 1; // Varsayılan değer 1

        // Kursun kimliği
        public string CourseId { get; set; }

        // Kursun adı
        public string CourseName { get; set; }

        // Kursun fiyatı
        public decimal Price { get; set; }

        // Uygulanan indirimli fiyat
        private decimal? DiscountAppliedPrice;

        // Geçerli fiyatı döndüren özellik
        public decimal GetCurrentPrice
        {
            get => DiscountAppliedPrice != null ? DiscountAppliedPrice.Value : Price; // İndirim uygulanmışsa, indirimli fiyatı döner; aksi takdirde, normal fiyatı döner.
        }

        // İndirim uygulama metodu
        public void AppliedDiscount(decimal discountPrice)
        {
            DiscountAppliedPrice = discountPrice; // İndirimli fiyatı atar
        }
    }
}
