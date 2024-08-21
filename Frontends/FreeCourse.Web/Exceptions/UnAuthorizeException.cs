using System; // Temel sistem sınıfları
using System.Collections.Generic; // Koleksiyon sınıfları
using System.Linq; // LINQ işlevleri
using System.Threading.Tasks; // Asenkron programlama için

namespace FreeCourse.Web.Exceptions
{
    // Yetkilendirme hatalarını temsil eden özel istisna sınıfı
    public class UnAuthorizeException : Exception
    {
        // Varsayılan yapıcı
        public UnAuthorizeException() : base()
        {
        }

        // Mesaj ile birlikte yapılandırıcı
        public UnAuthorizeException(string message) : base(message)
        {
        }

        // Mesaj ve iç istisna ile birlikte yapılandırıcı
        public UnAuthorizeException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
