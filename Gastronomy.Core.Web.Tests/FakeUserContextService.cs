using Gastronomy.Services.Abstractions;

namespace Gastronomy.Core.Web.Tests;

public sealed class FakeUserContextService : IUserContextService
{
    private readonly Func<Task<Guid>> _restaurantId;

    public FakeUserContextService(Func<Task<Guid>> restaurantId)
    {
        _restaurantId = restaurantId;
    }

    public Task<Guid> RestaurentId => _restaurantId();
}