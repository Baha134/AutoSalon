namespace AutoSalon.Models;

public class CarPhoto
{
    public int Id { get; set; }
    public int CarId { get; set; }
    public string FilePath { get; set; } = "";
    public bool IsMain { get; set; }
    public int SortOrder { get; set; }
    public Car? Car { get; set; }
}