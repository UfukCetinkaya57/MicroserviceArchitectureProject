using AutoMapper; // AutoMapper kütüphanesini kullanabilmek için eklenir.
using FreeCourse.Services.Catalog.Dtos; // DTO sınıflarını içe aktarıyoruz.
using FreeCourse.Services.Catalog.Model; // Model sınıflarını içe aktarıyoruz.

namespace FreeCourse.Services.Catalog.Mapping
{
    // GeneralMapping sınıfı, AutoMapper kullanarak model ve DTO nesneleri arasındaki eşlemeleri tanımlamak için kullanılır.
    public class GeneralMapping : Profile
    {
        // Constructor, eşleme kurallarını tanımlamak için kullanılır.
        public GeneralMapping()
        {
            // Course modelini CourseDto ile eşleştirir ve tersini de yapar.
            CreateMap<Course, CourseDto>().ReverseMap();
            // Category modelini CategoryDto ile eşleştirir ve tersini de yapar.
            CreateMap<Category, CategoryDto>().ReverseMap();

            // Feature modelini FeatureDto ile eşleştirir ve tersini de yapar.
            CreateMap<Feature, FeatureDto>().ReverseMap();

            // Course modelini CourseCreateDto ile eşleştirir ve tersini de yapar.
            CreateMap<Course, CourseCreateDto>().ReverseMap();
            // Course modelini CourseUpdateDto ile eşleştirir ve tersini de yapar.
            CreateMap<Course, CourseUpdateDto>().ReverseMap();
        }
    }
}
