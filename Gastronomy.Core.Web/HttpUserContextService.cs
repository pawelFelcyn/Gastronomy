using Gastronomy.Backend.Database;
using Gastronomy.Services.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Gastronomy.Core.Web;

public class HttpUserContextService : IUserContextService
{
    private readonly GastronomyDbContext _dbContext;

    //TODO: so far this is mocked, later I need to add authentication here
    public HttpUserContextService(GastronomyDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<Guid> RestaurentId => _dbContext.Restaurants.Select(x => x.Id).FirstAsync();
}