namespace Otanime.Models.ViewModels;

public class HomeViewModel
{
    public List<Product> FeaturedProducts { get; set; } = new List<Product>();
    public List<Product> NewArrivals { get; set; } = new List<Product>();
}