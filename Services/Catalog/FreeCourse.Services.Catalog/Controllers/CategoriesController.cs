using FreeCourse.Services.Catalog.Dtos;
using FreeCourse.Services.Catalog.Model;
using FreeCourse.Services.Catalog.Services;
using FreeCourse.Shared.ControllerBases;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FreeCourse.Services.Catalog.Controllers
{
    // CategoriesController, kategori ile ilgili HTTP isteklerini yöneten bir denetleyici sınıfıdır.
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : CustomBaseController
    {
        private readonly ICategoryService _categoryService; // Kategori işlemlerini gerçekleştirmek için kullanılan servis.

        // Constructor, kategori servisini alır ve alan değişkenine atar.
        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        // Tüm kategorileri almak için kullanılan GET metodu.
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            // Kategori servisi üzerinden tüm kategorileri alır.
            var categories = await _categoryService.GetAllAsync();
            // Alınan kategorileri döndürmek için yardımcı metodu çağırır.
            return CreateActionResultInstance(categories);
        }

        // Belirli bir kategori ID'sine göre kategori almak için kullanılan GET metodu.
        [HttpGet("{id}")] // Burada URL'deki ID parametresi kullanılır.
        public async Task<IActionResult> GetById(string id)
        {
            // Kategori servisi üzerinden belirtilen ID'ye ait kategoriyi alır.
            var category = await _categoryService.GetByIdAsync(id);
            // Alınan kategoriyi döndürmek için yardımcı metodu çağırır.
            return CreateActionResultInstance(category);
        }

        // Yeni bir kategori oluşturmak için kullanılan POST metodu.
        [HttpPost]
        public async Task<IActionResult> Create(CategoryDto categoryDto)
        {
            // Kategori servisi üzerinden yeni kategoriyi oluşturur.
            var response = await _categoryService.CreateAsync(categoryDto);
            // Oluşturulan kategoriyi döndürmek için yardımcı metodu çağırır.
            return CreateActionResultInstance(response);
        }
    }
}
