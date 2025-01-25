using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Otanime.Data;
using Otanime.ViewModels;

namespace Otanime.Controllers;

public class ProductController(
    ApplicationDbContext context,
    ILogger<ProductController> logger)
    : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index(
        string searchTerm,
        string category,
        string sortBy = "name",
        int page = 1,
        int pageSize = 12)
    {
        var query = context.Products
            .Where(p => p.InStock)
            .AsQueryable();

        // Filtrage
        if (!string.IsNullOrEmpty(searchTerm))
        {
            var lowerSearchTerm = searchTerm.ToLower();
            query = query.Where(p =>
                p.Name.ToLower().Contains(lowerSearchTerm) ||
                p.Description.ToLower().Contains(lowerSearchTerm));
        }

        if (!string.IsNullOrEmpty(category) && category != "Toutes")
        {
            query = query.Where(p => p.Category == category);
        }

        // Tri
        query = sortBy.ToLower() switch
        {
            "price" => query.OrderBy(p => (double)p.Price),
            "price_desc" => query.OrderByDescending(p => (double)p.Price),
            "newest" => query.OrderByDescending(p => p.CreatedAt),
            _ => query.OrderBy(p => p.Name)
        };

        // Pagination
        var totalItems = await query.CountAsync();
        var products = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var vm = new ProductListViewModel
        {
            Products = products,
            Categories = await GetProductCategories(),
            SearchTerm = searchTerm,
            SelectedCategory = category,
            SortBy = sortBy,
            Pagination = new PaginationInfo
            {
                CurrentPage = page,
                ItemsPerPage = pageSize,
                TotalItems = totalItems
            }
        };

        return View(vm);
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var product = await context.Products
            .Include(p => p.CartItems)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (product == null)
        {
            logger.LogWarning("Product {Id} not found", id);
            return NotFound();
        }

        // Produits similaires
        var relatedProducts = await context.Products
            .Where(p => p.Category == product.Category && p.Id != product.Id)
            .Take(4)
            .ToListAsync();

        var vm = new ProductDetailViewModel
        {
            Product = product,
            RelatedProducts = relatedProducts
        };

        return View(vm);
    }

    [HttpGet]
    public async Task<IActionResult> SearchSuggestions(string term)
    {
        var suggestions = await context.Products
            .Where(p => p.Name.Contains(term))
            .Take(5)
            .Select(p => new { label = p.Name, value = p.Id })
            .ToListAsync();

        return Json(suggestions);
    }

    private async Task<List<string>> GetProductCategories()
    {
        return await context.Products
            .Select(p => p.Category)
            .Distinct()
            .OrderBy(c => c)
            .ToListAsync();
    }
}