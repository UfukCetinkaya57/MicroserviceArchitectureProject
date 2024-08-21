using FreeCourse.Services.Catalog.Dtos;
using FreeCourse.Services.Catalog.Services;
using FreeCourse.Shared.ControllerBases;
using FreeCourse.Shared.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FreeCourse.Services.Catalog.Controllers
{
    // CoursesController, kurslarla ilgili HTTP isteklerini yöneten bir denetleyici sınıfıdır.
    [Route("api/[controller]")]
    [ApiController]
    public class CoursesController : CustomBaseController
    {
        private readonly ICourseService _courseService; // Kurs işlemlerini gerçekleştirmek için kullanılan servis.

        // Constructor, kurs servisini alır ve alan değişkenine atar.
        public CoursesController(ICourseService courseService)
        {
            _courseService = courseService;
        }

        // Tüm kursları almak için kullanılan GET metodu.
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            // Kurs servisi üzerinden tüm kursları alır.
            var response = await _courseService.GetAllAsync();
            // Alınan kursları döndürmek için yardımcı metodu çağırır.
            return CreateActionResultInstance(response);
        }

        // Belirli bir kurs ID'sine göre kurs almak için kullanılan GET metodu.
        [HttpGet("{id}")] // Burada URL'deki ID parametresi kullanılır.
        public async Task<IActionResult> GetById(string id)
        {
            // Kurs servisi üzerinden belirtilen ID'ye ait kursu alır.
            var response = await _courseService.GetByIdAsync(id);
            // Alınan kursu döndürmek için yardımcı metodu çağırır.
            return CreateActionResultInstance(response);
        }

        // Belirli bir kullanıcı ID'sine göre tüm kursları almak için kullanılan GET metodu.
        [Route("/api/[controller]/GetAllByUserId/{userId}")] // Bu özel rota, kullanıcının kurslarını almak için kullanılır.
        [HttpGet]
        public async Task<IActionResult> GetAllByUserId(string userId)
        {
            // Kurs servisi üzerinden belirtilen kullanıcıya ait tüm kursları alır.
            var response = await _courseService.GetAllByUserIdAsync(userId);
            // Alınan kursları döndürmek için yardımcı metodu çağırır.
            return CreateActionResultInstance(response);
        }

        // Yeni bir kurs oluşturmak için kullanılan POST metodu.
        [HttpPost]
        public async Task<IActionResult> Create(CourseCreateDto courseCreateDto)
        {
            // Kurs servisi üzerinden yeni kursu oluşturur.
            var response = await _courseService.CreateAsync(courseCreateDto);
            // Oluşturulan kursu döndürmek için yardımcı metodu çağırır.
            return CreateActionResultInstance(response);
        }

        // Var olan bir kursu güncellemek için kullanılan PUT metodu.
        [HttpPut]
        public async Task<IActionResult> Update(CourseUpdateDto courseUpdateDto)
        {
            // Kurs servisi üzerinden mevcut kursu günceller.
            var response = await _courseService.UpdateAsync(courseUpdateDto);
            // Güncellenen kursu döndürmek için yardımcı metodu çağırır.
            return CreateActionResultInstance(response);
        }

        // Belirli bir kursu silmek için kullanılan DELETE metodu.
        [HttpDelete("{id}")] // Burada URL'deki ID parametresi kullanılır.
        public async Task<IActionResult> Delete(string id)
        {
            // Kurs servisi üzerinden belirtilen ID'ye ait kursu siler.
            var response = await _courseService.DeleteAsync(id);
            // Silme işlemine dair yanıtı döndürmek için yardımcı metodu çağırır.
            return CreateActionResultInstance(response);
        }
    }
}
