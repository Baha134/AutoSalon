namespace AutoSalon.Services;

public interface IFavoriteService
{
    List<int> GetIds(HttpContext ctx);
    void Add(HttpContext ctx, int carId);
    void Remove(HttpContext ctx, int carId);
    bool Contains(HttpContext ctx, int carId);
    int Count(HttpContext ctx);
}