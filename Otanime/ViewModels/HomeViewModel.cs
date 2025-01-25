using Otanime.Models;

namespace Otanime.ViewModels;

public class HomeViewModel
{
    public List<Product> Products { get; set; }
    public List<string?> Categories { get; set; }
    public string SearchTerm { get; set; }
    public string SelectedCategory { get; set; }
    public string SortBy { get; set; }
    public PaginationInfo Pagination { get; set; }
}

public class PaginationInfo
{
    public int CurrentPage { get; set; }
    public int ItemsPerPage { get; set; }
    public int TotalItems { get; set; }
    public int TotalPages => (int)Math.Ceiling((decimal)TotalItems / ItemsPerPage);
}