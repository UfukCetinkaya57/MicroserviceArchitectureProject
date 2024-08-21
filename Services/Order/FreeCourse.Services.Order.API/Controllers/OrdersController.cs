using FreeCourse.Services.Order.Application.Commands;
using FreeCourse.Services.Order.Application.Queries;
using FreeCourse.Shared.ControllerBases;
using FreeCourse.Shared.Services;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace FreeCourse.Services.Order.API.Controllers
{
    // API için OrdersController sınıfı, siparişlerle ilgili işlemleri yönetir.
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : CustomBaseController
    {
        private readonly IMediator _mediator; // Mediator aracılığıyla komut ve sorguları yönetmek için
        private readonly ISharedIdentityService _sharedIdentityService; // Kullanıcı kimlik bilgilerini almak için

        // Constructor, IMediator ve ISharedIdentityService bağımlılıklarını alır.
        public OrdersController(IMediator mediator, ISharedIdentityService sharedIdentityService)
        {
            _mediator = mediator;
            _sharedIdentityService = sharedIdentityService;
        }

        // Kullanıcının siparişlerini almak için HTTP GET isteği
        [HttpGet]
        public async Task<IActionResult> GetOrders()
        {
            // Kullanıcının kimliğini al ve GetOrdersByUserIdQuery sorgusunu gönder
            var response = await _mediator.Send(new GetOrdersByUserIdQuery { UserId = _sharedIdentityService.GetUserId });

            // Yanıtı oluştur ve döndür
            return CreateActionResultInstance(response);
        }

        // Yeni bir sipariş kaydetmek için HTTP POST isteği
        [HttpPost]
        public async Task<IActionResult> SaveOrder(CreateOrderCommand createOrderCommand)
        {
            // Siparişi oluşturmak için CreateOrderCommand'ı gönder
            var response = await _mediator.Send(createOrderCommand);

            // Yanıtı oluştur ve döndür
            return CreateActionResultInstance(response);
        }
    }
}
