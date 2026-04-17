namespace AutoSalon.Models;

/// <summary>
/// Избранное хранится в cookie, не в БД.
/// Этот класс используется только как DTO внутри сервиса.
/// </summary>
public class Favorite
{
    public int CarId { get; set; }
}