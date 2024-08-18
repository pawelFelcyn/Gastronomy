using Gastronomy.Dtos;
using LanguageExt.Common;

namespace Gastronomy.Services.Abstractions;

public interface IPhotosService
{
    Task<Result<IEnumerable<string>>> UploadDishPhoto(Guid dishId, UploadPhotoDto[] dto);
    Task<Result<IEnumerable<string>>> GetDishPhotos(Guid dishId);
}