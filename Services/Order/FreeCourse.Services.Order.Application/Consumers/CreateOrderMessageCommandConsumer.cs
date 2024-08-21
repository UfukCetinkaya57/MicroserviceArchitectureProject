using FreeCourse.Services.Order.Infrastructure;
using FreeCourse.Shared.Messages;
using MassTransit;
using System.Threading.Tasks;

namespace FreeCourse.Services.Order.Application.Consumers
{
    // CreateOrderMessageCommand tüketicisi, yeni sipariş oluşturma komutlarını işlemek için kullanılır.
    public class CreateOrderMessageCommandConsumer : IConsumer<CreateOrderMessageCommand>
    {
        private readonly OrderDbContext _orderDbContext;

        // Constructor, OrderDbContext'i alır ve sınıfın bir örneğini oluşturur.
        public CreateOrderMessageCommandConsumer(OrderDbContext orderDbContext)
        {
            _orderDbContext = orderDbContext;
        }

        // Olay alındığında tetiklenen metod. Yeni sipariş oluşturma isteğini işler.
        public async Task Consume(ConsumeContext<CreateOrderMessageCommand> context)
        {
            // Yeni bir adres nesnesi oluştur.
            var newAddress = new Domain.OrderAggregate.Address(
                context.Message.Province,
                context.Message.District,
                context.Message.Street,
                context.Message.ZipCode,
                context.Message.Line
            );

            // Yeni bir sipariş nesnesi oluştur.
            Domain.OrderAggregate.Order order = new Domain.OrderAggregate.Order(context.Message.BuyerId, newAddress);

            // Sipariş öğelerini ekle.
            context.Message.OrderItems.ForEach(x =>
            {
                order.AddOrderItem(x.ProductId, x.ProductName, x.Price, x.PictureUrl);
            });

            // Siparişi veritabanına ekle.
            await _orderDbContext.Orders.AddAsync(order);

            // Değişiklikleri veritabanına kaydet.
            await _orderDbContext.SaveChangesAsync();
        }
    }
}
