using System; // Temel sistem sınıfları
using System.Collections.Generic; // Koleksiyon sınıfları
using System.Linq; // LINQ işlevleri
using System.Threading.Tasks; // Asenkron programlama için

namespace FreeCourse.Web.Models
{
    // Kullanıcı bilgilerini temsil eden model
    public class UserViewModel
    {
        // Kullanıcının benzersiz kimliği
        public string Id { get; set; }

        // Kullanıcının adı
        public string UserName { get; set; }

        // Kullanıcının e-posta adresi
        public string Email { get; set; }

        // Kullanıcının yaşadığı şehir
        public string City { get; set; }

        // Kullanıcının özelliklerini döndüren metot
        public IEnumerable<string> GetUserProps()
        {
            yield return UserName; // Kullanıcı adını döndür
            yield return Email; // E-posta adresini döndür
            yield return City; // Şehir bilgisini döndür
        }
    }
}
