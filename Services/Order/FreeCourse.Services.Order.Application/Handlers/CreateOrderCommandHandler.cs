using FreeCourse.Services.Order.Application.Commands;
using FreeCourse.Services.Order.Application.Dtos;
using FreeCourse.Services.Order.Domain.OrderAggregate;
using FreeCourse.Services.Order.Infrastructure;
using FreeCourse.Shared.Dtos;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace FreeCourse.Services.Order.Application.Handlers
{
    // CreateOrderCommandHandler sınıfı, sipariş oluşturma komutlarını işlemek için kullanılır.
    public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Response<CreatedOrderDto>>
    {
        private readonly OrderDbContext _context;

        // Constructor, OrderDbContext'i alır ve sınıfın bir örneğini oluşturur.
        public CreateOrderCommandHandler(OrderDbContext context)
        {
            _context = context;
        }

        // Handle metodu, CreateOrderCommand'ı işler ve bir CreatedOrderDto döner.
        public async Task<Response<CreatedOrderDto>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            // Yeni bir Address nesnesi oluştur.
            var newAddress = new Address(
                request.Address.Province,
                request.Address.District,
                request.Address.Street,
                request.Address.ZipCode,
                request.Address.Line
            );

            // Yeni bir Order nesnesi oluştur.
            Domain.OrderAggregate.Order newOrder = new Domain.OrderAggregate.Order(request.BuyerId, newAddress);

            // Sipariş öğelerini ekle.
            request.OrderItems.ForEach(x =>
            {
                newOrder.AddOrderItem(x.ProductId, x.ProductName, x.Price, x.PictureUrl);
            });

            // Siparişi veritabanına ekle.
            await _context.Orders.AddAsync(newOrder);

            // Değişiklikleri veritabanına kaydet.
            await _context.SaveChangesAsync();

            // Başarıyla oluşturulan siparişin yanıtını döndür.
            return Response<CreatedOrderDto>.Success(new CreatedOrderDto { OrderId = newOrder.Id }, 200);
        }
    }
}
