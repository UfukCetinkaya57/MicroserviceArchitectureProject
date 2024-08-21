using AutoMapper;
using System;

namespace FreeCourse.Services.Order.Application.Mapping
{
    // ObjectMapper sınıfı, AutoMapper için singleton bir yapı sağlar ve haritalama konfigürasyonunu yönetir.
    public static class ObjectMapper
    {
        // Lazy<IMapper> kullanarak, IMapper örneğini yalnızca ihtiyaç duyulduğunda oluşturur.
        private static readonly Lazy<IMapper> lazy = new Lazy<IMapper>(() =>
        {
            // MapperConfiguration oluşturulurken, CustomMapping profili eklenir.
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<CustomMapping>();
            });

            // Mapper'ı oluştur ve döndür.
            return config.CreateMapper();
        });

        // Mapper'ı erişmek için kullanılır.
        public static IMapper Mapper => lazy.Value;
    }
}
