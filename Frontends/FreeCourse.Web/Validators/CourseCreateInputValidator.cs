using FluentValidation; // FluentValidation kütüphanesi
using FreeCourse.Web.Models.Catalogs; // CourseCreateInput model sınıfları
using System; // Temel sistem sınıfları
using System.Collections.Generic; // Koleksiyon sınıfları
using System.Linq; // LINQ işlevleri
using System.Threading.Tasks; // Asenkron programlama için

namespace FreeCourse.Web.Validators
{
    // CourseCreateInput doğrulayıcı sınıfı
    public class CourseCreateInputValidator : AbstractValidator<CourseCreateInput>
    {
        // Yapıcı metot
        public CourseCreateInputValidator()
        {
            // İsim alanı boş olamaz
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("isim alanı boş olamaz");

            // Açıklama alanı boş olamaz
            RuleFor(x => x.Description)
                .NotEmpty()
                .WithMessage("açıklama alanı boş olamaz");

            // Süre alanı 1 ile int.MaxValue arasında olmalıdır
            RuleFor(x => x.Feature.Duration)
                .InclusiveBetween(1, int.MaxValue)
                .WithMessage("süre alanı boş olamaz");

            // Fiyat alanı boş olamaz ve hatalı para formatı kontrolü
            RuleFor(x => x.Price)
                .NotEmpty()
                .WithMessage("fiyat alanı boş olamaz")
                .ScalePrecision(2, 6)
                .WithMessage("hatalı para formatı");

            // Kategori alanı seçilmelidir
            RuleFor(x => x.CategoryId)
                .NotEmpty()
                .WithMessage("kategori alanı seçiniz");
        }
    }
}
