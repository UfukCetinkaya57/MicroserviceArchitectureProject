using AutoMapper; // AutoMapper kütüphanesini kullanmak için eklenir.
using FreeCourse.Services.Catalog.Dtos; // DTO sınıflarını kullanmak için eklenir.
using FreeCourse.Services.Catalog.Model; // Model sınıflarını kullanmak için eklenir.
using FreeCourse.Services.Catalog.Settings; // Veritabanı ayarlarını kullanmak için eklenir.
using FreeCourse.Shared.Dtos; // Paylaşılan DTO sınıflarını kullanmak için eklenir.
using MongoDB.Driver; // MongoDB sürücüsünü kullanmak için eklenir.
using System.Collections.Generic; // Liste türlerini kullanmak için eklenir.

namespace FreeCourse.Services.Catalog.Services
{
    // CategoryService, MongoDB'deki kategorilerle etkileşimi yönetir.
    public class CategoryService : ICategoryService
    {
        // MongoDB'deki kategori koleksiyonu.
        private readonly IMongoCollection<Category> _categoryCollection;

        // AutoMapper örneği, nesneleri eşleştirmek için kullanılır.
        private readonly IMapper _mapper;

        // Constructor, veritabanı ayarlarını alır ve MongoDB bağlantısını oluşturur.
        public CategoryService(IMapper mapper, IDatabaseSettings databaseSettings)
        {
            var client = new MongoClient(databaseSettings.ConnectionString); // MongoDB istemcisi oluşturur.
            var database = client.GetDatabase(databaseSettings.DatabaseName); // Belirtilen veritabanını alır.
            _categoryCollection = database.GetCollection<Category>(databaseSettings.CategoryCollectionName); // Kategori koleksiyonunu alır.
            _mapper = mapper; // AutoMapper örneğini atar.
        }

        // Tüm kategorileri getirir.
        public async Task<Response<List<CategoryDto>>> GetAllAsync()
        {
            // Tüm kategorileri MongoDB'den alır.
            var categories = await _categoryCollection.Find(category => true).ToListAsync();
            // Kategorileri DTO'lara dönüştürüp başarılı yanıt döner.
            return Response<List<CategoryDto>>.Success(_mapper.Map<List<CategoryDto>>(categories), 200);
        }

        // Yeni bir kategori oluşturur.
        public async Task<Response<CategoryDto>> CreateAsync(CategoryDto categoryDto)
        {
            // DTO'yu Category modeline dönüştürür.
            var category = _mapper.Map<Category>(categoryDto);
            // MongoDB'ye yeni kategoriyi ekler.
            await _categoryCollection.InsertOneAsync(category);
            // Oluşturulan kategoriyi DTO'ya dönüştürüp başarılı yanıt döner.
            return Response<CategoryDto>.Success(_mapper.Map<CategoryDto>(category), 200);
        }

        // Belirli bir kategoriyi ID'sine göre getirir.
        public async Task<Response<CategoryDto>> GetByIdAsync(string id)
        {
            // MongoDB'de ID'sine göre kategoriyi bulur.
            var category = await _categoryCollection.Find<Category>(x => x.Id == id).FirstOrDefaultAsync();
            if (category == null)
                return Response<CategoryDto>.Fail("Category not found", 404); // Kategori bulunamazsa hata döner.
            // Kategoriyi DTO'ya dönüştürüp başarılı yanıt döner.
            return Response<CategoryDto>.Success(_mapper.Map<CategoryDto>(category), 200);
        }
    }
}
