using FreeCourse.Services.Discount.Services; // İndirim servisiyle ilgili iş mantığı katmanına erişim sağlamak için
using FreeCourse.Shared.ControllerBases; // Özel taban kontrolcü sınıfına erişim sağlamak için
using FreeCourse.Shared.Services; // Paylaşılan servisler için kullanılan sınıfa erişim sağlamak için
using Microsoft.AspNetCore.Http; // HTTP özelliklerine erişim sağlamak için
using Microsoft.AspNetCore.Mvc; // MVC özelliklerine erişim sağlamak için

namespace FreeCourse.Services.Discount.Controllers
{
    // API rotasını ve denetleyici türünü belirler
    [Route("api/[controller]")]
    [ApiController]
    public class DiscountsController : CustomBaseController // Özel taban kontrolcü sınıfından türetilmiştir
    {
        private readonly IDiscountService _discountService; // İndirim servisi bağımlılığı
        private readonly ISharedIdentityService _sharedIdentityService; // Paylaşılan kimlik servisi bağımlılığı

        // Denetleyici sınıfı için yapılandırıcı
        public DiscountsController(IDiscountService discountService, ISharedIdentityService sharedIdentityService)
        {
            _discountService = discountService; // İndirim servisini yapılandırıcı ile enjekte et
            _sharedIdentityService = sharedIdentityService; // Paylaşılan kimlik servisini yapılandırıcı ile enjekte et
        }

        // Tüm indirimleri getirmek için HTTP GET isteği
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return CreateActionResultInstance(await _discountService.GetAll()); // İndirimleri getir ve sonuçları döndür
        }

        // Belirtilen ID ile indirimi getirmek için HTTP GET isteği
        // api/discounts/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var discount = await _discountService.GetById(id); // Belirtilen ID'ye sahip indirimi getir
            return CreateActionResultInstance(discount); // Sonucu döndür
        }

        // Belirtilen kod ve kullanıcı ID'si ile indirimi getirmek için HTTP GET isteği
        // api/discounts/GetByCode/ABC123
        [Route("/api/[controller]/[action]/{code}")]
        [HttpGet]
        public async Task<IActionResult> GetByCode(string code)
        {
            var userId = _sharedIdentityService.GetUserId; // Kullanıcı ID'sini al
            var discount = await _discountService.GetByCodeAndUserId(code, userId); // Kod ve kullanıcı ID'sine göre indirimi getir
            return CreateActionResultInstance(discount); // Sonucu döndür
        }

        // Yeni bir indirim kaydetmek için HTTP POST isteği
        [HttpPost]
        public async Task<IActionResult> Save(Models.Discount discount)
        {
            return CreateActionResultInstance(await _discountService.Save(discount)); // İndirimi kaydet ve sonucu döndür
        }

        // Mevcut bir indirimi güncellemek için HTTP PUT isteği
        [HttpPut]
        public async Task<IActionResult> Update(Models.Discount discount)
        {
            return CreateActionResultInstance(await _discountService.Update(discount)); // İndirimi güncelle ve sonucu döndür
        }

        // Belirtilen ID ile indirimi silmek için HTTP DELETE isteği
        // api/discounts/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            return CreateActionResultInstance(await _discountService.Delete(id)); // İndirimi sil ve sonucu döndür
        }
    }
}
