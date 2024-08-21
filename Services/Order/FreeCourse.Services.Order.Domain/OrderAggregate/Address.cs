using FreeCourse.Services.Order.Domain.Core;
using System.Collections.Generic;

namespace FreeCourse.Services.Order.Domain.OrderAggregate
{
    // Address sınıfı bir ValueObject'tir ve bir siparişin adres bilgilerini tutar.
    // Değer objesi olarak, eşitlik karşılaştırmaları, sahip olduğu değerlere göre yapılır.
    public class Address : ValueObject
    {
        // Adresin bulunduğu il.
        public string Province { get; private set; }

        // Adresin bulunduğu ilçe.
        public string District { get; private set; }

        // Adresin bulunduğu sokak.
        public string Street { get; private set; }

        // Adresin posta kodu.
        public string ZipCode { get; private set; }

        // Adresin diğer detayları (örneğin apartman no, daire no).
        public string Line { get; private set; }

        // Address sınıfının yapıcı metodu (constructor), adres bilgilerini alır ve ilgili alanları doldurur.
        public Address(string province, string district, string street, string zipCode, string line)
        {
            Province = province;
            District = district;
            Street = street;
            ZipCode = zipCode;
            Line = line;
        }

        // Adresin hangi bileşenlerinin eşitlik karşılaştırmalarında kullanılacağını belirten metod.
        // Bu metod, ValueObject sınıfından gelir ve burada override edilir.
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Province;
            yield return District;
            yield return Street;
            yield return ZipCode;
            yield return Line;
        }
    }
}
