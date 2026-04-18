using AutoSalon.Models;

namespace AutoSalon.ViewModels;

public class CarDetailViewModel
{
    public Car Car { get; set; } = null!;
    public SalonSettings? Settings { get; set; }
    public List<Car> Similar { get; set; } = new();
}
