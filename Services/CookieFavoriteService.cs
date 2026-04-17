namespace AutoSalon.Services;

public class CookieFavoriteService : IFavoriteService
{
    private const string CookieKey = "fav";
    private const int MaxItems = 20;

    public List<int> GetIds(HttpContext ctx)
    {
        var val = ctx.Request.Cookies[CookieKey];
        if (string.IsNullOrEmpty(val)) return new();

        return val.Split(',', StringSplitOptions.RemoveEmptyEntries)
                  .Select(s => int.TryParse(s, out var id) ? id : 0)
                  .Where(id => id > 0)
                  .Distinct()
                  .ToList();
    }

    public void Add(HttpContext ctx, int carId)
    {
        var ids = GetIds(ctx);
        if (ids.Contains(carId)) return;
        if (ids.Count >= MaxItems) ids.RemoveAt(0);
        ids.Add(carId);
        SetCookie(ctx, ids);
    }

    public void Remove(HttpContext ctx, int carId)
    {
        var ids = GetIds(ctx);
        ids.Remove(carId);
        SetCookie(ctx, ids);
    }

    public bool Contains(HttpContext ctx, int carId) => GetIds(ctx).Contains(carId);

    public int Count(HttpContext ctx) => GetIds(ctx).Count;

    private static void SetCookie(HttpContext ctx, List<int> ids)
    {
        ctx.Response.Cookies.Append(CookieKey, string.Join(",", ids), new CookieOptions
        {
            Expires = DateTimeOffset.UtcNow.AddDays(30),
            HttpOnly = true,
            SameSite = SameSiteMode.Lax
        });
    }
}