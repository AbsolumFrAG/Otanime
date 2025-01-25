using Otanime.Models;

namespace Otanime.ViewModels;

public class ProductListViewModel
{
    public List<Product> Products { get; set; }
    public List<string> Categories { get; set; }
    public string SearchTerm { get; set; }
    public string SelectedCategory { get; set; }
    public string SortBy { get; set; }
    public PaginationInfo Pagination { get; set; }
}