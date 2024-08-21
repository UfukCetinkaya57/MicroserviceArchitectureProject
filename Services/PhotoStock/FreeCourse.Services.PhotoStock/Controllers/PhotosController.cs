using FreeCourse.Services.PhotoStock.Dtos;
using FreeCourse.Shared.ControllerBases;
using FreeCourse.Shared.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FreeCourse.Services.PhotoStock.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PhotosController : CustomBaseController
    {
        // Fotoğraf kaydetme metodu
        [HttpPost]
        public async Task<IActionResult> PhotoSave(IFormFile photo, CancellationToken cancellationToken)
        {
            // Fotoğraf null değilse ve boyutu 0'dan büyükse
            if (photo != null && photo.Length > 0)
            {
                // Fotoğrafı kaydetmek için yolu oluştur
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/photos", photo.FileName);

                // Fotoğrafı belirtilen yola kopyala
                using var stream = new FileStream(path, FileMode.Create);
                await photo.CopyToAsync(stream, cancellationToken);

                // Kaydedilen fotoğrafın URL'sini ayarla
                var returnPath = photo.FileName;

                // Fotoğraf DTO'sunu oluştur
                PhotoDto photoDto = new PhotoDto() { Url = returnPath };

                // Başarılı sonuç döndür
                return CreateActionResultInstance(Response<PhotoDto>.Success(photoDto, 200));
            }

            // Fotoğraf boşsa hata döndür
            return CreateActionResultInstance(Response<PhotoDto>.Fail("Photo is empty", 400));
        }

        // Fotoğraf silme metodu
        [HttpDelete]
        public IActionResult PhotoDelete(string photoUrl)
        {
            // Silinecek fotoğrafın yolu
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/photos", photoUrl);

            // Eğer fotoğraf mevcut değilse hata döndür
            if (!System.IO.File.Exists(path))
            {
                return CreateActionResultInstance(Response<NoContent>.Fail("Photo not found", 404));
            }

            // Fotoğrafı sil
            System.IO.File.Delete(path);
            // Başarılı sonuç döndür
            return CreateActionResultInstance(Response<NoContent>.Success(204));
        }
    }
}
