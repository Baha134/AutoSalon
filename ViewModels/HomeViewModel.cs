using AutoSalon.Models;

namespace AutoSalon.ViewModels;

public class HomeViewModel
{
    public List<Car> FeaturedCars { get; set; } = new();
    public List<string> TopBrands { get; set; } = new();
    public SalonSettings Settings { get; set; } = null!;
    public int TotalCars { get; set; }
}