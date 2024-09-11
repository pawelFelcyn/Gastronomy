using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Gastronomy.Backend.Database;
using Gastronomy.Dtos;
using Gastronomy.Services.Abstractions;
using LanguageExt.Common;
using Microsoft.EntityFrameworkCore;

namespace Gastronomy.Core.Web;

public sealed class AzurePhotosService : IPhotosService
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly IUserContextService _userContextService;
    private readonly GastronomyDbContext _dbContext;

    public AzurePhotosService(BlobServiceClient blobServiceClient,
        IUserContextService userContextService, GastronomyDbContext dbContext)
    {
        _blobServiceClient = blobServiceClient;
        _userContextService = userContextService;
        _dbContext = dbContext;
    }

    public async Task<Result<IEnumerable<string>>> GetDishPhotos(Guid dishId)
    {
        try
        {
            BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient(dishId.ToString());

            if (!await containerClient.ExistsAsync())
            {
                return new Result<IEnumerable<string>>(Enumerable.Empty<string>());
            }

            var blobUrls = new List<string>();

            await foreach (var blobItem in containerClient.GetBlobsAsync())
            {
                var blobClient = containerClient.GetBlobClient(blobItem.Name);
                var blobUrl = blobClient.Uri.AbsoluteUri;
                blobUrls.Add(blobUrl);
            }

            return blobUrls;
        }
        catch (RequestFailedException)
        {
            return new(new GettingDishImageFailedException());
        }
    }


    public async Task<Result<IEnumerable<string>>> UploadDishPhoto(Guid dishId, UploadPhotoDto[] dto)
    {
        try
        {
            return await (await EnsureNotForbidden(dishId))
                .MapAsync<IEnumerable<string>>(async _ =>
                {
                    BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient(dishId.ToString());
                    await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);


                    var urls = dto.Select(async photoData =>
                    {
                        var randomGuid = Guid.NewGuid();
                        var name = $"{randomGuid}{photoData.Extension}";
                        var blobClient = containerClient.GetBlobClient(name);
                        await blobClient.UploadAsync(photoData.FileStream);

                        return blobClient.Uri.AbsoluteUri;
                    });

                    return await Task.WhenAll(urls);
                });
        }
        catch (RequestFailedException)
        {
            return new(new CreatingDishImageFailedException());
        }
    }

    private async Task<Result<bool>> EnsureNotForbidden(Guid dishId)
    {
        var restaurantId = await _userContextService.RestaurentId;

        var dishRestaurantId = await _dbContext
            .Dishes
            .Where(x => x.Id == dishId)
            .Select(x => x.DishCategory!.RestaurantId)
            .FirstOrDefaultAsync();

        if (dishRestaurantId == default)
        {
            return new(new NotFoundException());
        }

        if (dishRestaurantId != restaurantId)
        {
            return new(new ForbidException());
        }

        return true;
    }

    public async Task<Result<bool>> DeleteDishPhoto(Guid dishId, string photoName)
    {
        try
        {
            return await (await EnsureNotForbidden(dishId))
            .MapAsync(async _ =>
            {
                BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient(dishId.ToString());
                await containerClient.DeleteBlobIfExistsAsync(photoName);
                return true;
            });
        }
        catch (RequestFailedException)
        {
            return new(new DeletingDishImageFailedException());
        }
    }
}