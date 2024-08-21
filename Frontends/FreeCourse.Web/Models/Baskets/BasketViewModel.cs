using System; // Temel sistem sınıfları
using System.Collections.Generic; // Koleksiyon sınıfları
using System.Linq; // LINQ işlevleri
using System.Threading.Tasks; // Asenkron programlama için

namespace FreeCourse.Web.Models.Baskets
{
    // Alışveriş sepetini temsil eden model
    public class BasketViewModel
    {
        // Constructor
        public BasketViewModel()
        {
            _basketItems = new List<BasketItemViewModel>(); // Sepet öğeleri için liste başlatılır
        }

        // Kullanıcı kimliği
        public string UserId { get; set; }

        // İndirim kodu
        public string? DiscountCode { get; set; }

        // İndirim oranı
        public int? DiscountRate { get; set; }

        // Sepet öğelerini saklamak için özel liste
        private List<BasketItemViewModel> _basketItems;

        // Sepet öğeleri
        public List<BasketItemViewModel> BasketItems
        {
            get
            {
                if (HasDiscount) // Eğer indirim varsa
                {
                    // Örnek: kurs fiyatı 100 TL, indirim %10
                    _basketItems.ForEach(x =>
                    {
                        var discountPrice = x.Price * ((decimal)DiscountRate.Value / 100); // İndirimli fiyat hesapla
                        x.AppliedDiscount(Math.Round(x.Price - discountPrice, 2)); // İndirim uygulama
                    });
                }
                return _basketItems; // Sepet öğelerini döner
            }
            set
            {
                _basketItems = value; // Sepet öğeleri atanır
            }
        }

        // Toplam fiyatı hesaplayan özellik
        public decimal TotalPrice
        {
            get => _basketItems.Sum(x => x.GetCurrentPrice); // Sepetteki tüm öğelerin geçerli fiyatlarının toplamını döner
        }

        // İndirim var mı?
        public bool HasDiscount
        {
            get => !string.IsNullOrEmpty(DiscountCode) && DiscountRate.HasValue; // İndirim kodu varsa ve oran belirlenmişse true döner
        }

        // İndirim iptal etme metodu
        public void CancelDiscount()
        {
            DiscountCode = null; // İndirim kodunu sıfırla
            DiscountRate = null; // İndirim oranını sıfırla
        }

        // İndirim uygulama metodu
        public void ApplyDiscount(string code, int rate)
        {
            DiscountCode = code; // İndirim kodunu ata
            DiscountRate = rate; // İndirim oranını ata
        }
    }
}
