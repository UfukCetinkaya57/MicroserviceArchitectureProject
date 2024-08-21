using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace FreeCourse.Shared.Services
{
    // Bu servis, uygulama genelinde kullanıcı kimliğini almak için kullanılan bir servistir.
    public class SharedIdentityService : ISharedIdentityService
    {
        // IHttpContextAccessor, ASP.NET Core uygulamasında HTTP isteklerine erişim sağlar.
        // Bu servis, mevcut HTTP isteği bağlamındaki bilgilere ulaşabilmek için IHttpContextAccessor kullanır.
        private IHttpContextAccessor _httpcontextAccessor;

        // Constructor, IHttpContextAccessor bağımlılığını alır ve bunu sınıf içindeki bir alan değişkenine atar.
        public SharedIdentityService(IHttpContextAccessor httpcontextAccessor)
        {
            _httpcontextAccessor = httpcontextAccessor;
        }

        // GetUserId, mevcut HTTP bağlamındaki kullanıcının kimliğini (ID) döndürür.
        // "sub" ifadesi, JWT (JSON Web Token) içindeki "Subject" claim'ine karşılık gelir ve genellikle kullanıcının ID'sini içerir.
        public string GetUserId => _httpcontextAccessor.HttpContext.User.FindFirst("sub").Value;

    }
}
