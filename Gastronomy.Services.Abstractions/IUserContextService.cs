namespace Gastronomy.Services.Abstractions;

public interface IUserContextService
{
    Task<Guid> RestaurentId { get; }
}