using FreeCourse.Web.Models.Orders; // Sipariş modellerini içe aktarma
using FreeCourse.Web.Services.Interfaces; // Servis arayüzlerini içe aktarma
using Microsoft.AspNetCore.Mvc; // MVC yapısı için gerekli sınıflar
using System; // Temel sistem sınıfları
using System.Collections.Generic; // Koleksiyon sınıfları
using System.Linq; // LINQ işlevleri
using System.Threading.Tasks; // Asenkron programlama için

namespace FreeCourse.Web.Controllers
{
    public class OrderController : Controller
    {
        private readonly IBasketService _basketService; // Sepet servisi
        private readonly IOrderService _orderService; // Sipariş servisi

        public OrderController(IBasketService basketService, IOrderService orderService)
        {
            _basketService = basketService; // Sepet servisini atama
            _orderService = orderService; // Sipariş servisini atama
        }

        // Kullanıcının alışveriş sepetini kontrol edip ödeme sayfasını görüntüler
        public async Task<IActionResult> Checkout()
        {
            var basket = await _basketService.Get(); // Sepeti al

            ViewBag.basket = basket; // Sepet bilgilerini görünümde kullanmak için atama
            return View(new CheckoutInfoInput()); // Yeni CheckoutInfoInput modelini döndür
        }

        [HttpPost]
        public async Task<IActionResult> Checkout(CheckoutInfoInput checkoutInfoInput)
        {
            // 1. yol: Senkron iletişim
            // var orderStatus = await _orderService.CreateOrder(checkoutInfoInput);

            // 2. yol: Asenkron iletişim
            var orderSuspend = await _orderService.SuspendOrder(checkoutInfoInput); // Siparişi askıya al
            if (!orderSuspend.IsSuccessful) // Eğer işlem başarısız olduysa
            {
                var basket = await _basketService.Get(); // Sepeti tekrar al

                ViewBag.basket = basket; // Sepet bilgilerini görünümde kullanmak için atama

                ViewBag.error = orderSuspend.Error; // Hata mesajını görünümde göster

                return View(); // Hata ile birlikte görünümü döndür
            }

            // 1. yol: Senkron iletişim
            // return RedirectToAction(nameof(SuccessfulCheckout), new { orderId = orderSuspend.OrderId });

            // 2. yol: Asenkron iletişim, rastgele bir sipariş ID'si döndürme
            return RedirectToAction(nameof(SuccessfulCheckout), new { orderId = new Random().Next(1, 1000) });
        }

        // Başarılı sipariş sayfasını görüntüler
        public IActionResult SuccessfulCheckout(int orderId)
        {
            ViewBag.orderId = orderId; // Sipariş ID'sini görünümde kullanmak için atama
            return View(); // Başarılı ödeme görünümünü döndür
        }

        // Kullanıcının sipariş geçmişini görüntüler
        public async Task<IActionResult> CheckoutHistory()
        {
            return View(await _orderService.GetOrder()); // Sipariş geçmişini al ve görünümü döndür
        }
    }
}
