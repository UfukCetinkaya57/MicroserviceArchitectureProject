using AutoMapper; // AutoMapper kütüphanesini kullanmak için eklenir.
using FreeCourse.Services.Catalog.Dtos; // DTO sınıflarını kullanmak için eklenir.
using FreeCourse.Services.Catalog.Model; // Model sınıflarını kullanmak için eklenir.
using FreeCourse.Services.Catalog.Settings; // Veritabanı ayarlarını kullanmak için eklenir.
using FreeCourse.Shared.Dtos; // Paylaşılan DTO sınıflarını kullanmak için eklenir.
using Mass = MassTransit; // MassTransit kütüphanesini kullanmak için kısaltma tanımlanır.
using MongoDB.Driver; // MongoDB sürücüsünü kullanmak için eklenir.
using System; // Temel sistem türlerini kullanmak için eklenir.
using System.Collections.Generic; // Liste türlerini kullanmak için eklenir.
using System.Linq; // LINQ yöntemlerini kullanmak için eklenir.
using System.Threading.Tasks; // Asenkron programlama desteği için eklenir.
using FreeCourse.Shared.Messages; // Olay mesajlarını kullanmak için eklenir.

namespace FreeCourse.Services.Catalog.Services
{
    // CourseService, MongoDB'deki kurslarla etkileşimi yönetir.
    public class CourseService : ICourseService
    {
        // MongoDB'deki kurs koleksiyonu.
        private readonly IMongoCollection<Course> _courseCollection;
        // MongoDB'deki kategori koleksiyonu.
        private readonly IMongoCollection<Category> _categoryCollection;
        // AutoMapper örneği, nesneleri eşleştirmek için kullanılır.
        private readonly IMapper _mapper;
        // MassTransit yayıncı uç noktası.
        private readonly Mass.IPublishEndpoint _publishEndpoint;

        // Constructor, veritabanı ayarlarını alır ve MongoDB bağlantısını oluşturur.
        public CourseService(IMapper mapper, IDatabaseSettings databaseSettings, Mass.IPublishEndpoint publishEndpoint)
        {
            var client = new MongoClient(databaseSettings.ConnectionString); // MongoDB istemcisi oluşturur.

            var database = client.GetDatabase(databaseSettings.DatabaseName); // Belirtilen veritabanını alır.

            _courseCollection = database.GetCollection<Course>(databaseSettings.CourseCollectionName); // Kurs koleksiyonunu alır.

            _categoryCollection = database.GetCollection<Category>(databaseSettings.CategoryCollectionName); // Kategori koleksiyonunu alır.
            _mapper = mapper; // AutoMapper örneğini atar.

            _publishEndpoint = publishEndpoint; // MassTransit yayıncı uç noktasını atar.
        }

        // Tüm kursları getirir.
        public async Task<Response<List<CourseDto>>> GetAllAsync()
        {
            // Tüm kursları MongoDB'den alır.
            var courses = await _courseCollection.Find(course => true).ToListAsync();

            // Eğer kurs varsa, her kursun kategorisini alır.
            if (courses.Any())
            {
                foreach (var course in courses)
                {
                    course.Category = await _categoryCollection.Find<Category>(x => x.Id == course.CategoryId).FirstAsync();
                }
            }
            else
            {
                courses = new List<Course>(); // Boş bir liste döner.
            }

            // Kursları DTO'lara dönüştürüp başarılı yanıt döner.
            return Response<List<CourseDto>>.Success(_mapper.Map<List<CourseDto>>(courses), 200);
        }

        // Belirli bir kursu ID'sine göre getirir.
        public async Task<Response<CourseDto>> GetByIdAsync(string id)
        {
            // MongoDB'de ID'sine göre kursu bulur.
            var course = await _courseCollection.Find<Course>(x => x.Id == id).FirstOrDefaultAsync();

            // Kurs bulunamazsa hata döner.
            if (course == null)
            {
                return Response<CourseDto>.Fail("Course not found", 404);
            }

            // Kursun kategorisini alır.
            course.Category = await _categoryCollection.Find<Category>(x => x.Id == course.CategoryId).FirstAsync();

            // Kursu DTO'ya dönüştürüp başarılı yanıt döner.
            return Response<CourseDto>.Success(_mapper.Map<CourseDto>(course), 200);
        }

        // Kullanıcıya ait tüm kursları getirir.
        public async Task<Response<List<CourseDto>>> GetAllByUserIdAsync(string userId)
        {
            // Kullanıcıya ait tüm kursları MongoDB'den alır.
            var courses = await _courseCollection.Find<Course>(x => x.UserId == userId).ToListAsync();

            // Eğer kurs varsa, her kursun kategorisini alır.
            if (courses.Any())
            {
                foreach (var course in courses)
                {
                    course.Category = await _categoryCollection.Find<Category>(x => x.Id == course.CategoryId).FirstAsync();
                }
            }
            else
            {
                courses = new List<Course>(); // Boş bir liste döner.
            }

            // Kursları DTO'lara dönüştürüp başarılı yanıt döner.
            return Response<List<CourseDto>>.Success(_mapper.Map<List<CourseDto>>(courses), 200);
        }

        // Yeni bir kurs oluşturur.
        public async Task<Response<CourseDto>> CreateAsync(CourseCreateDto courseCreateDto)
        {
            // DTO'yu Course modeline dönüştürür.
            var newCourse = _mapper.Map<Course>(courseCreateDto);

            newCourse.CreatedTime = DateTime.Now; // Kursun oluşturulma zamanını atar.
            await _courseCollection.InsertOneAsync(newCourse); // MongoDB'ye yeni kursu ekler.

            // Oluşturulan kursu DTO'ya dönüştürüp başarılı yanıt döner.
            return Response<CourseDto>.Success(_mapper.Map<CourseDto>(newCourse), 200);
        }

        // Var olan bir kursu günceller.
        public async Task<Response<NoContent>> UpdateAsync(CourseUpdateDto courseUpdateDto)
        {
            // DTO'yu Course modeline dönüştürür.
            var updateCourse = _mapper.Map<Course>(courseUpdateDto);

            // Kursu günceller.
            var result = await _courseCollection.FindOneAndReplaceAsync(x => x.Id == courseUpdateDto.Id, updateCourse);

            // Eğer güncellenecek kurs bulunamazsa hata döner.
            if (result == null)
            {
                return Response<NoContent>.Fail("Course not found", 404);
            }

            // Kurs adında bir değişiklik olduğunda olayı yayınlar.
            await _publishEndpoint.Publish<CourseNameChangedEvent>(new CourseNameChangedEvent { CourseId = updateCourse.Id, UpdatedName = courseUpdateDto.Name });

            // Başarılı yanıt döner.
            return Response<NoContent>.Success(204);
        }

        // Belirli bir kursu siler.
        public async Task<Response<NoContent>> DeleteAsync(string id)
        {
            // MongoDB'den kursu siler.
            var result = await _courseCollection.DeleteOneAsync(x => x.Id == id);

            // Eğer kurs silinirse başarılı yanıt döner.
            if (result.DeletedCount > 0)
            {
                return Response<NoContent>.Success(204);
            }
            else
            {
                // Eğer silinecek kurs bulunamazsa hata döner.
                return Response<NoContent>.Fail("Course not found", 404);
            }
        }
    }
}
