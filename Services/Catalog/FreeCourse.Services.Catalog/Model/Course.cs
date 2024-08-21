using MongoDB.Bson.Serialization.Attributes; // MongoDB'nin BsonSerialization özelliklerini kullanmak için eklenir.
using MongoDB.Bson; // MongoDB'nin Bson türlerini kullanmak için eklenir.

namespace FreeCourse.Services.Catalog.Model
{
    // Course sınıfı, MongoDB veritabanındaki kurs belgelerini temsil eder.
    public class Course
    {
        // Bu özellik, MongoDB'de belge kimliği olarak kullanılacak.
        [BsonId] // Bu, bu özelliğin MongoDB'de belge kimliği (ID) olduğunu belirtir.
        [BsonRepresentation(BsonType.ObjectId)] // Bu, özelliğin ObjectId formatında depolanacağını belirtir.
        public string Id { get; set; }

        // Kursun adını temsil eden özellik.
        public string Name { get; set; }

        // Kursun fiyatını temsil eden özellik.
        [BsonRepresentation(BsonType.Decimal128)] // Bu, fiyatın MongoDB'de Decimal128 formatında depolanacağını belirtir.
        public decimal Price { get; set; }

        // Kursu oluşturan kullanıcının kimliğini temsil eden özellik.
        public string UserId { get; set; }

        // Kursun resim URL'sini temsil eden özellik.
        public string Picture { get; set; }

        // Kursun oluşturulma zamanını temsil eden özellik.
        [BsonRepresentation(BsonType.DateTime)] // Bu, oluşturulma zamanının MongoDB'de DateTime formatında depolanacağını belirtir.
        public DateTime CreatedTime { get; set; }

        // Kursun özelliklerini temsil eden Feature nesnesi.
        public Feature Feature { get; set; }

        // Kursun açıklamasını temsil eden özellik.
        public string Description { get; set; }

        // Kursun ait olduğu kategorinin kimliğini temsil eden özellik.
        [BsonRepresentation(BsonType.ObjectId)] // Bu, özelliğin ObjectId formatında depolanacağını belirtir.
        public string CategoryId { get; set; }

        // Kategoriyi temsil eden Category nesnesi, ancak bu özellik MongoDB'ye kaydedilmeyecek.
        [BsonIgnore] // Bu, bu özelliğin MongoDB'de depolanmayacağını belirtir.
        public Category Category { get; set; }
    }
}
