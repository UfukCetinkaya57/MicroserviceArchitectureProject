using FreeCourse.Services.FakePayment.Models; // PaymentDto ve diğer model sınıflarını kullanmak için
using FreeCourse.Shared.ControllerBases; // Özelleştirilmiş temel controller sınıfına erişim sağlamak için
using FreeCourse.Shared.Dtos; // Paylaşılan DTO'lara (Data Transfer Object) erişim sağlamak için
using FreeCourse.Shared.Messages; // Mesajlaşma sınıflarına (CreateOrderMessageCommand gibi) erişim sağlamak için
using MassTransit; // MassTransit kütüphanesini kullanmak için (mesajlaşma altyapısı için)
using Microsoft.AspNetCore.Http; // HTTP ile ilgili sınıflara erişim sağlamak için
using Microsoft.AspNetCore.Mvc; // MVC Controller altyapısına erişim sağlamak için
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks; // Asenkron programlama desteği için

namespace FreeCourse.Services.FakePayment.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FakePaymentsController : CustomBaseController
    {
        private readonly ISendEndpointProvider _sendEndpointProvider; // Mesaj gönderme işlemleri için MassTransit'in ISendEndpointProvider'ını kullanır

        // Yapılandırıcı: ISendEndpointProvider'ı enjekte eder
        public FakePaymentsController(ISendEndpointProvider sendEndpointProvider)
        {
            _sendEndpointProvider = sendEndpointProvider;
        }

        // Ödeme alma işlemi yapar ve mesaj kuyruğuna sipariş oluşturma komutu gönderir
        [HttpPost]
        public async Task<IActionResult> ReceivePayment(PaymentDto paymentDto)
        {
            // paymentDto ile ödeme işlemi gerçekleştirilir (işlemin detayı simüle edilmiştir)
            var sendEndpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri("queue:create-order-service"));

            // CreateOrderMessageCommand sınıfından yeni bir mesaj oluşturulur
            var createOrderMessageCommand = new CreateOrderMessageCommand
            {
                BuyerId = paymentDto.Order.BuyerId,
                Province = paymentDto.Order.Address.Province,
                District = paymentDto.Order.Address.District,
                Street = paymentDto.Order.Address.Street,
                Line = paymentDto.Order.Address.Line,
                ZipCode = paymentDto.Order.Address.ZipCode
            };

            // OrderItems üzerinden geçerek her birini mesajın OrderItems listesine ekler
            paymentDto.Order.OrderItems.ForEach(x =>
            {
                createOrderMessageCommand.OrderItems.Add(new OrderItem
                {
                    PictureUrl = x.PictureUrl,
                    Price = x.Price,
                    ProductId = x.ProductId,
                    ProductName = x.ProductName
                });
            });

            // Mesaj kuyruğuna sipariş oluşturma komutu gönderilir
            await sendEndpoint.Send<CreateOrderMessageCommand>(createOrderMessageCommand);

            // İşlem başarılı olduğunda 200 OK döner
            return CreateActionResultInstance(Shared.Dtos.Response<NoContent>.Success(200));
        }
    }
}
