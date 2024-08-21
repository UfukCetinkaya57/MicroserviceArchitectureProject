using FreeCourse.Web.Models.FakePayments; // Ödeme modelleri
using FreeCourse.Web.Services.Interfaces; // Servis arayüzleri
using System; // Temel sistem sınıfları
using System.Collections.Generic; // Koleksiyon sınıfları
using System.Linq; // LINQ işlevleri
using System.Net.Http; // HTTP istemcisi
using System.Net.Http.Json; // JSON ile çalışmak için genişletmeler
using System.Threading.Tasks; // Asenkron programlama için

namespace FreeCourse.Web.Services
{
    // Ödeme servisi
    public class PaymentService : IPaymentService
    {
        // HTTP istemcisi
        private readonly HttpClient _httpClient;

        // Yapıcı metot
        public PaymentService(HttpClient httpClient)
        {
            _httpClient = httpClient; // HTTP istemcisini al
        }

        // Ödeme alma metodu
        public async Task<bool> ReceivePayment(PaymentInfoInput paymentInfoInput)
        {
            // Ödeme bilgilerini içeren bir HTTP POST isteği gönder
            var response = await _httpClient.PostAsJsonAsync<PaymentInfoInput>("fakepayments", paymentInfoInput);

            // İsteğin başarısını döndür
            return response.IsSuccessStatusCode; // Başarılı ise true, değilse false döner
        }
    }
}
