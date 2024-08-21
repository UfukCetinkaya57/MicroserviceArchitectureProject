using AutoMapper;
using FreeCourse.Services.Order.Application.Dtos;
using FreeCourse.Services.Order.Application.Mapping;
using FreeCourse.Services.Order.Application.Queries;
using FreeCourse.Services.Order.Infrastructure;
using FreeCourse.Shared.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FreeCourse.Services.Order.Application.Handlers
{
    // GetOrdersByUserIdQueryHandler, kullanıcı ID'sine göre siparişleri almak için kullanılan bir MediatR isteği işleyicisidir.
    internal class GetOrdersByUserIdQueryHandler : IRequestHandler<GetOrdersByUserIdQuery, Response<List<OrderDto>>>
    {
        private readonly OrderDbContext _context;

        // Constructor, OrderDbContext örneğini alır.
        public GetOrdersByUserIdQueryHandler(OrderDbContext context)
        {
            _context = context;
        }

        // Handle metodu, verilen kullanıcı ID'sine göre siparişleri getirir.
        public async Task<Response<List<OrderDto>>> Handle(GetOrdersByUserIdQuery request, CancellationToken cancellationToken)
        {
            // Kullanıcı ID'sine göre siparişleri al ve OrderItems ile birlikte yükle.
            var orders = await _context.Orders
                .Include(x => x.OrderItems) // Sipariş öğelerini dahil et.
                .Where(x => x.BuyerId == request.UserId) // Kullanıcı ID'sine göre filtrele.
                .ToListAsync();

            // Eğer sipariş bulunamazsa, boş bir liste ile başarılı yanıt döndür.
            if (!orders.Any())
            {
                return Response<List<OrderDto>>.Success(new List<OrderDto>(), 200);
            }

            // Siparişleri OrderDto'ya dönüştür.
            var ordersDto = ObjectMapper.Mapper.Map<List<OrderDto>>(orders);

            // Dönüştürülmüş siparişleri başarılı yanıt ile döndür.
            return Response<List<OrderDto>>.Success(ordersDto, 200);
        }
    }
}
