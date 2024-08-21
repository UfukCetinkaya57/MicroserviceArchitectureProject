using AutoMapper;
using FreeCourse.Services.Order.Application.Dtos;
using FreeCourse.Services.Order.Domain.OrderAggregate;

namespace FreeCourse.Services.Order.Application.Mapping
{
    // CustomMapping sınıfı, AutoMapper ile domain nesneleri ile DTO'lar arasında dönüşüm yapılandırmasını içerir.
    internal class CustomMapping : Profile
    {
        // Constructor, dönüşüm haritalarını tanımlar.
        public CustomMapping()
        {
            // Domain OrderAggregate.Order ile OrderDto arasında dönüşüm oluştur.
            CreateMap<Domain.OrderAggregate.Order, OrderDto>().ReverseMap();

            // OrderItem ile OrderItemDto arasında dönüşüm oluştur.
            CreateMap<OrderItem, OrderItemDto>().ReverseMap();

            // Address ile AddressDto arasında dönüşüm oluştur.
            CreateMap<Address, AddressDto>().ReverseMap();
        }
    }
}
