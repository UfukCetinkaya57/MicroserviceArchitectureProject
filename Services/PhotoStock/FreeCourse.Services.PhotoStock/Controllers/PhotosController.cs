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

        [HttpPost]
        public async Task<IActionResult> PhotoSave(IFormFile photo, CancellationToken cancellationToken)
        {
            if (photo != null && photo.Length > 0)
            {
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/photos", photo.FileName);

                using var stream = new FileStream(path, FileMode.Create);
                await photo.CopyToAsync(stream, cancellationToken);
                var returnPath= "photos/"+ photo.FileName;

                PhotoDto photoDto = new PhotoDto() { Url=returnPath};
                return CreateActionResultinstance(Response<PhotoDto>.Success(photoDto, 200));

            }
            return CreateActionResultinstance(Response<PhotoDto>.Fail("Photo is empty", 400));
        }

        public IActionResult PhotoDelete(string photoUrl)
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/photos", photoUrl);

            if (!System.IO.File.Exists(path))
            {

                return CreateActionResultinstance(Response<NoContent>.Fail("Photo not found", 404));

            }

            System.IO.File.Delete(path);
            return CreateActionResultinstance(Response<NoContent>.Success(204));
        }
    }
}
