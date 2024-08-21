using System;

namespace FreeCourse.Services.Order.Domain.Core
{
    // Bu sınıf, domain modelleri için temel bir varlık sınıfıdır. Tüm entity'ler bu sınıftan türetilir.
    public abstract class Entity
    {
        // Hash code'un önceden hesaplanıp saklanması için kullanılan değişken
        private int? _requestedHashCode;

        // Entity'nin benzersiz kimlik değeri
        private int _Id;

        // Id özelliği. Bu, entity'nin kimlik değerini temsil eder.
        public virtual int Id
        {
            get => _Id;
            set => _Id = value;
        }

        // Bu metot, entity'nin yeni oluşturulmuş olup olmadığını belirler.
        public bool IsTransient()
        {
            return this.Id == default(Int32); // Eğer Id değeri default(int) ise, entity yeni oluşturulmuş demektir.
        }

        // Entity'nin hash code'unu döndüren override edilmiş metot.
        public override int GetHashCode()
        {
            // Eğer entity yeni oluşturulmuş değilse (Id'ye sahipse)
            if (!IsTransient())
            {
                // Hash code daha önce hesaplanmamışsa hesapla
                if (!_requestedHashCode.HasValue)
                    _requestedHashCode = this.Id.GetHashCode() ^ 31; // XOR işlemi ile random dağılım sağlar.

                return _requestedHashCode.Value;
            }
            else
                return base.GetHashCode(); // Eğer transient ise, base sınıfın GetHashCode() metodu çağrılır.
        }

        // Entity'lerin eşitliğini kontrol eden override edilmiş metot.
        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is Entity))
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            if (this.GetType() != obj.GetType())
                return false;

            Entity item = (Entity)obj;

            if (item.IsTransient() || this.IsTransient())
                return false;
            else
                return item.Id == this.Id;
        }

        // Eşitlik operatörü overload
        public static bool operator ==(Entity left, Entity right)
        {
            if (Object.Equals(left, null))
                return (Object.Equals(right, null)) ? true : false;
            else
                return left.Equals(right);
        }

        // Eşit olmama operatörü overload
        public static bool operator !=(Entity left, Entity right)
        {
            if (Object.Equals(left, null))
                return (Object.Equals(right, null)) ? true : false;
            else
                return !left.Equals(right);
        }
    }
}
