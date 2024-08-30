Bu proje, mikroservis mimarisi temeline baz alınmış bir online eğitim platformudur. Proje, farklı veritabanları ve teknolojiler kullanarak esnek, ölçeklenebilir ve sürdürülebilir bir yapı sunmayı amaçlamaktadır.

## Kullanılan Teknolojiler

- **.NET Core 5.0**: Projenin ana çatısını oluşturur.
- **ASP.NET Core MVC**: Web uygulama katmanında kullanılmıştır.
- **Entity Framework Core**: Veritabanı işlemleri için kullanıldı.
- **AutoMapper**: Nesne dönüşümleri için kullanıldı.
- **FluentValidation**: Veri doğrulama işlemleri için kullanıldı.
- **Refit**: Mikroservisler arası HTTP tabanlı iletişim için kullanıldı.
- **RabbitMQ**: Mikroservisler arası mesajlaşma altyapısı olarak kullanıldı.
- **MassTransit**: RabbitMQ ile entegrasyon için kullanıldı.
- **Ocelot**: API Gateway olarak görev yapmaktadır.
- **Polly**: Hata yönetimi ve tekrar deneme mekanizması için kullanıldı.
- **Docker**: Mikroservislerin containerize edilmesi ve yönetilmesi için kullanıldı.
- **Kubernetes**: Container orkestrasyonu ve yönetimi için kullanıldı.

## Kullanılan Veritabanları

Her bir mikroservis, kendine özgü bir veritabanı kullanarak veri yönetimini gerçekleştirmektedir:

- **SQL Server**: Order ve Payment servislerinde kullanılmıştır.
- **MongoDB**: Catalog servisi için kullanılmıştır.
- **PostgreSQL**: User servisi için kullanılmıştır.
- **In-Memory Database**: Test süreçlerinde ve bazı servislerde hızlı veri erişimi için kullanılmıştır.
- **Redis**: Caching mekanizması olarak kullanılmıştır.

## Docker Entegrasyonu

Proje, Docker kullanılarak containerize edilmiştir. Aşağıdaki adımlarla Docker kullanarak projeyi çalıştırabilirsiniz:

1. Docker yüklü değilse [Docker'ı indirin ve kurun](https://www.docker.com/get-started).
2. Proje dizininde bir `Dockerfile` ve `docker-compose.yml` dosyası bulunmaktadır.
3. Terminal veya komut satırını açarak proje dizinine gidin.
4. `docker-compose up` komutunu çalıştırarak tüm mikroservisleri başlatın.
5. Docker, tüm bağımlılıkları indirir ve mikroservisleri ilgili container'larda çalıştırır.

## Projenin Amacı ve Özellikleri

Bu proje, bir online eğitim platformunun altyapısını oluşturmayı hedefleyen bir eğitim amaçlı projedir. Projede kullanılan teknolojiler, mikroservis mimarisi ve mesajlaşma altyapısı gibi modern yazılım geliştirme pratikleri üzerine odaklanmıştır.

**Özellikler:**

- Kullanıcı kaydı ve kimlik doğrulama
- Kurs oluşturma, güncelleme ve silme
- Sepet ve ödeme işlemleri
- Sipariş takibi ve yönetimi
- Fotoğraf yükleme ve silme işlemleri (PhotoStock servisi)
- Mikroservisler arası mesajlaşma ve entegrasyon

