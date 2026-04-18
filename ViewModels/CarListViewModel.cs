using AutoSalon.Models;

namespace AutoSalon.ViewModels;

public class CarListViewModel
{
    public List<Car> Cars { get; set; } = new();
    public CarFilterViewModel Filter { get; set; } = new();
    public int TotalCount { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / (Filter.PageSize > 0 ? Filter.PageSize : 12));
    public List<string> Brands { get; set; } = new();
    public List<string> BodyTypes { get; set; } = new();
}



