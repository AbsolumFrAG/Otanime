using Otanime.Models;

namespace Otanime.ViewModels;

public class ProductDetailViewModel
{
    public Product Product { get; set; }
    public List<Product> RelatedProducts { get; set; }
}