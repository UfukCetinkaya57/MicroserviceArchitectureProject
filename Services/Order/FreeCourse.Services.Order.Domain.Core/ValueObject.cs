using System;
using System.Collections.Generic;
using System.Linq;

namespace FreeCourse.Services.Order.Domain.Core
{
    // Bu sınıf, değere dayalı karşılaştırmaların yapılmasını sağlayan bir temel sınıftır.
    // Değer objeleri, bir entity'nin özelliklerini tanımlar ve eşitlik, sahip oldukları değerlere göre belirlenir.
    public abstract class ValueObject
    {
        // İki ValueObject'in eşit olup olmadığını kontrol eden operatör.
        protected static bool EqualOperator(ValueObject left, ValueObject right)
        {
            // Sadece biri null ise false döndür
            if (ReferenceEquals(left, null) ^ ReferenceEquals(right, null))
            {
                return false;
            }
            // Her ikisi de null ise true döndür, değilse Equals metodunu çağır
            return ReferenceEquals(left, null) || left.Equals(right);
        }

        // İki ValueObject'in eşit olmadığını kontrol eden operatör.
        protected static bool NotEqualOperator(ValueObject left, ValueObject right)
        {
            // EqualOperator metodunun tersini döndür
            return !(EqualOperator(left, right));
        }

        // Bu metod, ValueObject'in eşitlik karşılaştırması için kullanılan bileşenlerini döndürür.
        // Her alt sınıf, kendine özgü bileşenleri burada tanımlar.
        protected abstract IEnumerable<object> GetEqualityComponents();

        // ValueObject'lerin eşitliğini kontrol eden override edilmiş metod.
        public override bool Equals(object obj)
        {
            // Gelen objenin null olup olmadığını ve aynı türe sahip olup olmadığını kontrol et
            if (obj == null || obj.GetType() != GetType())
            {
                return false;
            }

            var other = (ValueObject)obj;

            // Eşitlik bileşenlerini karşılaştır
            return this.GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
        }

        // ValueObject'in hash code'unu döndüren override edilmiş metod.
        public override int GetHashCode()
        {
            // Eşitlik bileşenlerinin hash code'larını al ve bunları XOR ile birleştir
            return GetEqualityComponents()
             .Select(x => x != null ? x.GetHashCode() : 0)
             .Aggregate((x, y) => x ^ y);
        }

        // Bu metod, mevcut ValueObject'in bir kopyasını döndürür.
        public ValueObject GetCopy()
        {
            return this.MemberwiseClone() as ValueObject;
        }
    }
}
