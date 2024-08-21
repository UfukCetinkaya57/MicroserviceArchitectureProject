using FreeCourse.Shared.Dtos;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeCourse.Shared.ControllerBases
{
    // Bu sınıf, diğer controller'lar için temel bir sınıf olarak kullanılır.
    // Ortak işlemleri ve mantıkları burada toplayarak diğer controller'ların bunları miras almasını sağlar.
    public class CustomBaseController : ControllerBase
    {
        // Bu metot, Response<T> tipinde bir nesne alır ve bu nesneye bağlı olarak bir ActionResult döndürür.
        // Response<T> sınıfı, genellikle API yanıtlarını kapsüllemek ve HTTP durum kodlarıyla birlikte veri döndürmek için kullanılır.
        public ActionResult CreateActionResultInstance<T>(Response<T> response)
        {
            // ObjectResult, bir HTTP yanıtı oluşturur ve yanıtın gövdesini (body) 'response' nesnesiyle doldurur.
            // response nesnesinin StatusCode property'si, HTTP yanıtının durum kodunu belirler.
            return new ObjectResult(response)
            {
                StatusCode = response.StatusCode
            };
        }
    }
}
