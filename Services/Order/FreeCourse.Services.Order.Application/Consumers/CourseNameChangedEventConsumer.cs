using FreeCourse.Services.Order.Infrastructure;
using FreeCourse.Shared.Messages;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace FreeCourse.Services.Order.Application.Consumers
{
    // CourseNameChangedEvent tüketicisi, kurs adı değişikliği olaylarını işlemek için kullanılır.
    public class CourseNameChangedEventConsumer : IConsumer<CourseNameChangedEvent>
    {
        private readonly OrderDbContext _orderDbContext;

        // Constructor, OrderDbContext'i alır ve sınıfın bir örneğini oluşturur.
        public CourseNameChangedEventConsumer(OrderDbContext orderDbContext)
        {
            _orderDbContext = orderDbContext;
        }

        // Olay alındığında tetiklenen metod. Kurs adı değiştiğinde siparişlerdeki ilgili ürünlerin güncellenmesini sağlar.
        public async Task Consume(ConsumeContext<CourseNameChangedEvent> context)
        {
            // Değişiklikten etkilenen sipariş öğelerini veritabanından al.
            var orderItems = await _orderDbContext.OrderItems
                .Where(x => x.ProductId == context.Message.CourseId) // Ürün ID'si ile filtreleme
                .ToListAsync();

            // Her bir sipariş öğesi için güncelleme yap.
            orderItems.ForEach(x =>
            {
                // Sipariş öğesinin adını güncelle.
                x.UpdateOrderItem(context.Message.UpdatedName, x.PictureUrl, x.Price);
            });

            // Değişiklikleri veritabanına kaydet.
            await _orderDbContext.SaveChangesAsync();
        }
    }
}
