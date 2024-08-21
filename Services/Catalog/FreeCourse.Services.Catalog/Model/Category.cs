using MongoDB.Bson; // MongoDB'nin Bson türlerini kullanmak için eklenir.
using MongoDB.Bson.Serialization.Attributes; // MongoDB'nin BsonSerialization özelliklerini kullanmak için eklenir.

namespace FreeCourse.Services.Catalog.Model
{
    // Category sınıfı, MongoDB veritabanındaki kategori belgelerini temsil eder.
    public class Category
    {
        // Bu özellik, MongoDB'de belge kimliği olarak kullanılacak.
        [BsonId] // Bu, bu özelliğin MongoDB'de belge kimliği (ID) olduğunu belirtir.
        [BsonRepresentation(BsonType.ObjectId)] // Bu, özelliğin ObjectId formatında depolanacağını belirtir.
        public string Id { get; set; }

        // Kategorinin adını temsil eden özellik.
        public string Name { get; set; }
    }
}
