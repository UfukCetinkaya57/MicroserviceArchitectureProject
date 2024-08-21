using FreeCourse.Shared.Dtos; // Ortak DTO'lar
using FreeCourse.Shared.Services; // Ortak servis arayüzleri
using FreeCourse.Web.Models.FakePayments; // Ödeme modelleri
using FreeCourse.Web.Models.Orders; // Sipariş modelleri
using FreeCourse.Web.Services.Interfaces; // Servis arayüzleri
using System; // Temel sistem sınıfları
using System.Collections.Generic; // Koleksiyon sınıfları
using System.Linq; // LINQ işlevleri
using System.Net.Http; // HTTP istemcisi
using System.Net.Http.Json; // JSON ile çalışmak için genişletmeler
using System.Threading.Tasks; // Asenkron programlama için

namespace FreeCourse.Web.Services
{
    // Sipariş servisi
    public class OrderService : IOrderService
    {
        // Ödeme servisi
        private readonly IPaymentService _paymentService;
        // HTTP istemcisi
        private readonly HttpClient _httpClient;
        // Sepet servisi
        private readonly IBasketService _basketService;
        // Kimlik servisi
        private readonly ISharedIdentityService _sharedIdentityService;

        // Yapıcı metot
        public OrderService(IPaymentService paymentService, HttpClient httpClient, IBasketService basketService, ISharedIdentityService sharedIdentityService)
        {
            _paymentService = paymentService; // Ödeme servisini al
            _httpClient = httpClient; // HTTP istemcisini al
            _basketService = basketService; // Sepet servisini al
            _sharedIdentityService = sharedIdentityService; // Kimlik servisini al
        }

        // Sipariş oluşturma metodu
        public async Task<OrderCreatedViewModel> CreateOrder(CheckoutInfoInput checkoutInfoInput)
        {
            // Sepeti al
            var basket = await _basketService.Get();

            // Ödeme bilgilerini hazırla
            var paymentInfoInput = new PaymentInfoInput()
            {
                CardName = checkoutInfoInput.CardName,
                CardNumber = checkoutInfoInput.CardNumber,
                Expiration = checkoutInfoInput.Expiration,
                CVV = checkoutInfoInput.CVV,
                TotalPrice = basket.TotalPrice // Sepet toplamı
            };

            // Ödeme işlemini gerçekleştir
            var responsePayment = await _paymentService.ReceivePayment(paymentInfoInput);
            if (!responsePayment) // Ödeme başarısızsa
            {
                return new OrderCreatedViewModel() { Error = "Ödeme alınamadı", IsSuccessful = false };
            }

            // Sipariş oluşturma bilgilerini hazırla
            var orderCreateInput = new OrderCreateInput()
            {
                BuyerId = _sharedIdentityService.GetUserId, // Kullanıcı ID'si
                Address = new AddressCreateInput
                {
                    Province = checkoutInfoInput.Province,
                    District = checkoutInfoInput.District,
                    Street = checkoutInfoInput.Street,
                    Line = checkoutInfoInput.Line,
                    ZipCode = checkoutInfoInput.ZipCode
                },
            };

            // Sepet içeriğini sipariş kalemlerine dönüştür
            basket.BasketItems.ForEach(x =>
            {
                var orderItem = new OrderItemCreateInput
                {
                    ProductId = x.CourseId,
                    Price = x.GetCurrentPrice,
                    PictureUrl = "",
                    ProductName = x.CourseName
                };
                orderCreateInput.OrderItems.Add(orderItem); // Sipariş kalemini ekle
            });

            // Siparişi oluştur
            var response = await _httpClient.PostAsJsonAsync<OrderCreateInput>("orders", orderCreateInput);
            if (!response.IsSuccessStatusCode) // Sipariş oluşturulamadıysa
            {
                return new OrderCreatedViewModel() { Error = "Sipariş oluşturulamadı", IsSuccessful = false };
            }

            // Başarıyla oluşturulan siparişi al
            var orderCreatedViewModel = await response.Content.ReadFromJsonAsync<Response<OrderCreatedViewModel>>();
            orderCreatedViewModel.Data.IsSuccessful = true; // Başarılı durumu ayarla
            await _basketService.Delete(); // Sepeti temizle
            return orderCreatedViewModel.Data; // Oluşturulan siparişi döndür
        }

        // Tüm siparişleri alma metodu
        public async Task<List<OrderViewModel>> GetOrder()
        {
            var response = await _httpClient.GetFromJsonAsync<Response<List<OrderViewModel>>>("orders");
            return response.Data; // Sipariş listesini döndür
        }

        // Siparişi askıya alma metodu
        public async Task<OrderSuspendViewModel> SuspendOrder(CheckoutInfoInput checkoutInfoInput)
        {
            // Sepeti al
            var basket = await _basketService.Get();

            // Sipariş oluşturma bilgilerini hazırla
            var orderCreateInput = new OrderCreateInput()
            {
                BuyerId = _sharedIdentityService.GetUserId, // Kullanıcı ID'si
                Address = new AddressCreateInput
                {
                    Province = checkoutInfoInput.Province,
                    District = checkoutInfoInput.District,
                    Street = checkoutInfoInput.Street,
                    Line = checkoutInfoInput.Line,
                    ZipCode = checkoutInfoInput.ZipCode
                },
            };

            // Sepet içeriğini sipariş kalemlerine dönüştür
            basket.BasketItems.ForEach(x =>
            {
                var orderItem = new OrderItemCreateInput
                {
                    ProductId = x.CourseId,
                    Price = x.GetCurrentPrice,
                    PictureUrl = "",
                    ProductName = x.CourseName
                };
                orderCreateInput.OrderItems.Add(orderItem); // Sipariş kalemini ekle
            });

            // Ödeme bilgilerini hazırla
            var paymentInfoInput = new PaymentInfoInput()
            {
                CardName = checkoutInfoInput.CardName,
                CardNumber = checkoutInfoInput.CardNumber,
                Expiration = checkoutInfoInput.Expiration,
                CVV = checkoutInfoInput.CVV,
                TotalPrice = basket.TotalPrice, // Sepet toplamı
                Order = orderCreateInput // Sipariş bilgisi
            };

            // Ödeme işlemini gerçekleştir
            var responsePayment = await _paymentService.ReceivePayment(paymentInfoInput);
            if (!responsePayment) // Ödeme başarısızsa
            {
                return new OrderSuspendViewModel() { Error = "Ödeme alınamadı", IsSuccessful = false };
            }

            await _basketService.Delete(); // Sepeti temizle
            return new OrderSuspendViewModel() { IsSuccessful = true }; // Başarılı durumu döndür
        }
    }
}
